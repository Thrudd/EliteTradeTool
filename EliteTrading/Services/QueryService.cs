using EliteTrading.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Data.Entity;
using EliteTrading.Models.ViewModels;
using EliteTrading.Models;
using EliteTrading.Extensions;
using System.Collections;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using EliteTrading.Models.Calculator;
using EliteTrading.Models.Search;

namespace EliteTrading.Services {
    public class QueryService {
        public async Task<Commodity> GetCommodityById(int Id) {
            using (ApplicationDbContext db = new ApplicationDbContext()) {
                return await db.Commodities.FirstOrDefaultAsync(m => m.Id == Id);
            }
        }



        //public async Task<StationDetailViewModel> GetStationDetail(string stationName) {
        //    int stationId = await db.Stations.Where(m=>m.Name == stationName).Select(m=>m.Id).FirstAsync();
        //    return await GetStationDetail(stationId);
        //}

        public async Task<StationDetailViewModel> GetStationDetail(int id) {
            using (ApplicationDbContext db = new ApplicationDbContext()) {
                StationDetailViewModel model = new StationDetailViewModel();
                model.Station = await db.Stations
                    .Include(m => m.Economy)
                    .Include(m => m.SecondaryEconomy)
                    .Include(m => m.StationType)
                    .Include(m => m.System)
                    .Include(m => m.System.Stations)
                    .Include("System.Stations.Economy")
                    .Include(m => m.Allegiance)
                    .Include(m => m.Government)
                    .Where(m => m.Id == id)
                    .FirstOrDefaultAsync();
                if (model.Station.HasMarket) {
                    CommodityList list = new CommodityList();
                    List<Category> commodityCategories = await db.Categories.OrderBy(m => m.Name).ToListAsync();
                    List<StationCommodity> stationCommodities = await db.StationCommodities
                        .Include(m => m.Commodity)
                        .Where(m => m.StationId == id)
                        .OrderBy(m => m.Commodity.Name)
                        .ToListAsync();
                    foreach (Category commodityCategory in commodityCategories) {
                        CommodityCategory cat = new CommodityCategory();
                        cat.Name = commodityCategory.Name;
                        cat.StationCommodity = stationCommodities.Where(m => m.Commodity.CategoryId == commodityCategory.Id).ToList();
                        list.Category.Add(cat);
                    }
                    model.StationCommodities = list;
                }
                return model;
            }
        }



