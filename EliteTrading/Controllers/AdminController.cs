using System;
using System.Net;
using System.Web;
using System.Linq;
using System.Data;
using System.Web.Mvc;
using System.Data.Entity;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Collections.Generic;

using EliteTrading.Models;
using EliteTrading.Services;
using EliteTrading.Extensions;
using EliteTrading.Models.Admin;
using EliteTrading.Entities;
using EliteTrading.Models.ViewModels;


namespace EliteTrading.Controllers {
    [Authorize(Roles = "Mostly Harmless")]
    public class AdminController : Controller {
        private ApplicationDbContext db = new ApplicationDbContext();
        private static DefaultCacheProvider _cache = new DefaultCacheProvider();
        private ReputationService _rep = new ReputationService();

        // GET: Admin
        public ActionResult Index() {
            return View();
        }

        [Authorize(Roles = "Elite")]
        public ActionResult EDDN() {
            EDDNStatus status = new EDDNStatus();
            if (_cache.IsSet("EDDNStatus")) {
                status = (EDDNStatus)_cache.Get("EDDNStatus");
            }
            return View(status);
        }


        [Authorize(Roles = "Master")]
        public ActionResult MissingStations() {
            return View();
        }

        [HttpPost, Authorize(Roles = "Master")]
        public async Task<JsonResult> MissingStations(string currentLocation, int searchRange) {
            // Check the database
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
            SqlCommand cmd = new SqlCommand("EDDNMissingStations", con);
            cmd.CommandType = CommandType.StoredProcedure;

            SqlParameter param;
            param = cmd.Parameters.Add("@StartSystemName", SqlDbType.NVarChar, 50);
            param.Value = currentLocation;

            param = cmd.Parameters.Add("@SearchRange", SqlDbType.Int);
            param.Value = searchRange;

            List<MissingStationViewModel> list = new List<MissingStationViewModel>();

            try {
                // Execute the command.
                con.Open();
                SqlDataReader reader = await cmd.ExecuteReaderAsync();
                if (reader.HasRows) {
                    while (reader.Read()) {
                        MissingStationViewModel item = new MissingStationViewModel();
                        item.Distance = (double)reader["Distance"];
                        item.Message = reader["Message"].ToString();
                        list.Add(item);
                    }
                }
                reader.Close();
            } catch (Exception ex) {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            } finally {
                con.Close();
            }
            return Json(list);
        }

        public async Task<JsonResult> SelectorData(string query) {
            var systems = await db.Systems.AsNoTracking().Where(m => m.Name.StartsWith(query)).OrderBy(m => m.Name).ToListAsync();
            var stations = await db.Stations.AsNoTracking().Include(m => m.System).Where(m => m.Name.StartsWith(query)).OrderBy(m => m.Name).ToListAsync();
            // Get the stations
            var list = systems.Select(m => new AdminSelector {
                StationId = 0,
                Station = "",
                System = m.Name,
                SystemId = m.Id
            }).ToList();
            SortByLength(list);
            // Append the systems with no stations
            list.AddRange(stations.Select(m => new AdminSelector {
                StationId = m.Id,
                Station = m.Name,
                System = m.System.Name,
                SystemId = m.System.Id
            }).ToList());


            return Json(list, JsonRequestBehavior.AllowGet);
        }

        static IEnumerable<AdminSelector> SortByLength(IEnumerable<AdminSelector> e) {
            // Use LINQ to sort the array received and return a copy.
            var sorted = from s in e
                         orderby s.System.Length ascending
                         select s;
            return sorted;
        }




        public async Task<JsonResult> GetSelectLists() {
            AdminSelectLists model = new AdminSelectLists();
            if (_cache.IsSet("AdminSelectLists")) {
                model = (AdminSelectLists)_cache.Get("AdminSelectLists");
            } else {
                model.Allegiances = await db.Allegiances.AsNoTracking().OrderBy(m => m.Name).Select(m => new SelectListItem { Text = m.Name, Value = m.Id.ToString() }).ToListAsync();
                model.Economies = await db.Economy.AsNoTracking().OrderBy(m => m.Name).Select(m => new SelectListItem { Text = m.Name, Value = m.Id.ToString() }).ToListAsync();
                model.Governments = await db.Governments.AsNoTracking().OrderBy(m => m.Name).Select(m => new SelectListItem { Text = m.Name, Value = m.Id.ToString() }).ToListAsync();
                model.StationTypes = await db.StationTypes.AsNoTracking().OrderBy(m => m.Name).Select(m => new SelectListItem { Text = m.Name, Value = m.Id.ToString() }).ToListAsync();
                model.CommodityCategories = await db.Categories.AsNoTracking().OrderBy(m => m.Name).Select(m => new AdminCategoryCommodityList { Text = m.Name, Value = m.Id }).ToListAsync();
                foreach (AdminCategoryCommodityList cat in model.CommodityCategories) {
                    cat.Commodities = await db.Commodities.AsNoTracking().Where(m => m.CategoryId == cat.Value).OrderBy(m => m.Name).Select(m => new AdminCommodityList { Text = m.Name, Value = m.Id }).ToListAsync();
                }

                _cache.Set("AdminSelectLists", model, 3600);
            }

            // model.Systems = await db.Systems.OrderBy(m => m.Name).Select(m => new SelectListItem { Text = m.Name, Value = m.Id.ToString() }).ToListAsync();
            return Json(model, JsonRequestBehavior.AllowGet);
        }