        public async Task<EliteTrading.Models.DataLists.DataListsResult> DataLists(EliteTrading.Models.DataLists.DataListsQuery query) {
            using (ApplicationDbContext db = new ApplicationDbContext()) {

                EliteTrading.Models.DataLists.DataListsResult model = new EliteTrading.Models.DataLists.DataListsResult();
                model.QueryType = query.QueryType;
                if (query.QueryType == EliteTrading.Models.DataLists.DataListsQueryType.System) {
                    var sys = await db.Systems
                                            .AsNoTracking()
                                            .Include(m => m.Allegiance)
                                            .Include(m => m.Government)
                                            .FirstOrDefaultAsync(m => m.Name.ToLower() == query.SystemName.ToLower());
                    if (sys != null) {
                        model.System = new Models.DataLists.System {
                            Id = sys.Id,
                            Name = sys.Name,
                            Allegiance = sys.Allegiance.Name,
                            Government = sys.Government.Name
                        };

                        var stations = await db.Stations
                                                .AsNoTracking()
                                                .Include(m => m.Economy)
                                                .Include(m => m.SecondaryEconomy)
                                                .Include(m => m.StationType)
                                                .Include(m => m.StationCommodities)
                                                .Include("StationCommodities.Commodity")
                                                .Include("StationCommodities.Commodity.Category")
                                                .Where(m => m.System.Name.ToLower() == query.SystemName.ToLower())
                                                .OrderBy(m => m.Name)
                                                .ToListAsync();

                        var categories = await db.Categories.OrderBy(m => m.Name).ToListAsync();

                        model.System.Stations = stations.Select(m => new Models.DataLists.Station {
                            Id = m.Id,
                            Name = m.Name,
                            FactionName = m.FactionName,
                            Allegiance = sys.Allegiance.Name,
                            Government = sys.Government.Name,
                            Economy = m.SecondaryEconomyId.HasValue ? m.Economy.Name + "/" + m.SecondaryEconomy.Name : m.Economy.Name,
                            StationType = m.StationType.Name,
                            StationTypeIcon = m.StationType.Icon,
                            Services = m.Services,
                            Pads = m.StationType.Pads,
                            DistanceFromJumpIn = m.DistanceFromJumpIn,
                            CommodityCategories = new List<Models.DataLists.CommodityCategory>()
                        }).ToList();


                        foreach (Models.DataLists.Station station in model.System.Stations) {
                            foreach (Category cat in categories) {
                                if (stations.Where(m => m.Id == station.Id).First().StationCommodities.Where(m => m.Commodity.CategoryId == cat.Id).Any()) {

                                    var newCat = new Models.DataLists.CommodityCategory {
                                        Id = cat.Id,
                                        Name = cat.Name,
                                        Commodities = stations.Where(m => m.Id == station.Id).First()
                                                        .StationCommodities.Where(m => m.Commodity.CategoryId == cat.Id).OrderBy(m => m.Commodity.Name)
                                                        .Select(m => new Models.DataLists.Commodity {
                                                            Id = m.Id,
                                                            Name = m.Commodity.Name,
                                                            Buy = m.Buy,
                                                            Sell = m.Sell,
                                                            AveragePrice = m.Commodity.GalacticAveragePrice,
                                                            Supply = m.Supply,
                                                            SupplyAmount = m.SupplyAmount,
                                                            Demand = m.Demand,
                                                            DemandAmount = m.DemandAmount,
                                                            LastUpdate = m.LastUpdate.TimeSince(),
                                                            LastUpdatedBy = m.UpdatedBy
                                                        }).ToList()
                                    };
                                    station.CommodityCategories.Add(newCat);
                                }
                            }
                        }
                    }
                } else {
                    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
                    SqlCommand cmd = new SqlCommand("DataListHaveWant", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter param;
                    param = cmd.Parameters.Add("@SystemName", SqlDbType.NVarChar, 30);
                    param.Value = query.SystemName;

                    param = cmd.Parameters.Add("@CommodityId", SqlDbType.Int);
                    param.Value = query.CommodityId;

                    param = cmd.Parameters.Add("@QueryType", SqlDbType.Int);
                    param.Value = (int)query.QueryType;

                    param = cmd.Parameters.Add("@ExcludeOutposts", SqlDbType.Bit);
                    param.Value = query.ExcludeOutposts;

                    param = cmd.Parameters.Add("@ExcludePlanets", SqlDbType.Bit);
                    param.Value = query.ExcludePlanets;

                    param = cmd.Parameters.Add("@SearchRange", SqlDbType.Float);
                    param.Value = query.SearchRange;

                    double version = double.Parse(ConfigurationManager.AppSettings["CurrentVersion"]);
                    param = cmd.Parameters.Add("@Version", SqlDbType.Float);
                    param.Value = version;

                    model.CommodityList = new List<Models.DataLists.CommodityListItem>();

                    try {
                        // Execute the command.
                        con.Open();
                        SqlDataReader reader = await cmd.ExecuteReaderAsync();
                        if (reader.HasRows) {


                            /* Second is the starting stations goods for sale */
                            while (reader.Read()) {
                                EliteTrading.Models.DataLists.CommodityListItem item = new EliteTrading.Models.DataLists.CommodityListItem();
                                item.Location = reader["Location"].ToString();
                                item.DistanceFromJumpIn = (double)reader["DistanceFromJumpIn"];
                                item.StationAllegiance = reader["StationAllegiance"].ToString();
                                item.Sell = (int)reader["Sell"];
                                item.Buy = (int)reader["Buy"];
                                item.GalacticAveragePrice = (int)reader["GalacticAveragePrice"];
                                item.StationTypeName = reader["StationTypeName"].ToString();
                                item.StationTypeIcon = reader["StationTypeIcon"].ToString();
                                item.LastUpdate = ((DateTime)reader["LastUpdate"]).TimeSince();
                                item.LastUpdatedBy = reader["UpdatedBy"].ToString();
                                item.Distance = (double)reader["Distance"];
                                if (query.QueryType == Models.DataLists.DataListsQueryType.StationBuys) {
                                    item.Demand = reader["Demand"].ToString();
                                    item.DemandAmount = (int)reader["DemandAmount"];
                                } else {
                                    item.Supply = reader["Supply"].ToString();
                                    item.SupplyAmount = (int)reader["SupplyAmount"];
                                }
                                item.PermitRequired = (bool)reader["PermitRequired"];
                                model.CommodityList.Add(item);
                            }
                        }
                        reader.Close();
                    } catch (Exception ex) {
                        Exception e = new Exception(
                            "CommodityId=" + query.CommodityId +
                            " SystemName=" + query.SystemName +
                            " QueryType=" + query.QueryType, ex);
                        Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                    } finally {
                        con.Close();
                    }
                }

                return model;
            }
        }




        //public async Task<WhatToBuyViewModel> Station(int StationId) {
        //    Station currentStation = await db.Stations.Include(s => s.System).FirstOrDefaultAsync(s => s.Id == StationId);

        //    WhatToBuyViewModel model = new WhatToBuyViewModel();
        //    model.CommodityRoutes = new List<CommodityRouteList>();
        //    model.StationRoutes = new List<StationRouteList>();
        //    model.Location = currentStation.System.Name + " (" + currentStation.Name + ")";


        //    // Get all StationCommodities
        //    List<StationCommodity> stationsCommodities = await db.StationCommodities
        //            .Include(sc => sc.Commodity)
        //            .Include(sc => sc.Station)
        //            .Include(sc => sc.Station.System)
        //            .ToListAsync();

        //    // Get the commodities at the current station
        //    List<StationCommodity> currentStationsCommodities = stationsCommodities.Where(sc => sc.StationId == StationId && sc.Buy > 1)
        //        .OrderBy(sc => sc.Commodity.Name)
        //        .ToList();

        //    // Loop through the comodities
        //    foreach (StationCommodity stationCommodity in currentStationsCommodities) {
        //        // Get destinations that buy the commodity, exclude the current station
        //        List<StationCommodity> query = stationsCommodities.Where(sc => sc.Commodity.Id == stationCommodity.CommodityId && sc.Sell > stationCommodity.Buy && sc.StationId != StationId)
        //            .OrderBy(sc => sc.Buy)
        //            .ToList();

        //        // loop through the results making CommodityRoute items and adding them to the list
        //        var commodityList = new List<CommodityRoute>();
        //        foreach (StationCommodity route in query) {
        //            commodityList.Add(new CommodityRoute {
        //                CommodityId = stationCommodity.Commodity.Id,
        //                CommodityName = stationCommodity.Commodity.Name,
        //                DestinationId = route.StationId,
        //                DestinationName = route.Station.System.Name + " (" + route.Station.Name + ")",
        //                Buy = stationCommodity.Buy,
        //                Sell = route.Sell,
        //                GalacticAveragePrice = stationCommodity.Commodity.GalacticAveragePrice,
        //                Profit = route.Sell - stationCommodity.Buy,
        //                UpdatedBy = stationCommodity.UpdatedBy
        //            });
        //        }
        //        if (commodityList.Any()) {
        //            model.CommodityRoutes.Add(new CommodityRouteList {
        //                Commodity = stationCommodity.Commodity.Name,
        //                Routes = commodityList.OrderByDescending(m => m.Profit).ToList()
        //            });
        //        }
        //    }

        //    // Get stations other than the current one
        //    List<Station> stations = await db.Stations.Include(s => s.System).Where(s => s.Id != StationId).ToListAsync();
        //    // Loop through the stations
        //    foreach (Station station in stations) {
        //        var stationList = new List<CommodityRoute>();
        //        // Loop through the commodities available at the current station
        //        foreach (StationCommodity stationCommodity in currentStationsCommodities) {
        //            // Get stationcomodities that are in demand
        //            List<StationCommodity> query = stationsCommodities.Where(sc => sc.Commodity.Id == stationCommodity.CommodityId && sc.Sell > stationCommodity.Buy && sc.StationId == station.Id)
        //                .OrderBy(sc => sc.Buy)
        //                .ToList();

        //            if (stationsCommodities.Any()) {
        //                foreach (StationCommodity route in query) {
        //                    stationList.Add(new CommodityRoute {
        //                        CommodityId = route.Commodity.Id,
        //                        CommodityName = route.Commodity.Name,
        //                        DestinationId = station.Id,
        //                        DestinationName = station.Name + " (" + station.Name + ")",
        //                        Buy = stationCommodity.Buy,
        //                        Sell = route.Sell,
        //                        GalacticAveragePrice = stationCommodity.Commodity.GalacticAveragePrice,
        //                        Profit = route.Sell - stationCommodity.Buy,
        //                        UpdatedBy = stationCommodity.UpdatedBy
        //                    });
        //                }
        //            }
        //        }
        //        if (stationList.Any()) {
        //            model.StationRoutes.Add(new StationRouteList {
        //                Station = station.System.Name + " (" + station.Name + ")",
        //                Routes = stationList.OrderByDescending(m => m.Profit).ToList()
        //            });
        //        }
        //    }
        //    return model;
        //}




        public async Task<SearchResultViewModel> Search(SearchQuery query) {
            using (ApplicationDbContext db = new ApplicationDbContext()) {

#if DEBUG
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
#endif

                List<Station> stations = new List<Station>();
                SearchResultViewModel result = new SearchResultViewModel();
                
                var currentSystem = await db.Systems.AsNoTracking().FirstOrDefaultAsync(s => s.Name == query.CurrentLocation);


                Dictionary<int, double> distances = new Dictionary<int, double>();
                // limit search by range
                var allDistances = await GetDistances(currentSystem.Id, query.SearchRange);
                distances = allDistances.Where(x => x.Value < query.SearchRange).ToDictionary(x => x.Key, x => x.Value);
                
                

                List<StationCommodity> stationCommoditiesList = new List<StationCommodity>();

                double version = double.Parse(ConfigurationManager.AppSettings["CurrentVersion"]);

                if (query.Commodity) {
                    Station currentStation = await db.Stations.AsNoTracking().Include(s => s.System).Include(m => m.Economy).Include(m => m.SecondaryEconomy).FirstOrDefaultAsync(s => s.System.Name == query.CurrentLocation);
                    if (currentStation == null)
                        return result;

                    switch (query.SearchType) {
                        case "Station Selling":
                            stationCommoditiesList = await db.StationCommodities
                                .AsNoTracking()
                                .Include(x => x.Station)
                                .Include(x => x.Station.StationType)
                                .Include(x => x.Station.System)
                                .Include(x => x.Station.Economy)
                                .Include(x => x.Station.SecondaryEconomy)
                                .Include(x => x.Station.Government)
                                .Include(x => x.Station.Allegiance)
                                .Where(x => x.CommodityId == query.CommodityId && x.Buy > 0 && x.Version == version &&
                                    x.Station.System.X < (currentStation.System.X + query.SearchRange) && x.Station.System.X > (currentStation.System.X - query.SearchRange) &&
                                    x.Station.System.Y < (currentStation.System.Y + query.SearchRange) && x.Station.System.Y > (currentStation.System.Y - query.SearchRange) &&
                                    x.Station.System.Z < (currentStation.System.Z + query.SearchRange) && x.Station.System.Z > (currentStation.System.Z - query.SearchRange)
                                    ).ToListAsync();

                            stations = stationCommoditiesList.Select(x => x.Station).ToList();
                            break;

                        case "Station Buying":
                            stationCommoditiesList = await db.StationCommodities
                                .AsNoTracking()
                                .Include(x => x.Station)
                                .Include(x => x.Station.StationType)
                                .Include(x => x.Station.System)
                                .Include(x => x.Station.Economy)
                                .Include(x => x.Station.SecondaryEconomy)
                                .Include(x => x.Station.Government)
                                .Include(x => x.Station.Allegiance)
                                .Where(x => x.CommodityId == query.CommodityId && x.Sell > 0 && x.Version == version &&
                                    x.Station.System.X < (currentStation.System.X + query.SearchRange) && x.Station.System.X > (currentStation.System.X - query.SearchRange) &&
                                    x.Station.System.Y < (currentStation.System.Y + query.SearchRange) && x.Station.System.Y > (currentStation.System.Y - query.SearchRange) &&
                                    x.Station.System.Z < (currentStation.System.Z + query.SearchRange) && x.Station.System.Z > (currentStation.System.Z - query.SearchRange)
                                    ).ToListAsync();

                            stations = stationCommoditiesList.Select(x => x.Station).ToList();
                            break;
                    }
                } else {
                    stations = await db.Stations
                        .AsNoTracking()
                        .Include(x => x.System)
                        .Include(x => x.Economy)
                        .Include(x => x.SecondaryEconomy)
                        .Include(x => x.Government)
                        .Include(x => x.Allegiance)
                        .Include(x => x.StationType)
                        .Where(x => x.Version == version &&
                            x.System.X < (currentSystem.X + query.SearchRange) && x.System.X > (currentSystem.X - query.SearchRange) &&
                            x.System.Y < (currentSystem.Y + query.SearchRange) && x.System.Y > (currentSystem.Y - query.SearchRange) &&
                            x.System.Z < (currentSystem.Z + query.SearchRange) && x.System.Z > (currentSystem.Z - query.SearchRange)
                            )
                        .ToListAsync();
                }



                if(query.ExcludeOutposts && query.ExcludePlanets){
                    stations = stations.Where(s => s.StationType.Id == 1).ToList();
                } else if(query.ExcludeOutposts && !query.ExcludePlanets) {
                    stations = stations.Where(s => s.StationType.Id != 2).ToList();
                } else if (!query.ExcludeOutposts && query.ExcludePlanets) {
                    stations = stations.Where(s => s.StationType.Id != 3).ToList();
                }

                stations = stations.Where(m => distances.Any(x => x.Key == m.SystemId)).ToList();

                if (query.FactionName.Length > 0) {
                    stations = stations.Where(x => x.FactionName.ToLower().Contains(query.FactionName.ToLower())).ToList();
                }

                if (query.Blackmarket) {
                    stations = stations.Where(x => x.HasBlackmarket).ToList();
                }
                if (query.Outfitting) {
                    stations = stations.Where(x => x.HasOutfitting).ToList();
                }
                if (query.Shipyard) {
                    stations = stations.Where(x => x.HasShipyard).ToList();
                }
                if (query.Repairs) {
                    stations = stations.Where(x => x.HasRepairs).ToList();
                }
                if (query.Refuel) {
                    stations = stations.Where(x => x.HasRefuel).ToList();
                }
                if (query.Rearm) {
                    stations = stations.Where(x => x.HasRearm).ToList();
                }
                if (query.Economy) {
                    stations = stations.Where(x => x.EconomyId == query.EconomyId || x.SecondaryEconomyId == query.EconomyId).ToList();
                }
                if (query.Government) {
                    stations = stations.Where(x => x.GovernmentId == query.GovernmentId).ToList();
                }
                if (query.Allegiance) {
                    stations = stations.Where(x => x.AllegianceId == query.AllegianceId).ToList();
                }

                result.Query = query;
                result.Results = stations.Select(x => new SearchedStation {
                    Station = x,
                    SystemName = x.System.Name,
                    System = x.System,
                    Distance = distances[x.SystemId]
                }).ToList();



                if (query.Commodity) {
                    switch (query.SearchType) {
                        case "Station Selling":
                            foreach (SearchedStation station in result.Results) {
                                var sc = stationCommoditiesList.Where(x => x.CommodityId == query.CommodityId && x.StationId == station.Station.Id && x.Buy > 0).FirstOrDefault();
                                station.Buy = sc.Buy;
                                station.Supply = sc.Supply;
                                station.SupplyAmount = sc.SupplyAmount;
                                station.LastUpdate = sc.LastUpdate.TimeSince();
                            }
                            break;
                        case "Station Buying":
                            foreach (SearchedStation station in result.Results) {
                                var sc = stationCommoditiesList.Where(x => x.CommodityId == query.CommodityId && x.StationId == station.Station.Id && x.Sell > 0).FirstOrDefault();
                                station.Sell = sc.Sell;
                                station.Demand = sc.Demand;
                                station.DemandAmount = sc.DemandAmount;
                                station.LastUpdate = sc.LastUpdate.TimeSince();
                            }
                            break;
                    }
                }

#if DEBUG
            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value. 
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);

            Debug.WriteLine("RunTime " + elapsedTime);
#endif
                return result;
            }
        }




//        public async Task<CalculatorByProfitViewModel> Calculator(CalculatorQuery query) {
                 
//#if DEBUG
//                Stopwatch stopWatch = new Stopwatch();
//                stopWatch.Start();
//#endif

//            CalcStation startStation = new CalcStation();
//            List<CalcStation> destinationStations = new List<CalcStation>();
//            List<CalcStationCommodity> startStationGoods = new List<CalcStationCommodity>();
//            List<CalcStationCommodity> destinationStationGoods = new List<CalcStationCommodity>();
//            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
//            SqlCommand cmd = new SqlCommand("Calculator", con);
//            cmd.CommandType = CommandType.StoredProcedure;

//            SqlParameter param;
//            param = cmd.Parameters.Add("@StartStationId", SqlDbType.Int);
//            param.Value = query.StartStationId;

//            param = cmd.Parameters.Add("@EndStationId", SqlDbType.Int);
//            param.Value = query.EndStationId;

//            param = cmd.Parameters.Add("@Cash", SqlDbType.Int);
//            param.Value = query.Cash;

//            param = cmd.Parameters.Add("@SearchRange", SqlDbType.Float);
//            param.Value = query.SearchRange;

//            param = cmd.Parameters.Add("@MaxDistanceFromJumpIn", SqlDbType.Float);
//            param.Value = query.MaxDistanceFromJumpIn;

//            param = cmd.Parameters.Add("@PadSize", SqlDbType.NVarChar, 10);
//            param.Value = query.PadSize;

//            double version = double.Parse(ConfigurationManager.AppSettings["CurrentVersion"]);
//            param = cmd.Parameters.Add("@Version", SqlDbType.Float);
//            param.Value = version;


//            try {
//                // Execute the command.
//                con.Open();
//                SqlDataReader reader = await cmd.ExecuteReaderAsync();
//                if (reader.HasRows) {
//                    reader.Read();
//                    /* First result is the starting station details */
//                    startStation.StationId = (int)reader["StationId"];
//                    startStation.SystemId = (int)reader["SystemId"];
//                    startStation.StationName = reader["StationName"].ToString();
//                    startStation.SystemName = reader["SystemName"].ToString();
//                    startStation.X = (double)reader["X"];
//                    startStation.Y = (double)reader["Y"];
//                    startStation.Z = (double)reader["Z"];
//                    startStation.Distance = 0;

//                    reader.NextResult();

//                    /* Second is the starting stations goods for sale */
//                    while (reader.Read()) {
//                        CalcStationCommodity item = new CalcStationCommodity();
//                        item.CommodityId = (int)reader["CommodityId"];
//                        item.StationId = (int)reader["StationId"];
//                        item.CommodityName = reader["CommodityName"].ToString();
//                        item.Buy = (int)reader["Buy"];
//                        item.GalacticAveragePrice = (int)reader["GalacticAveragePrice"];
//                        item.LastUpdate = (DateTime)reader["LastUpdate"];
//                        item.UpdatedBy = reader["UpdatedBy"].ToString();

//                        startStationGoods.Add(item);
//                    }

//                    reader.NextResult();

//                    /* Third result set is the destination station or stations depending if query.EndStationId is null */
//                    while (reader.Read()) {
//                        CalcStation destStation = new CalcStation();
//                        destStation.StationId = (int)reader["StationId"];
//                        destStation.SystemId = (int)reader["SystemId"];
//                        destStation.StationName = reader["StationName"].ToString();
//                        destStation.SystemName = reader["SystemName"].ToString();
//                        destStation.X = (double)reader["X"];
//                        destStation.Y = (double)reader["Y"];
//                        destStation.Z = (double)reader["Z"];
//                        destStation.Distance = Math.Round(Math.Sqrt(Math.Pow(Math.Abs(destStation.X - startStation.X), 2) + Math.Pow(Math.Abs(destStation.Y - startStation.Y), 2) + Math.Pow(Math.Abs(destStation.Z - startStation.Z), 2)), 2);
//                        destStation.DistanceFromJumpIn = (double)reader["DistanceFromJumpIn"];
//                        destinationStations.Add(destStation);
//                    }

//                    reader.NextResult();

//                    /* Forth result is the list of station commodities bought at the desitination station or stations in range */
//                    while (reader.Read()) {
//                        var startGood = startStationGoods.Where(m => m.CommodityId == (int)reader["CommodityId"]).FirstOrDefault();
//                        if (startGood != null && startGood.Buy > 0) {
//                            CalcStationCommodity item = new CalcStationCommodity();
//                            item.CommodityId = (int)reader["CommodityId"];
//                            item.StationId = (int)reader["StationId"];
//                            item.CommodityName = reader["CommodityName"].ToString();
//                            item.Sell = (int)reader["Sell"];
//                            item.LastUpdate = (DateTime)reader["LastUpdate"];
//                            item.UpdatedBy = reader["UpdatedBy"].ToString();
//                            item.Buy = startGood.Buy;
//                            item.Profit = item.Sell - item.Buy;
//                            destinationStationGoods.Add(item);
//                        }
//                    }
//                }


//                reader.Close();
//            } catch (Exception ex) {
//                Exception e = new Exception(
//                    "StartStationId=" + query.StartStationId + 
//                    " EndStationId="+ query.EndStationId +
//                    " Cash="+ query.Cash+
//                    " SearchRange=" + query.SearchRange +
//                    " PadSize=" + query.PadSize +
//                    " MinProfit=" + query.MinProfit +
//                    " Cargo="+ query.Cargo, ex);
//                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
//            } finally {
//                con.Close();
//            }

//            CalculatorByProfitViewModel model = new CalculatorByProfitViewModel();
//            List<CalculatorResult> calculatorResults = new List<CalculatorResult>();
//            if (destinationStationGoods.Any()) {
//                destinationStationGoods = destinationStationGoods.OrderByDescending(m=>m.Profit).ToList();

//                foreach (CalcStationCommodity item in destinationStationGoods) {

//                    CalcStation dest = destinationStations.Where(m=>m.StationId == item.StationId).First();
//                    CalcStationCommodity scAtStartStation = startStationGoods.Where(m=>m.CommodityId == item.CommodityId).First();
//                    // Add the result
//                    var result = new CalculatorResult {
//                        StartingStationId = startStation.StationId,
//                        StartingStationName = startStation.StationName,
//                        StartingSystemName = startStation.SystemName,
//                        DestinationStationId = item.StationId,
//                        DestinationStationName = dest.StationName,
//                        DestinationSystemName = dest.SystemName,
//                        Distance = dest.Distance,
//                        CommodityId = item.CommodityId,
//                        CommodityName = item.CommodityName,
//                        Buy = scAtStartStation.Buy,
//                        Sell = item.Sell,
//                        GalacticAveragePrice = scAtStartStation.GalacticAveragePrice,
//                        Profit = item.Profit,
//                        LastUpdate = item.LastUpdate.TimeSince(),
//                        UpdatedBy = item.UpdatedBy,
//                        DistanceFromJumpIn = dest.DistanceFromJumpIn
//                    };

//                    // work out how many can be bought
//                    int max = (int)Math.Floor((double)query.Cash / (double)result.Buy);

//                    // work out if we can fit the max into the cargo space
//                    result.Qty = max <= query.Cargo ? max : query.Cargo;
//                    result.Total = result.Qty * result.Profit;

//                    if (result.Total >= query.MinProfit) {
//                        calculatorResults.Add(result);
//                    }
//                }

//                model.StartingSystemId = startStation.SystemId;
//                model.StartingSystemName = startStation.SystemName;
//                if (query.EndStationId.HasValue)
//                    model.DestinationSystemName = destinationStations.First().SystemName;
//                model.Location = startStation.SystemName + " (" + startStation.StationName + ")";
//                model.Cargo = query.Cargo;
//                model.Cash = query.Cash;
//                if (calculatorResults.Any()) {
//                    model.StationRoutes = calculatorResults.OrderByDescending(m => m.Total).Take(100).ToList();
//                }
                
//            }


//#if DEBUG
//                stopWatch.Stop();
//                // Get the elapsed time as a TimeSpan value.
//                TimeSpan ts = stopWatch.Elapsed;

//                // Format and display the TimeSpan value. 
//                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
//                    ts.Hours, ts.Minutes, ts.Seconds,
//                    ts.Milliseconds / 10);
//                Debug.WriteLine("RunTime " + elapsedTime);
//#endif


//            return model; 
//        }



        public async Task<CalculatorResultViewModel> Calculator(CalculatorQuery query) {
            CalculatorResultViewModel model = new CalculatorResultViewModel();

            if (query.StartSystem == "" && query.EndSystem == "")
                return model; // one system must be specified

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
            
            try {
                SqlCommand cmd = new SqlCommand();

                SqlParameter param;
                double version = double.Parse(ConfigurationManager.AppSettings["CurrentVersion"]);

                if (query.StartSystem != "" && query.EndSystem != ""){ // Both specified
                    if (query.StartStationId.HasValue && query.EndStationId.HasValue == false) {
                        cmd = new SqlCommand("CalcStationToSystem", con); // Need to write this one
                        cmd.CommandType = CommandType.StoredProcedure;

                        param = cmd.Parameters.Add("@StartStationId", SqlDbType.Int);
                        param.Value = query.StartStationId;

                        param = cmd.Parameters.Add("@EndSystemName", SqlDbType.NVarChar, 30);
                        param.Value = query.EndSystem;

                        param = cmd.Parameters.Add("@Cash", SqlDbType.Int);
                        param.Value = query.Cash;

                        param = cmd.Parameters.Add("@MinProfit", SqlDbType.Int);
                        param.Value = query.MinProfit;

                        param = cmd.Parameters.Add("@Cargo", SqlDbType.Int);
                        param.Value = query.Cargo;

                        param = cmd.Parameters.Add("@MaxDistanceFromJumpIn", SqlDbType.Float);
                        param.Value = query.MaxDistanceFromJumpIn;

                        //param = cmd.Parameters.Add("@PadSize", SqlDbType.NVarChar, 10);
                        //param.Value = query.PadSize;

                        param = cmd.Parameters.Add("@ExcludeOutposts", SqlDbType.NVarChar, 10);
                        param.Value = query.ExcludeOutposts;

                        param = cmd.Parameters.Add("@ExcludePlanets", SqlDbType.NVarChar, 10);
                        param.Value = query.ExcludePlanets;

                        param = cmd.Parameters.Add("@Version", SqlDbType.Float);
                        param.Value = version;

                    } else if (query.StartStationId.HasValue == false && query.EndStationId.HasValue == false) {
                        cmd = new SqlCommand("CalcSystemToSystem", con);
                        cmd.CommandType = CommandType.StoredProcedure;

                        param = cmd.Parameters.Add("@StartSystemName", SqlDbType.NVarChar, 30);
                        param.Value = query.StartSystem;

                        param = cmd.Parameters.Add("@EndSystemName", SqlDbType.NVarChar, 30);
                        param.Value = query.EndSystem;

                        param = cmd.Parameters.Add("@Cash", SqlDbType.Int);
                        param.Value = query.Cash;

                        param = cmd.Parameters.Add("@MinProfit", SqlDbType.Int);
                        param.Value = query.MinProfit;

                        param = cmd.Parameters.Add("@Cargo", SqlDbType.Int);
                        param.Value = query.Cargo;

                        param = cmd.Parameters.Add("@MaxDistanceFromJumpIn", SqlDbType.Float);
                        param.Value = query.MaxDistanceFromJumpIn;

                        //param = cmd.Parameters.Add("@PadSize", SqlDbType.NVarChar, 10);
                        //param.Value = query.PadSize;

                        param = cmd.Parameters.Add("@ExcludeOutposts", SqlDbType.NVarChar, 10);
                        param.Value = query.ExcludeOutposts;

                        param = cmd.Parameters.Add("@ExcludePlanets", SqlDbType.NVarChar, 10);
                        param.Value = query.ExcludePlanets;

                        param = cmd.Parameters.Add("@Version", SqlDbType.Float);
                        param.Value = version;

                    } else if (query.StartStationId.HasValue == false && query.EndStationId.HasValue){
                        cmd = new SqlCommand("CalcSystemToStation", con);
                        cmd.CommandType = CommandType.StoredProcedure;

                        param = cmd.Parameters.Add("@StartSystemName", SqlDbType.NVarChar, 30);
                        param.Value = query.StartSystem;

                        param = cmd.Parameters.Add("@EndStationId", SqlDbType.Int);
                        param.Value = query.EndStationId;

                        param = cmd.Parameters.Add("@Cash", SqlDbType.Int);
                        param.Value = query.Cash;

                        param = cmd.Parameters.Add("@MinProfit", SqlDbType.Int);
                        param.Value = query.MinProfit;

                        param = cmd.Parameters.Add("@Cargo", SqlDbType.Int);
                        param.Value = query.Cargo;

                        param = cmd.Parameters.Add("@MaxDistanceFromJumpIn", SqlDbType.Float);
                        param.Value = query.MaxDistanceFromJumpIn;

                        //param = cmd.Parameters.Add("@PadSize", SqlDbType.NVarChar, 10);
                        //param.Value = query.PadSize;

                        param = cmd.Parameters.Add("@ExcludeOutposts", SqlDbType.NVarChar, 10);
                        param.Value = query.ExcludeOutposts;

                        param = cmd.Parameters.Add("@ExcludePlanets", SqlDbType.NVarChar, 10);
                        param.Value = query.ExcludePlanets;

                        param = cmd.Parameters.Add("@Version", SqlDbType.Float);
                        param.Value = version;

                    } else{
                        cmd = new SqlCommand("CalcStationToStation", con);
                        cmd.CommandType = CommandType.StoredProcedure;

                        param = cmd.Parameters.Add("@StartStationId", SqlDbType.Int);
                        param.Value = query.StartStationId;

                        param = cmd.Parameters.Add("@EndStationId", SqlDbType.Int);
                        param.Value = query.EndStationId;

                        param = cmd.Parameters.Add("@Cash", SqlDbType.Int);
                        param.Value = query.Cash;

                        param = cmd.Parameters.Add("@MinProfit", SqlDbType.Int);
                        param.Value = query.MinProfit;

                        param = cmd.Parameters.Add("@Cargo", SqlDbType.Int);
                        param.Value = query.Cargo;

                        param = cmd.Parameters.Add("@Version", SqlDbType.Float);
                        param.Value = version;
                    }
		            
                } else if (query.StartSystem != "" && query.EndSystem == ""){ // Start system specified
	                if (query.StartStationId.HasValue){
                        cmd = new SqlCommand("CalcStationToAny", con);
                        cmd.CommandType = CommandType.StoredProcedure;

                        param = cmd.Parameters.Add("@StartStationId", SqlDbType.Int);
                        param.Value = query.StartStationId;

                        param = cmd.Parameters.Add("@Cash", SqlDbType.Int);
                        param.Value = query.Cash;

                        param = cmd.Parameters.Add("@MinProfit", SqlDbType.Int);
                        param.Value = query.MinProfit;

                        param = cmd.Parameters.Add("@Cargo", SqlDbType.Int);
                        param.Value = query.Cargo;

                        param = cmd.Parameters.Add("@SearchRange", SqlDbType.Float);
                        param.Value = query.SearchRange;

                        param = cmd.Parameters.Add("@MaxDistanceFromJumpIn", SqlDbType.Float);
                        param.Value = query.MaxDistanceFromJumpIn;

                        //param = cmd.Parameters.Add("@PadSize", SqlDbType.NVarChar, 10);
                        //param.Value = query.PadSize;

                        param = cmd.Parameters.Add("@ExcludeOutposts", SqlDbType.NVarChar, 10);
                        param.Value = query.ExcludeOutposts;

                        param = cmd.Parameters.Add("@ExcludePlanets", SqlDbType.NVarChar, 10);
                        param.Value = query.ExcludePlanets;

                        param = cmd.Parameters.Add("@Version", SqlDbType.Float);
                        param.Value = version;

	                }else{
                        cmd = new SqlCommand("CalcSystemToAny", con);
                        cmd.CommandType = CommandType.StoredProcedure;

                        param = cmd.Parameters.Add("@StartSystemName", SqlDbType.NVarChar, 30);
                        param.Value = query.StartSystem;

                        param = cmd.Parameters.Add("@Cash", SqlDbType.Int);
                        param.Value = query.Cash;

                        param = cmd.Parameters.Add("@MinProfit", SqlDbType.Int);
                        param.Value = query.MinProfit;

                        param = cmd.Parameters.Add("@Cargo", SqlDbType.Int);
                        param.Value = query.Cargo;

                        param = cmd.Parameters.Add("@SearchRange", SqlDbType.Float);
                        param.Value = query.SearchRange;

                        param = cmd.Parameters.Add("@MaxDistanceFromJumpIn", SqlDbType.Float);
                        param.Value = query.MaxDistanceFromJumpIn;

                        //param = cmd.Parameters.Add("@PadSize", SqlDbType.NVarChar, 10);
                        //param.Value = query.PadSize;

                        param = cmd.Parameters.Add("@ExcludeOutposts", SqlDbType.NVarChar, 10);
                        param.Value = query.ExcludeOutposts;

                        param = cmd.Parameters.Add("@ExcludePlanets", SqlDbType.NVarChar, 10);
                        param.Value = query.ExcludePlanets;

                        param = cmd.Parameters.Add("@Version", SqlDbType.Float);
                        param.Value = version;
                    }

                } else if (query.StartSystem == "" && query.EndSystem != "") { // End system specified
                    if (query.EndStationId.HasValue) {
                        cmd = new SqlCommand("CalcAnyToStation", con);  // Need to write this one
                        cmd.CommandType = CommandType.StoredProcedure;

                        param = cmd.Parameters.Add("@EndStationId", SqlDbType.Int);
                        param.Value = query.EndStationId;

                        param = cmd.Parameters.Add("@Cash", SqlDbType.Int);
                        param.Value = query.Cash;

                        param = cmd.Parameters.Add("@MinProfit", SqlDbType.Int);
                        param.Value = query.MinProfit;

                        param = cmd.Parameters.Add("@Cargo", SqlDbType.Int);
                        param.Value = query.Cargo;

                        param = cmd.Parameters.Add("@SearchRange", SqlDbType.Float);
                        param.Value = query.SearchRange;

                        param = cmd.Parameters.Add("@MaxDistanceFromJumpIn", SqlDbType.Float);
                        param.Value = query.MaxDistanceFromJumpIn;

                        //param = cmd.Parameters.Add("@PadSize", SqlDbType.NVarChar, 10);
                        //param.Value = query.PadSize;

                        param = cmd.Parameters.Add("@ExcludeOutposts", SqlDbType.NVarChar, 10);
                        param.Value = query.ExcludeOutposts;

                        param = cmd.Parameters.Add("@ExcludePlanets", SqlDbType.NVarChar, 10);
                        param.Value = query.ExcludePlanets;

                        param = cmd.Parameters.Add("@Version", SqlDbType.Float);
                        param.Value = version;

                    } else {
                        cmd = new SqlCommand("CalcAnyToSystem", con);  // Need to write this one
                        cmd.CommandType = CommandType.StoredProcedure;

                        param = cmd.Parameters.Add("@EndSystemName", SqlDbType.NVarChar, 30);
                        param.Value = query.EndSystem;

                        param = cmd.Parameters.Add("@Cash", SqlDbType.Int);
                        param.Value = query.Cash;

                        param = cmd.Parameters.Add("@MinProfit", SqlDbType.Int);
                        param.Value = query.MinProfit;

                        param = cmd.Parameters.Add("@Cargo", SqlDbType.Int);
                        param.Value = query.Cargo;

                        param = cmd.Parameters.Add("@SearchRange", SqlDbType.Float);
                        param.Value = query.SearchRange;

                        param = cmd.Parameters.Add("@MaxDistanceFromJumpIn", SqlDbType.Float);
                        param.Value = query.MaxDistanceFromJumpIn;

                        //param = cmd.Parameters.Add("@PadSize", SqlDbType.NVarChar, 10);
                        //param.Value = query.PadSize;

                        param = cmd.Parameters.Add("@ExcludeOutposts", SqlDbType.NVarChar, 10);
                        param.Value = query.ExcludeOutposts;

                        param = cmd.Parameters.Add("@ExcludePlanets", SqlDbType.NVarChar, 10);
                        param.Value = query.ExcludePlanets;

                        param = cmd.Parameters.Add("@Version", SqlDbType.Float);
                        param.Value = version;
                    }
                }


                // Execute the command.
                con.Open();
                SqlDataReader reader = await cmd.ExecuteReaderAsync();
                if (reader.HasRows) {

                    /* Second is the starting stations goods for sale */
                    while (reader.Read()) {
                        CalculatorResult item = new CalculatorResult();
                        item.CommodityName = reader["CommodityName"].ToString();
                        item.Buy = (int)reader["Buy"];
                        item.Supply = reader["Supply"].ToString();
                        item.SupplyAmount = (int)reader["SupplyAmount"];
                        item.Sell = (int)reader["Sell"];
                        item.Demand = reader["Demand"].ToString();
                        item.DemandAmount = (int)reader["DemandAmount"];
                        item.GalacticAveragePrice = (int)reader["GalacticAveragePrice"];
                        item.BuyLastUpdate = ((DateTime)reader["BuyLastUpdate"]).TimeSince();
                        item.BuyUpdatedBy = reader["BuyUpdatedBy"].ToString();
                        item.SellLastUpdate = ((DateTime)reader["SellLastUpdate"]).TimeSince();
                        item.SellUpdatedBy = reader["SellUpdatedBy"].ToString();
                        item.Profit = (int)reader["Profit"];
                        item.Source = reader["Source"].ToString();
                        item.SourceSystemName = reader["SourceSystemName"].ToString();
                        item.SourceStationId = (int)reader["SourceStationId"];
                        item.SourceStationTypeName = reader["SourceStationTypeName"].ToString();
                        item.SourceStationTypeIcon = reader["SourceStationTypeIcon"].ToString();
                        item.SourceStationAllegiance = reader["SourceStationAllegiance"].ToString();
                        item.SourceSystemId = (int)reader["SourceSystemId"];
                        item.SourceStationDistance = (double)reader["SourceStationDistance"];
                        item.Destination = reader["Destination"].ToString();
                        item.DestinationSystemName = reader["DestinationSystemName"].ToString();
                        item.DestinationStationId = (int)reader["DestinationStationId"];
                        item.DestinationStationTypeName = reader["DestinationStationTypeName"].ToString();
                        item.DestinationStationTypeIcon = reader["DestinationStationTypeIcon"].ToString();
                        item.DestinationStationAllegiance = reader["DestinationStationAllegiance"].ToString();
                        item.DestinationSystemId = (int)reader["DestinationSystemId"];
                        item.DestinationStationDistance = (double)reader["DestinationStationDistance"];
                        item.Distance = (double)reader["Distance"];
                        item.Qty = (int)reader["Qty"];
                        item.TotalProfit = (int)reader["TotalProfit"];
                        item.SourcePermitRequired = (bool)reader["SourcePermitRequired"];
                        item.DestinationPermitRequired = (bool)reader["DestinationPermitRequired"];
                        model.Results.Add(item);
                    }
                    
                }
                reader.Close();
            } catch (Exception ex) {
                Exception e = new Exception(
                    "StartSystem=" + query.StartSystem + 
                    " StartStationId=" + query.StartStationId +
                    " EndSystemName=" + query.EndSystem +
                    " EndStationId="+ query.EndStationId +
                    " Cash="+ query.Cash+
                    " SearchRange=" + query.SearchRange +
                    " Exclude Outposts=" + query.ExcludeOutposts +
                    " Exclude Planets=" + query.ExcludePlanets +
                    " MinProfit=" + query.MinProfit +
                    " Cargo="+ query.Cargo, ex);
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
            } finally {
                con.Close();
            }

            return model;

        }

//        public async Task<CalculatorByProfitViewModel> Calculator(CalculatorQuery query) {
//            using (ApplicationDbContext db = new ApplicationDbContext()) {
//#if DEBUG
//                Stopwatch stopWatch = new Stopwatch();
//                stopWatch.Start();
//#endif

//                // Get stations
//                Station currentStation = await db.Stations.AsNoTracking().Include(m => m.StationCommodities).Include(m => m.System).FirstOrDefaultAsync(s => s.Id == query.StartStationId);
//                Station destinationStation = await db.Stations.AsNoTracking().Include(m => m.System).FirstOrDefaultAsync(s => s.Id == query.EndStationId);

//                // Get a list of the commdity types
//                List<Commodity> commodities = await db.Commodities.AsNoTracking().ToListAsync();

//                // Build query for stations
//                var stationQuery = db.Stations
//                            .AsNoTracking()
//                            .Include(s => s.System)
//                            .Include(s => s.StationType)
//                            .Where(s => s.HasMarket);

//                if (destinationStation == null) {
//                    stationQuery = stationQuery.Where(s =>
//                                s.System.X < (currentStation.System.X + query.SearchRange) && s.System.X > (currentStation.System.X - query.SearchRange) &&
//                                s.System.Y < (currentStation.System.Y + query.SearchRange) && s.System.Y > (currentStation.System.Y - query.SearchRange) &&
//                                s.System.Z < (currentStation.System.Z + query.SearchRange) && s.System.Z > (currentStation.System.Z - query.SearchRange));
//                }

//                // Apply pad size filter
//                List<Station> stations = new List<Station>();
//                switch (query.PadSize) {
//                    case "Small":
//                        stations = await stationQuery
//                            .Where(s => s.StationType.PadSmall)
//                            .ToListAsync();
//                        break;
//                    case "Medium":
//                        stations = await stationQuery
//                            .Where(s => s.StationType.PadMedium)
//                            .ToListAsync();
//                        break;
//                    case "Large":
//                        stations = await stationQuery
//                            .Where(s => s.StationType.PadLarge)
//                            .ToListAsync();
//                        break;
//                }



//                // Build query for StationCommodities to buy, exclude any that have no sell price and exclude old data
//                double version = double.Parse(ConfigurationManager.AppSettings["CurrentVersion"]);
//                var stationCommodityQuery = db.StationCommodities
//                                .AsNoTracking()
//                                .Include(m => m.Station)
//                                .Include(sc => sc.Station.System)
//                                .Include(m => m.Commodity)
//                                .Where(m => m.Sell > 0 && m.Version == version);


//                // if destination is specified prune out all other stations than destination and current otherwise limit it to range
//                if (destinationStation != null) {
//                    stations = stations.Where(m => m.Id == currentStation.Id || m.Id == destinationStation.Id).ToList();
//                    stationCommodityQuery = stationCommodityQuery.Where(m => m.StationId == destinationStation.Id);
//                } else {
//                    stationCommodityQuery = stationCommodityQuery.Where(m =>
//                                                        m.Station.System.X < (currentStation.System.X + query.SearchRange) && m.Station.System.X > (currentStation.System.X - query.SearchRange) &&
//                                                        m.Station.System.Y < (currentStation.System.Y + query.SearchRange) && m.Station.System.Y > (currentStation.System.Y - query.SearchRange) &&
//                                                        m.Station.System.Z < (currentStation.System.Z + query.SearchRange) && m.Station.System.Z > (currentStation.System.Z - query.SearchRange));
//                }



//                List<StationCommodity> stationCommodities = await stationCommodityQuery.ToListAsync();



//                List<CalculatorResult> calculatorResults = new List<CalculatorResult>();
//                // get the list of purchaseable commodities at the station
//                if (currentStation.StationCommodities != null) {
//                    // Get the station commodities on sale for the current station exclude any more expensive than the current cash
//                    List<StationCommodity> currentStationsCommodities = currentStation.StationCommodities
//                        .Where(s => s.Buy > 1 && s.Buy < query.Cash)
//                        .ToList();



//                    // Loop through the stations other than the current station
//                    foreach (Station station in stations.Where(s => s.Id != currentStation.Id)) {
//                        var stationList = new List<CommodityRoute>();
//                        // Loop through the commodities available at the current station
//                        foreach (StationCommodity stationCommodity in currentStationsCommodities) {
//                            // Get stationcomodities that are in demand
//                            List<StationCommodity> scQuery = stationCommodities
//                                .Where(sc => sc.CommodityId == stationCommodity.CommodityId && sc.Sell > stationCommodity.Buy && sc.StationId == station.Id)
//                                //.OrderBy(sc => sc.Buy)
//                                .ToList();

//                            if (scQuery.Any()) {
//                                foreach (StationCommodity route in scQuery) {


//                                    // Add the result
//                                    var result = new CalculatorResult {
//                                        StartingStationId = currentStation.Id,
//                                        StartingStationName = currentStation.Name,
//                                        StartingSystemName = currentStation.System.Name,
//                                        DestinationStationId = station.Id,
//                                        DestinationStationName = station.Name,
//                                        DestinationSystemName = station.System.Name,
//                                        Distance = Math.Round(Math.Sqrt(Math.Pow(Math.Abs(station.System.X - currentStation.System.X), 2) + Math.Pow(Math.Abs(station.System.Y - currentStation.System.Y), 2) + Math.Pow(Math.Abs(station.System.Z - currentStation.System.Z), 2)), 2),
//                                        CommodityId = route.CommodityId,
//                                        CommodityName = route.Commodity.Name,
//                                        Buy = stationCommodity.Buy,
//                                        Sell = route.Sell,
//                                        GalacticAveragePrice = route.Commodity.GalacticAveragePrice,
//                                        Profit = route.Sell - stationCommodity.Buy,
//                                        LastUpdate = route.LastUpdate.TimeSince(),
//                                        UpdatedBy = route.UpdatedBy,
//                                        DistanceFromJumpIn = route.Station.DistanceFromJumpIn
//                                    };

//                                    // work out how many can be bought
//                                    int max = (int)Math.Floor((double)query.Cash / (double)result.Buy);

//                                    // work out if we can fit the max into the cargo space
//                                    result.Qty = max <= query.Cargo ? max : query.Cargo;
//                                    result.Total = result.Qty * result.Profit;

//                                    if (result.Total >= query.MinProfit) {
//                                        calculatorResults.Add(result);
//                                    }
//                                }
//                            }
//                        }
//                    }
//                }

//                CalculatorByProfitViewModel model = new CalculatorByProfitViewModel();
//                model.StartingSystemId = currentStation.System.Id;
//                model.StartingSystemName = currentStation.System.Name;
//                if (destinationStation != null)
//                    model.DestinationSystemName = destinationStation.System.Name;
//                model.Location = currentStation.System.Name + " (" + currentStation.Name + ")";
//                model.Cargo = query.Cargo;
//                model.Cash = query.Cash;
//                model.StationRoutes = calculatorResults.OrderByDescending(m => m.Total).Take(50).ToList();

//#if DEBUG
//                stopWatch.Stop();
//                // Get the elapsed time as a TimeSpan value.
//                TimeSpan ts = stopWatch.Elapsed;

//                // Format and display the TimeSpan value. 
//                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
//                    ts.Hours, ts.Minutes, ts.Seconds,
//                    ts.Milliseconds / 10);
//                Debug.WriteLine("RunTime " + elapsedTime);
//#endif


//                return model;
//            }
//        }