        // Get the system data for a single system
        [NoCacheHeaderFilter]
        public async Task<JsonResult> System(int id) {
            var sys = await db.Systems.AsNoTracking().Include(m => m.Allegiance).Include("Stations.Economy").Include(m => m.Stations).Include(m => m.Government)
                                .Where(m => m.Id == id).ToListAsync();

            var result = sys.Select(m => new AdminSystem {
                Id = m.Id,
                Name = m.Name,
                AllegianceId = m.AllegianceId,
                AllegianceName = m.Allegiance.Name,
                Economy = m.Economy,
                GovernmentId = m.GovernmentId,
                GovernmentName = m.Government.Name,
                X = m.X,
                Y = m.Y,
                Z = m.Z,
                PermitRequired = m.PermitRequired
            }).FirstOrDefault();

            return Json(result, JsonRequestBehavior.AllowGet);
        }




        [Authorize(Roles = "Master")]
        [HttpPost]
        public async Task<ActionResult> System(AdminSystem model) {
            if (ModelState.IsValid) {

                var sys = await db.Systems.Include(m => m.Allegiance).Include(m => m.Stations).Include("Stations.Economy").Include(m => m.Government).Where(m => m.Name.ToLower() == model.Name.ToLower()).FirstOrDefaultAsync();

                if (sys != null) { // update
                    sys.Name = model.Name;
                    sys.AllegianceId = model.AllegianceId;
                    sys.GovernmentId = model.GovernmentId;
                    sys.Version = double.Parse(ConfigurationManager.AppSettings["CurrentVersion"]);
                    sys.X = model.X;
                    sys.Y = model.Y;
                    sys.Z = model.Z;
                    sys.PermitRequired = model.PermitRequired;
                    db.Entry(sys).State = EntityState.Modified;
                } else { // add
                    sys = new EliteTrading.Entities.System();
                    sys.Name = model.Name;
                    sys.AllegianceId = model.AllegianceId;
                    sys.GovernmentId = model.GovernmentId;
                    sys.X = model.X;
                    sys.Y = model.Y;
                    sys.Z = model.Z;
                    sys.PermitRequired = model.PermitRequired;
                    sys.Version = double.Parse(ConfigurationManager.AppSettings["CurrentVersion"]);
                    db.Systems.Add(sys);
                }
                try {
                    await db.SaveChangesAsync();
                    model.RepResult = await _rep.AddRepAsync(User.Identity.Name, ActionRep.AddEditSystem);
                } catch (Exception ex) {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                }
                return Json(model);
            }

            return new HttpStatusCodeResult(400, "Invalid parameters");
        }





        // Get all the stations in a system
        [NoCacheHeaderFilter]
        public async Task<JsonResult> SystemStations(int id) {
            AdminSystemSations model = new AdminSystemSations();
            model.SystemId = id;
            model.Stations = await db.Stations
                .AsNoTracking()
                .Include(m => m.StationType)
                .Include(m => m.Allegiance)
                .Include(m => m.Economy)
                .Include(m => m.Government)
                .Where(m => m.SystemId == id)
                .Select(m => new AdminStation {
                    Id = m.Id,
                    Name = m.Name,
                    SystemId = m.SystemId,
                    HasBlackmarket = m.HasBlackmarket,
                    HasMarket = m.HasMarket,
                    HasOutfitting = m.HasOutfitting,
                    HasShipyard = m.HasShipyard,
                    HasRepairs = m.HasRepairs,
                    HasRefuel = m.HasRefuel,
                    HasRearm = m.HasRearm,
                    FactionName = m.FactionName,
                    Allegiance = m.Allegiance.Name,
                    Economy = m.SecondaryEconomyId.HasValue ? m.Economy.Name + "/" + m.SecondaryEconomy.Name : m.Economy.Name,
                    Government = m.Government.Name,
                    DistanceFromJumpIn = m.DistanceFromJumpIn,
                    StationTypeId = m.StationTypeId,
                    StationTypeName = m.StationType.Name
                }).ToListAsync();

            return Json(model, JsonRequestBehavior.AllowGet);
        }




        // Get a single stations data
        [NoCacheHeaderFilter]
        public async Task<JsonResult> Station(int id) {
            var station = await db.Stations.AsNoTracking().Where(m => m.Id == id).Select(m => new AdminStation {
                Id = m.Id,
                Name = m.Name,
                SystemId = m.SystemId,
                HasBlackmarket = m.HasBlackmarket,
                HasMarket = m.HasMarket,
                HasOutfitting = m.HasOutfitting,
                HasShipyard = m.HasShipyard,
                HasRepairs = m.HasRepairs,
                HasRefuel = m.HasRefuel,
                HasRearm = m.HasRearm,
                FactionName = m.FactionName,
                AllegianceId = m.AllegianceId,
                EconomyId = m.EconomyId,
                SecondaryEconomyId = m.SecondaryEconomyId.HasValue ? m.SecondaryEconomyId.Value : 0,
                GovernmentId = m.GovernmentId,
                DistanceFromJumpIn = m.DistanceFromJumpIn,
                StationTypeId = m.StationTypeId,
                StationTypeName = m.StationType.Name,
            }).FirstOrDefaultAsync();

            return Json(station, JsonRequestBehavior.AllowGet);
        }



        [Authorize(Roles = "Master")]
        [HttpPost]
        public async Task<ActionResult> Station(AdminStation model) {
            if (ModelState.IsValid) {
                ResultMessage result = new ResultMessage();
                var username = await db.Users.Where(m => m.Email == User.Identity.Name).Select(m => m.CommanderName).FirstOrDefaultAsync();
                // Try to get the station from the database
                Station station = await db.Stations.Where(m => m.SystemId == model.SystemId && m.Id == model.Id).FirstOrDefaultAsync();
                double version = double.Parse(ConfigurationManager.AppSettings["CurrentVersion"]);
                if (station != null) { // Update
                    // If station no longer has a market delete the StationCommodities
                    if (station.HasMarket && model.HasMarket == false) {
                        List<StationCommodity> toRemove = await db.StationCommodities.Where(m => m.StationId == station.Id).ToListAsync();
                        db.StationCommodities.RemoveRange(toRemove);
                    }

                    // Set the values
                    station.Name = model.Name;
                    station.FactionName = model.FactionName;
                    station.HasBlackmarket = model.HasBlackmarket;
                    station.HasMarket = model.HasMarket;
                    station.HasOutfitting = model.HasOutfitting;
                    station.HasShipyard = model.HasShipyard;
                    station.HasRepairs = model.HasRepairs;
                    station.HasRefuel = model.HasRefuel;
                    station.HasRearm = model.HasRearm;
                    station.AllegianceId = model.AllegianceId;
                    station.EconomyId = model.EconomyId;
                    station.SecondaryEconomyId = model.SecondaryEconomyId > 0 ? model.SecondaryEconomyId : null;
                    station.GovernmentId = model.GovernmentId;
                    station.DistanceFromJumpIn = model.DistanceFromJumpIn;
                    station.StationTypeId = model.StationTypeId;
                    station.Version = version;
                    station.LastUpdateBy = username;
                    station.LastUpdateDate = DateTime.Now;
                    db.Entry(station).State = EntityState.Modified;
                    result.Message = "Station Updated";
                    var sys = await db.Systems.Where(m => m.Id == station.SystemId).FirstAsync();
                    if (sys.Version != version) {
                        sys.Version = version;
                        db.Entry(sys).State = EntityState.Modified;
                    }
                } else { // Station id not passed so try to add a station
                    // first check that someone isnt trying to add a station that already exists
                    station = await db.Stations.Where(m => m.SystemId == model.SystemId && m.Name.ToLower() == model.Name.ToLower()).FirstOrDefaultAsync();
                    if (station != null) { // Same name as an existing station so update
                        // If station no longer has a market delete the StationCommodities
                        if (station.HasMarket && model.HasMarket == false) {
                            List<StationCommodity> toRemove = await db.StationCommodities.Where(m => m.StationId == station.Id).ToListAsync();
                            db.StationCommodities.RemoveRange(toRemove);
                        }

                        // Set the values
                        station.Name = model.Name;
                        station.FactionName = model.FactionName;
                        station.HasBlackmarket = model.HasBlackmarket;
                        station.HasMarket = model.HasMarket;
                        station.HasOutfitting = model.HasOutfitting;
                        station.HasShipyard = model.HasShipyard;
                        station.HasRepairs = model.HasRepairs;
                        station.HasRefuel = model.HasRefuel;
                        station.HasRearm = model.HasRearm;
                        station.AllegianceId = model.AllegianceId;
                        station.EconomyId = model.EconomyId;
                        station.SecondaryEconomyId = model.SecondaryEconomyId > 0 ? model.SecondaryEconomyId : null;
                        station.GovernmentId = model.GovernmentId;
                        station.DistanceFromJumpIn = model.DistanceFromJumpIn;
                        station.StationTypeId = model.StationTypeId;
                        station.Version = version;
                        station.LastUpdateBy = username;
                        station.LastUpdateDate = DateTime.Now;
                        db.Entry(station).State = EntityState.Modified;
                        result.Message = "Station Updated";
                        var sys = await db.Systems.Where(m => m.Id == station.SystemId).FirstAsync();
                        if (sys.Version != version) {
                            sys.Version = version;
                            db.Entry(sys).State = EntityState.Modified;
                        }
                    } else { // Station ot found and no station with the same name exists so were adding a new one
                        // Set the values
                        station = new Station();
                        station.Name = model.Name;
                        station.FactionName = model.FactionName;
                        station.HasBlackmarket = model.HasBlackmarket;
                        station.SystemId = model.SystemId;
                        station.HasMarket = model.HasMarket;
                        station.HasOutfitting = model.HasOutfitting;
                        station.HasShipyard = model.HasShipyard;
                        station.HasRepairs = model.HasRepairs;
                        station.HasRefuel = model.HasRefuel;
                        station.HasRearm = model.HasRearm;
                        station.AllegianceId = model.AllegianceId;
                        station.EconomyId = model.EconomyId;
                        station.SecondaryEconomyId = model.SecondaryEconomyId > 0 ? model.SecondaryEconomyId : null;
                        station.GovernmentId = model.GovernmentId;
                        station.DistanceFromJumpIn = model.DistanceFromJumpIn;
                        station.Version = version;
                        station.StationTypeId = model.StationTypeId;
                        station.LastUpdateBy = username;
                        station.LastUpdateDate = DateTime.Now;
                        db.Stations.Add(station);
                        result.Message = "Station Added";
                        var sys = await db.Systems.Where(m => m.Id == station.SystemId).FirstAsync();
                        if (sys.Version != version) {
                            sys.Version = version;
                            db.Entry(sys).State = EntityState.Modified;
                        }

                        // Check EDDN Logs for any entries and remove them
                        List<EDDNLog> logEntries = await db.EDDNLogs.Where(m => m.Message == sys.Name + " (" + model.Name + ")").ToListAsync();
                        db.EDDNLogs.RemoveRange(logEntries);
                    }
                }

                // Try to save the data
                try {
                    await db.SaveChangesAsync();


                    if (model.CopyMarketFromStationId != 0) {
                        // station.Id
                        var commodities = await db.StationCommodities.Where(m => m.StationId == model.CopyMarketFromStationId).ToListAsync();
                        foreach (StationCommodity sc in commodities) {
                            db.StationCommodities.Add(new StationCommodity {
                                StationId = station.Id,
                                CommodityId = sc.CommodityId,
                                Buy = sc.Buy,
                                Sell = sc.Sell,
                                LastUpdate = sc.LastUpdate,
                                UpdatedBy = username,
                                Supply = sc.Supply,
                                SupplyAmount = sc.SupplyAmount,
                                Demand = sc.Demand,
                                DemandAmount = sc.DemandAmount,
                                Version = double.Parse(ConfigurationManager.AppSettings["CurrentVersion"])
                            });
                        }
                        await db.SaveChangesAsync();
                    }

                    // Apply rep to user
                    result.RepResult = await _rep.AddRepAsync(User.Identity.Name, ActionRep.AddEditStation);
                    result.Result = true;
                } catch (Exception ex) {
                    // Blegh something went wrong
                    result.Result = false;
                    result.Message = "Error adding/updating station";
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                };

                return Json(result);
            }
            // Invalid params
            return new HttpStatusCodeResult(400, "Invalid parameters");
        }