        public async Task<FindTradeResult> FindTrades(FindTradesQuery query, FindTradeResult results) {
            
#if DEBUG
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
#endif


            if (query.SystemName != null && query.SearchRange > 0 && query.SearchRange <= 60 && query.MaxDistanceFromJumpIn > 0 && query.MaxDistanceFromJumpIn <= 2000 && query.MaxDistanceBetweenSystems > 0) {

                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
                SqlCommand cmd;
            
                cmd = new SqlCommand("FindBiTrades", con);
 
                cmd.CommandType = CommandType.StoredProcedure;
                

                SqlParameter param;
                param = cmd.Parameters.Add("@SystemName", SqlDbType.NVarChar, 30);
                param.Value = query.SystemName;

                param = cmd.Parameters.Add("@SearchRange", SqlDbType.Int);
                param.Value = query.SearchRange;

                param = cmd.Parameters.Add("@MaxDistanceFromJumpIn", SqlDbType.Float);
                param.Value = query.MaxDistanceFromJumpIn;

                //param = cmd.Parameters.Add("@PadSize", SqlDbType.NVarChar, 10);
                //param.Value = query.PadSize;

                param = cmd.Parameters.Add("@ExcludeOutposts", SqlDbType.NVarChar, 10);
                param.Value = query.ExcludeOutposts;

                param = cmd.Parameters.Add("@ExcludePlanets", SqlDbType.NVarChar, 10);
                param.Value = query.ExcludePlanets;

                double version = double.Parse(ConfigurationManager.AppSettings["CurrentVersion"]);
                param = cmd.Parameters.Add("@Version", SqlDbType.Float);
                param.Value = version;



                try {
                    // Execute the command.
                    con.Open();
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    if (reader.HasRows) {
                        /* Loop through the results */
                        while (reader.Read()) {
                            FindBiTradeItem item = new FindBiTradeItem();
                            item.Distance = Math.Round(Math.Sqrt(Math.Pow(Math.Abs((double)reader["DestinationX"] - (double)reader["SourceX"]), 2) + Math.Pow(Math.Abs((double)reader["DestinationY"] - (double)reader["SourceY"]), 2) + Math.Pow(Math.Abs((double)reader["DestinationZ"] - (double)reader["SourceZ"]), 2)), 2);

                            if (item.Distance < query.MaxDistanceBetweenSystems) {

                                item.OutgoingCommodityName = reader["OutgoingCommodityName"].ToString();
                                item.ReturningCommodityName = reader["ReturningCommodityName"].ToString();
                                item.OutgoingBuy = (int)reader["OutgoingBuy"];
                                item.OutgoingSell = (int)reader["OutgoingSell"];

                                item.OutgoingBuyLastUpdate = DateTime.Parse(reader["OutgoingBuyLastUpdate"].ToString()).TimeSince();
                                item.OutgoingSellLastUpdate = DateTime.Parse(reader["OutgoingSellLastUpdate"].ToString()).TimeSince();

                                item.ReturningBuy = (int)reader["ReturningBuy"];
                                item.ReturningSell = (int)reader["ReturningSell"];

                                item.ReturningBuyLastUpdate = DateTime.Parse(reader["ReturningBuyLastUpdate"].ToString()).TimeSince();
                                item.ReturningSellLastUpdate = DateTime.Parse(reader["ReturningSellLastUpdate"].ToString()).TimeSince();

                                item.OutgoingProfit = (int)reader["OutgoingProfit"];
                                item.ReturningProfit = (int)reader["ReturningProfit"];
                                item.TotalProfit = (int)reader["TotalProfit"];

                                item.Source = reader["Source"].ToString();
                                item.SourceStationId = (long)reader["SourceStationId"];
                                item.SourceSystemId = (long)reader["SourceSystemId"];
                                item.SourceStationDistance = (double)reader["SourceStationDistance"];

                                item.Destination = reader["Destination"].ToString();
                                item.DestinationStationId = (long)reader["DestinationStationId"];
                                item.DestinationSystemId = (long)reader["DestinationSystemId"];
                                item.DestinationStationDistance = (double)reader["DestinationStationDistance"];

                                results.List.Add(item);
                            }
                        }

                        //if (query.BiDirectional) {
                            
                        //} else {
                        //    /* Loop through the results */
                        //    while (reader.Read()) {
                        //        FindTradeItem item = new FindTradeItem();
                        //        item.Distance = Math.Round(Math.Sqrt(Math.Pow(Math.Abs((double)reader["DestinationX"] - (double)reader["SourceX"]), 2) + Math.Pow(Math.Abs((double)reader["DestinationY"] - (double)reader["SourceY"]), 2) + Math.Pow(Math.Abs((double)reader["DestinationZ"] - (double)reader["SourceZ"]), 2)), 2);

                        //        if (item.Distance < query.MaxDistanceBetweenSystems) {

                        //            item.CommodityName = reader["CommodityName"].ToString();
                        //            item.Buy = (int)reader["Buy"];
                        //            item.Sell = (int)reader["Sell"];
                        //            item.BuyLastUpdate = DateTime.Parse(reader["BuyLastUpdate"].ToString()).TimeSince();
                        //            item.SellLastUpdate = DateTime.Parse(reader["SellLastUpdate"].ToString()).TimeSince();
                        //            item.Profit = (int)reader["Profit"];
                        //            item.Source = reader["Source"].ToString();
                        //            item.SourceStationId = (long)reader["SourceStationId"];
                        //            item.SourceSystemId = (long)reader["SourceSystemId"];
                        //            item.SourceStationDistance = (double)reader["SourceStationDistance"];
                        //            item.Destination = reader["Destination"].ToString();
                        //            item.DestinationStationId = (long)reader["DestinationStationId"];
                        //            item.DestinationSystemId = (long)reader["DestinationSystemId"];
                        //            item.DestinationStationDistance = (double)reader["DestinationStationDistance"];

                        //            results.UniList.Add(item);
                        //        }
                        //    }
                        //}
                        
                    }


                    reader.Close();
                } catch (Exception ex) {
                    Exception e = new Exception(
                                        "SystemName=" + query.SystemName +
                                        " SearchRange=" + query.SearchRange +
                                        " Exclude Outposts=" + query.ExcludeOutposts +
                                        " Exclude Planets=" + query.ExcludePlanets, ex);
                    Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                } finally {
                    con.Close();
                }


#if DEBUG
            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value. 
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Debug.WriteLine("RunTime " + elapsedTime);
#endif
            }
            return results;
        }