        [HttpPost, Authorize(Roles = "Elite")]
        public async Task<ActionResult> DeleteStation(int Id) {
            if (ModelState.IsValid) {
                var station = await db.Stations.Where(m => m.Id == Id).FirstOrDefaultAsync();
                ResultMessage result = new ResultMessage();
                if (station != null) {
                    db.Stations.Remove(station);
                    try {
                        await db.SaveChangesAsync();
                        result.Result = true;
                        result.Message = "Station deleted";
                    } catch (Exception ex) {
                        result.Result = false;
                        result.Message = "Failed to delete station";
                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    }

                } else {
                    result.Result = false;
                    result.Message = "Station not found";
                }

                return Json(result);
            }
            // Invalid params
            return new HttpStatusCodeResult(400, "Invalid parameters");
        }




        // Get commodities for a station
        [NoCacheHeaderFilter]
        public async Task<JsonResult> StationCommodities(int id, int categoryId, RepResult repResult = null) {
            return Json(await ListStationCommodities(id, categoryId, repResult), JsonRequestBehavior.AllowGet);
        }

        [NoCacheHeaderFilter]
        public async Task<AdminStationCommodities> ListStationCommodities(int id, int categoryId, RepResult repResult = null) {
            var query = db.StationCommodities
                .AsNoTracking()
                .Include(s => s.Commodity)
                .Include(s => s.Commodity.Category)
                .Include(s => s.Station).Where(c => c.StationId == id); ;

            if (categoryId != 0) {
                query = query.Where(c => c.Commodity.CategoryId == categoryId);
            }

            var list = query.OrderBy(s => s.Station.Name)
                .ThenBy(c => c.Commodity.Category.Name)
                .ThenBy(c => c.Commodity.Name);

            List<StationCommodityViewModel> stationCommodities = await list.Select(m => new StationCommodityViewModel {
                Id = m.Id,
                StationId = m.StationId,
                CommodityId = m.CommodityId,
                Buy = m.Buy,
                Sell = m.Sell,
                LastUpdate = m.LastUpdate,
                UpdatedBy = m.UpdatedBy,
                CommodityName = m.Commodity.Name,
            }).ToListAsync();

            foreach (StationCommodityViewModel item in stationCommodities) {
                item.TimeSince = item.LastUpdate.TimeSince();
            }
            AdminStationCommodities model = new AdminStationCommodities();
            model.StationCommodities = stationCommodities;
            model.StationName = await db.Stations.AsNoTracking().Where(m => m.Id == id).Select(m => m.System.Name + " (" + m.Name + ")").FirstOrDefaultAsync();
            model.StationId = id;
            model.RepResult = repResult;
            return model;
        }