        public async Task<HighestTradeCommodity> FindHighestTradeCommodity(int sourceStationId, int destinationStationId) {

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
            SqlCommand cmd = new SqlCommand("FindHighestTradeCommodity", con);
            cmd.CommandType = CommandType.StoredProcedure;

            SqlParameter param;
            param = cmd.Parameters.Add("@StartStationId", SqlDbType.Int);
            param.Value = sourceStationId;

            param = cmd.Parameters.Add("@EndStationId", SqlDbType.Int);
            param.Value = destinationStationId;

            double version = double.Parse(ConfigurationManager.AppSettings["CurrentVersion"]);
            param = cmd.Parameters.Add("@Version", SqlDbType.Float);
            param.Value = version;

            HighestTradeCommodity item = new HighestTradeCommodity();

            try {
                // Execute the command.
                con.Open();
                SqlDataReader reader = await cmd.ExecuteReaderAsync();
                
                if (reader.HasRows) {
                    /* Loop through the results */
                    reader.Read();

                    item.CommodityName = reader["CommodityName"].ToString();
                    item.Buy = (int)reader["Buy"];
                    item.Sell = (int)reader["Sell"];
                    item.BuyLastUpdate = DateTime.Parse(reader["BuyLastUpdate"].ToString()).TimeSince();
                    item.SellLastUpdate = DateTime.Parse(reader["SellLastUpdate"].ToString()).TimeSince();
                    item.Profit = (int)reader["Profit"];
                    item.Source = reader["Source"].ToString();
                    item.SourceStationId = (int)reader["SourceStationId"];
                    item.SourceStationDistance = (double)reader["SourceStationDistance"];
                    item.Destination = reader["Destination"].ToString();
                    item.DestinationStationId = (int)reader["DestinationStationId"];
                    item.DestinationStationDistance = (double)reader["DestinationStationDistance"];

                    reader.Close();
                }
            } catch (Exception ex) {
                Exception e = new Exception(
                    "StartStationId=" + sourceStationId +
                    " EndStationId=" + destinationStationId, ex);
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
            } finally {
                con.Close();
            }
            return item;
        }


        public async Task<Dictionary<int, double>> GetDistances(int SystemId, double range) {
            using (ApplicationDbContext db = new ApplicationDbContext()) {
                // We'll make a Dictionary<int StartSystemId, Dictionary<int EndSystemId, double Distance>>
                Dictionary<int, double> distances = new Dictionary<int, double>();
                EliteTrading.Entities.System currentSystem = await db.Systems.AsNoTracking().FirstOrDefaultAsync(m => m.Id == SystemId);
                // Get the systems
                List<EliteTrading.Entities.System> systems = await db.Systems.AsNoTracking().Where(m =>
                                                                                    m.X < (currentSystem.X + range) && m.X > (currentSystem.X - range) &&
                                                                                    m.Y < (currentSystem.Y + range) && m.Y > (currentSystem.Y - range) &&
                                                                                    m.Z < (currentSystem.Z + range) && m.Z > (currentSystem.Z - range)
                                                                                    ).ToListAsync();


                // Calculate the connections
                foreach (EliteTrading.Entities.System endSystem in systems) {
                    double distance = Math.Round(Math.Sqrt(Math.Pow(Math.Abs(endSystem.X - currentSystem.X), 2) + Math.Pow(Math.Abs(endSystem.Y - currentSystem.Y), 2) + Math.Pow(Math.Abs(endSystem.Z - currentSystem.Z), 2)), 2);
                    distances.Add(endSystem.Id, distance);
                }


                return distances;
            }
        }
    }
}