        // Get single station commodity
        [NoCacheHeaderFilter]
        public async Task<JsonResult> StationCommodity(int id) {
            var model = await db.StationCommodities
                .AsNoTracking()
                .Include(s => s.Commodity)
                .Include(s => s.Station).Where(c => c.Id == id)
                .Select(m => new StationCommodityViewModel {
                    Id = m.Id,
                    StationId = m.StationId,
                    CommodityId = m.CommodityId,
                    Buy = m.Buy,
                    Sell = m.Sell,
                    LastUpdate = m.LastUpdate,
                    UpdatedBy = m.UpdatedBy,
                    CommodityName = m.Commodity.Name,
                }).FirstOrDefaultAsync();


            return Json(model, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        [Authorize(Roles = "Competent")]
        public async Task<ActionResult> AddAllCommoditiesToCategory(int id, int categoryId) {
            if (id == 0) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RepResult repResult = new RepResult();
            try {
                List<StationCommodity> existingCommodities = new List<StationCommodity>();
                List<Commodity> allCommodities = new List<Commodity>();
                if (categoryId > 0) {
                    existingCommodities = await db.StationCommodities.Include(m => m.Commodity).Where(x => x.StationId == id && x.Commodity.CategoryId == categoryId).ToListAsync();
                    allCommodities = await db.Commodities.Where(x => x.CategoryId == categoryId).ToListAsync();
                } else {
                    existingCommodities = await db.StationCommodities.Where(x => x.StationId == id).ToListAsync();
                    allCommodities = await db.Commodities.ToListAsync();
                }

                var username = await db.Users.Where(m => m.Email == User.Identity.Name).Select(m => m.CommanderName).FirstOrDefaultAsync();

                // Loop through all the possible commodities
                foreach (Commodity item in allCommodities) {
                    // Check if the station has it
                    if (!existingCommodities.Where(m => m.CommodityId == item.Id).Any()) {
                        StationCommodity stationCommodity = new StationCommodity {
                            StationId = id,
                            CommodityId = item.Id,
                            Buy = 0,
                            Sell = 0,
                            LastUpdate = DateTime.Now,
                            UpdatedBy = username,
                            Version = double.Parse(ConfigurationManager.AppSettings["CurrentVersion"])
                        };

                        db.StationCommodities.Add(stationCommodity);

                    }
                }

                await db.SaveChangesAsync();
                repResult = await _rep.AddRepAsync(User.Identity.Name, ActionRep.PriceCheck);
            } catch (Exception ex) {
                Exception e = new Exception(
                    "StationId=" + id +
                    " CategoryId=" + categoryId, ex);
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
            }
            return Json(await ListStationCommodities(id, categoryId, repResult), JsonRequestBehavior.AllowGet);
        }



        [NoCacheHeaderFilter]
        public async Task<ActionResult> AddEditStationCommodity(AdminStationCommodity model) {
            if (ModelState.IsValid) {
                var username = await db.Users.Where(m => m.Email == User.Identity.Name).Select(m => m.CommanderName).FirstOrDefaultAsync();

                Commodity commodity = await db.Commodities.FirstOrDefaultAsync(m => m.Id == model.CommodityId);
                if (model.Sell != 0) {
                    if (model.Sell > commodity.Max || model.Sell < commodity.Min) {
                        return new HttpStatusCodeResult(400, "Value out of range: " + commodity.Min + "-" + commodity.Max);
                    }
                }

                if (model.Buy != 0) {
                    if (model.Buy > commodity.Max || model.Buy < commodity.Min) {
                        return new HttpStatusCodeResult(400, "Value out of range: " + commodity.Min + "-" + commodity.Max);
                    }
                }

                StationCommodity sc = new StationCommodity();
                if (model.Id == 0) {
                    sc.StationId = model.StationId;
                    sc.CommodityId = model.CommodityId;
                    sc.Buy = model.Buy;
                    sc.Sell = model.Sell;
                    sc.LastUpdate = DateTime.Now;
                    sc.UpdatedBy = username;
                    sc.Version = double.Parse(ConfigurationManager.AppSettings["CurrentVersion"]);
                    db.StationCommodities.Add(sc);
                } else {
                    sc = await db.StationCommodities.Where(m => m.Id == model.Id).FirstOrDefaultAsync();
                    sc.Buy = model.Buy;
                    sc.Sell = model.Sell;
                    sc.LastUpdate = DateTime.Now;
                    sc.UpdatedBy = username;
                    sc.Version = double.Parse(ConfigurationManager.AppSettings["CurrentVersion"]);
                    db.Entry(sc).State = EntityState.Modified;
                }

                RepResult repResult = new RepResult();
                // Try to save the data
                try {
                    await db.SaveChangesAsync();

                    // Apply rep to user
                    repResult = await _rep.AddRepAsync(User.Identity.Name, ActionRep.AddedStationCommodity);
                } catch (Exception ex) {
                    // Blegh something went wrong
                    Exception e = new Exception(
                        "Id=" + model.Id +
                        " StationId=" + model.StationId +
                        " CategoryId=" + model.CategoryId +
                        " CommodityId=" + model.CommodityId +
                        " Buy=" + model.Buy +
                        " Sell=" + model.Sell, ex);
                    Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                };
                return Json(await ListStationCommodities(model.StationId, model.CategoryId, repResult), JsonRequestBehavior.AllowGet);
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }




        // POST: StationCommodities/UpdateCommodity
        [HttpPost]
        public async Task<ActionResult> UpdateCommodity([Bind(Include = "StationCommodityId,Action,Value")] int StationCommodityId, string Action, int Value) {
            if (ModelState.IsValid) {
                ApplicationUser user = await db.Users.FirstOrDefaultAsync(m => m.UserName == User.Identity.Name);
                StationCommodity stationCommodity = await db.StationCommodities.Include(m => m.Commodity).FirstOrDefaultAsync(m => m.Id == StationCommodityId);
                if (stationCommodity != null) {
                    if (Value != 0) {
                        if (Value > stationCommodity.Commodity.Max || Value < stationCommodity.Commodity.Min) {
                            return new HttpStatusCodeResult(400, "Value out of range: " + stationCommodity.Commodity.Min + "-" + stationCommodity.Commodity.Max);
                        }
                    }

                    if (Action == "buy") {
                        stationCommodity.Buy = Value;
                    } else {
                        stationCommodity.Sell = Value;
                    }
                    stationCommodity.LastUpdate = DateTime.Now;
                    stationCommodity.UpdatedBy = user.CommanderName;
                    stationCommodity.Version = double.Parse(ConfigurationManager.AppSettings["CurrentVersion"]);
                    user.Updates += 1;
                    db.Entry(stationCommodity).State = EntityState.Modified;
                    db.Entry(user).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    RepResult repResult = await _rep.AddRepAsync(User.Identity.Name, ActionRep.CommodityChange);
                    return Json(new AdminStationCommodityChangeResult {
                        StationCommodityId = StationCommodityId,
                        Action = Action,
                        Value = Value,
                        LastUpdate = stationCommodity.LastUpdate.TimeSince(),
                        UpdatedBy = stationCommodity.UpdatedBy,
                        RepResult = repResult
                    });
                }
                return new HttpStatusCodeResult(400, "Commodity not found.");
            }

            return new HttpStatusCodeResult(400, "Invalid parameters");
        }



        [Authorize(Roles = "Expert")]
        [HttpPost]
        public async Task<ActionResult> DeleteStationCommodity(int id, int categoryId) {
            try {
                StationCommodity stationCommodity = await db.StationCommodities.Include(c => c.Commodity).FirstOrDefaultAsync(c => c.Id == id);

                if (stationCommodity != null) {
                    int stationId = stationCommodity.StationId;
                    db.StationCommodities.Remove(stationCommodity);

                    await db.SaveChangesAsync();
                    return Json(await ListStationCommodities(stationId, categoryId), JsonRequestBehavior.AllowGet);
                } else {
                    return new HttpStatusCodeResult(400, "Commodity not found.");
                }
            } catch (Exception ex) {
                Exception e = new Exception(
                    "id=" + id +
                    " categoryId=" + categoryId, ex);
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return new HttpStatusCodeResult(500, "Error deleting commodity.");
            }


        }




        [HttpPost]
        [Authorize(Roles = "Mostly Harmless")]
        public async Task<ActionResult> ConfirmStationCommodity(int id, int categoryId) {
            StationCommodity stationCommodity = await db.StationCommodities.Include(c => c.Commodity).FirstOrDefaultAsync(c => c.Id == id);
            RepResult repResult = new RepResult();
            if (stationCommodity != null) {
                int stationId = stationCommodity.StationId;
                var username = await db.Users.Where(m => m.Email == User.Identity.Name).Select(m => m.CommanderName).FirstOrDefaultAsync();

                stationCommodity.LastUpdate = DateTime.Now;
                stationCommodity.UpdatedBy = username;
                stationCommodity.Version = double.Parse(ConfigurationManager.AppSettings["CurrentVersion"]);
                db.Entry(stationCommodity).State = EntityState.Modified;

                await db.SaveChangesAsync();
                repResult = await _rep.AddRepAsync(User.Identity.Name, ActionRep.PriceCheck);
                return Json(await ListStationCommodities(stationId, categoryId, repResult), JsonRequestBehavior.AllowGet);
            }
            return new HttpStatusCodeResult(400, "Commodity not found.");
        }



        public async Task<ActionResult> Commodities() {
            List<AdminCommoditySearchListItem> list = await db.Commodities
                                                            .AsNoTracking()
                                                            .Include(m => m.Category)
                                                            .OrderBy(m => m.Category.Name)
                                                            .Select(m => new AdminCommoditySearchListItem {
                                                                Id = m.Id,
                                                                Name = m.Name,
                                                                CategoryId = m.CategoryId,
                                                                CategoryName = m.Category.Name
                                                            }).ToListAsync();
            return Json(list, JsonRequestBehavior.AllowGet);
        }


        [Authorize(Roles = "Competent")]
        public ActionResult Import() {
            return View();
        }


        [HttpPost]
        [Authorize(Roles = "Competent")]
        public async Task<ActionResult> CheckStationNames(List<AdminImportStation> stations) {
            if (ModelState.IsValid) {
                foreach (AdminImportStation station in stations) {
                    Station result = await db.Stations.AsNoTracking().Include(m => m.System).Where(m => m.Name.ToLower() == station.StationName.ToLower() && m.System.Name.ToLower() == station.SystemName.ToLower()).FirstOrDefaultAsync();
                    if (result != null) {
                        station.StationId = result.Id;
                    }
                }
                return Json(stations);
            }
            return new HttpStatusCodeResult(400, "Invalid parameters");
        }

        [HttpPost]
        [Authorize(Roles = "Competent")]
        public async Task<ActionResult> ImportCommodities(List<AdminImportData> data) {
            if (ModelState.IsValid) {
                try {
                    AdminImportResult result = new AdminImportResult();

                    // Get the user and the current version
                    var user = await db.Users.Where(m => m.Email == User.Identity.Name).FirstOrDefaultAsync();
                    double version = double.Parse(ConfigurationManager.AppSettings["CurrentVersion"]);

                    // Loop through the posted data
                    foreach (AdminImportData stationImport in data) {
                        // Get the station details
                        Station station = await db.Stations.Where(m => m.Id == stationImport.Id).FirstOrDefaultAsync();

                        // Check if the station exists
                        if (station != null) {

                            // If the station hasn't been previously marked as having a market, add one
                            if (!station.HasMarket) {
                                station.HasMarket = true;
                                db.Entry(station).State = EntityState.Modified;
                            }

                            // Get the commodities
                            List<Commodity> commodities = await db.Commodities.ToListAsync();

                            // Get the stations existing commodities
                            List<StationCommodity> list = await db.StationCommodities.Where(m => m.StationId == station.Id).ToListAsync();
                            foreach (AdminImportStationCommodity sc in stationImport.Commodities) {
                                if (sc.Supply == null && sc.Demand == null) {
                                    // Cargo in the users hold
                                    result.Skipped += 1;
                                    continue;
                                }

                                Commodity commodity = commodities.Where(m => m.Id == sc.CommodityId).FirstOrDefault();

                                // Check if the submitted data is within the acceptable range
                                if ((sc.Buy != 0 && (sc.Buy > commodity.Max || sc.Buy < commodity.Min)) || sc.Sell > commodity.Max || sc.Sell < commodity.Min) {
                                    // It is so add it to the OutOfRange collection
                                    result.OutOfRange.Add(new AdminImportStationOutOfRangeCommodity {
                                        Station = station.Name,
                                        Commodity = commodity.Name,
                                        Buy = sc.Buy,
                                        Sell = sc.Sell,
                                        Range = commodity.Min + " - " + commodity.Max
                                    });
                                    result.Skipped += 1;
                                    continue;
                                }

                                // Try to find the StationCommodity in the list of commodities for this station
                                StationCommodity item = list.Where(m => m.CommodityId == sc.CommodityId).FirstOrDefault();

                                // Check if it exists
                                if (item != null) { // Update
                                    // See if the existing entry is newer
                                    if (item.LastUpdate < sc.Date) {
                                        // Its older so lets update it
                                        item.LastUpdate = sc.Date;
                                        item.Buy = sc.Buy;
                                        item.Sell = sc.Sell;
                                        item.UpdatedBy = user.CommanderName;
                                        item.Version = version;
                                        item.Supply = sc.Supply;
                                        item.SupplyAmount = sc.SupplyAmount.HasValue ? sc.SupplyAmount.Value : 0;
                                        item.Demand = sc.Demand;
                                        item.DemandAmount = sc.DemandAmount.HasValue ? sc.DemandAmount.Value : 0;
                                        db.Entry(item).State = EntityState.Modified;
                                        result.Updated += 1;
                                    } else {
                                        // Existing StationCommodity is newer so skip this one
                                        result.Skipped += 1;
                                    }
                                } else { // Create
                                    item = new StationCommodity();
                                    item.StationId = station.Id;
                                    item.CommodityId = sc.CommodityId;
                                    item.LastUpdate = sc.Date;
                                    item.Buy = sc.Buy;
                                    item.Sell = sc.Sell;
                                    item.UpdatedBy = user.CommanderName;
                                    item.Version = version;
                                    item.Supply = sc.Supply;
                                    item.SupplyAmount = sc.SupplyAmount.HasValue ? sc.SupplyAmount.Value : 0;
                                    item.Demand = sc.Demand;
                                    item.DemandAmount = sc.DemandAmount.HasValue ? sc.DemandAmount.Value : 0;
                                    db.StationCommodities.Add(item);
                                    result.Added += 1;
                                }
                            }
                            // delete any that weren't updated
                            var toDelete = list.Where(m => m.UpdatedBy != user.CommanderName).ToList();
                            result.Deleted = toDelete.Count();
                            db.StationCommodities.RemoveRange(toDelete);

                            // Update the users counts
                            user.Updates += (result.Added + result.Deleted + result.Updated);
                            db.Entry(user).State = EntityState.Modified;

                            await db.SaveChangesAsync();
                            if (result.Updated > 0 || result.Added > 0) {
                                // Apply rep to user if useful changes were made
                                result.RepResult = await _rep.AddRepAsync(User.Identity.Name, ActionRep.ImportData);
                            }

                        }
                    }
                    return Json(result);
                } catch (Exception ex) {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    return new HttpStatusCodeResult(500, ex.Message);
                }
            }
            var allErrors = ModelState.Values.SelectMany(v => v.Errors);

            var errorList = ModelState.Values.SelectMany(m => m.Errors)
                                             .Select(e => e.ErrorMessage)
                                             .ToList();
            return new HttpStatusCodeResult(400, string.Join(",", errorList));
        }


        [HttpPost]
        [Authorize(Roles = "Novice")]
        public async Task<ActionResult> ConfirmManyStationCommodities(int id, int categoryId) {
            StationCommodity stationCommodity = await db.StationCommodities.FirstOrDefaultAsync(c => c.StationId == id);
            int stationId = stationCommodity.StationId;
            var username = await db.Users.Where(m => m.Email == User.Identity.Name).Select(m => m.CommanderName).FirstOrDefaultAsync();

            List<StationCommodity> stationCommodities = new List<StationCommodity>();
            if (categoryId > 0) {
                stationCommodities = await db.StationCommodities.Where(x => x.StationId == id && x.Commodity.CategoryId == categoryId).ToListAsync();
            } else {
                stationCommodities = await db.StationCommodities.Where(x => x.StationId == id).ToListAsync();
            }
            RepResult repResult = new RepResult();
            // Loop through the selected station commodities
            foreach (StationCommodity item in stationCommodities) {
                item.LastUpdate = DateTime.Now;
                item.UpdatedBy = username;
                item.Version = double.Parse(ConfigurationManager.AppSettings["CurrentVersion"]);
                db.Entry(item).State = EntityState.Modified;
            }
            repResult = await _rep.AddRepAsync(User.Identity.Name, ActionRep.PriceCheck);
            await db.SaveChangesAsync();
            return Json(await ListStationCommodities(id, categoryId, repResult), JsonRequestBehavior.AllowGet);
        }
    }
}

