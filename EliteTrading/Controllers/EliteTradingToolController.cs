using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Data.Entity;
using EliteTrading.Entities;
using EliteTrading.Models.ViewModels;
using EliteTrading.Services;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using EliteTrading.Extensions;
using EliteTrading.Models;
using System.Globalization;
using System.Configuration;
using Newtonsoft.Json.Linq;
using EliteTrading.Models.Search;
using EliteTrading.Models.EDSC;

namespace EliteTrading.Controllers {

    public class EliteTradingToolController : ApiController {

        public async Task<IHttpActionResult> GetStarData(string query) {
            using (ApplicationDbContext db = new ApplicationDbContext()) {
                // First check if we need to update the database of stars
                EliteTrading.Entities.Status status = await db.Status.FirstAsync();

                // Has an hour passed since the last EDSC check
                //if (status.LastStarCheck.AddHours(1) < DateTime.Now) {
                //    double version = double.Parse(ConfigurationManager.AppSettings["CurrentVersion"]);
                //    var all = await db.Allegiances.Where(m => m.Name == "None").FirstOrDefaultAsync();
                //    var gov = await db.Governments.Where(m => m.Name == "None").FirstOrDefaultAsync();

                //    try {
                //        using (WebClient webClient = new WebClient()) {

                //            webClient.Headers[HttpRequestHeader.ContentType] = "application/json";

                //            byte[] data = Encoding.Default.GetBytes("{ data: { ver: 2, outputmode:2, filter:{knownstatus:0,cr:2,date:'" + status.LastStarCheck.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) + "'}}}");
                //            byte[] result = webClient.UploadData("http://edstarcoordinator.com/api.asmx/GetSystems", "POST", data);

                //            dynamic x = Newtonsoft.Json.JsonConvert.DeserializeObject(Encoding.Default.GetString(result));
                //            var systems = x.d.systems;
                //            foreach (var system in systems) {
                //                string sysName = (string)system.name;
                //                EliteTrading.Entities.System sys = await db.Systems.Where(m => m.Name.ToLower() == sysName.ToLower()).FirstOrDefaultAsync();

                //                if (sys == null) {
                //                    // if not found add the system, use none/none/none for the unknown fields
                //                    sys = new EliteTrading.Entities.System();
                //                    sys.X = system.coord[0];
                //                    sys.Y = system.coord[1];
                //                    sys.Z = system.coord[2];
                //                    sys.Version = version;
                //                    sys.Name = system.name;
                //                    sys.AllegianceId = all.Id;
                //                    sys.GovernmentId = gov.Id;
                //                    sys.LastUpdateBy = "";
                //                    sys.LastUpdateDate = DateTime.Now;
                //                    sys.DevData = false;
                //                    sys.PermitRequired = false;
                //                    db.Systems.Add(sys);
                //                }
                //            }

                //            await db.SaveChangesAsync();

                //        }
                //    } catch (Exception ex) {
           
                //            Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                        
                //    }


                //    // Save the changes
                //    status.LastStarCheck = DateTime.Now;
                //    db.Entry(status).State = EntityState.Modified;
                //    await db.SaveChangesAsync();
                //}

                // Get the matching star names
                var list = await db.Systems.Where(m => m.Name.StartsWith(query))
                                            .OrderBy(m => m.Name)
                                            .Select(m => new EliteTrading.Models.API.StarData { Id = m.Id, Name = m.Name }).ToListAsync();

                // Return the list
                return Ok(list);
            }
        }

        public async Task<IHttpActionResult> GetCommodities() {
            DefaultCacheProvider _cache = new DefaultCacheProvider();
            using (ApplicationDbContext db = new ApplicationDbContext()) {
                List<CommoditiesListItem> model = new List<CommoditiesListItem>();
                if (_cache.IsSet("CommoditiesList")) {
                    model = (List<CommoditiesListItem>)_cache.Get("CommoditiesList");
                } else {
                    model = await db.Commodities.AsNoTracking().OrderBy(m => m.Name).Select(m => new CommoditiesListItem {
                        Id = m.Id,
                        Name = m.Name
                    }).ToListAsync();
                    _cache.Set("CommoditiesList", model, 3600);
                }
                return Ok(model);
            }
        }

        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> Calculator(CalculatorQuery calculatorQuery) {
            if (ModelState.IsValid) {
                QueryService _query = new QueryService();
                CalculatorResultViewModel model = await _query.Calculator(calculatorQuery);
                if (User.Identity.IsAuthenticated) {
                    ReputationService _rep = new ReputationService();
                    model.RepResult = await _rep.AddRepAsync(User.Identity.Name, ActionRep.Query);
                    using (ApplicationDbContext db = new ApplicationDbContext()) {
                        ApplicationUser user = await db.Users.FirstAsync(m => m.UserName == User.Identity.Name);
                        user.Queries += 1;
                        db.Entry(user).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                    }
                }
                
                return Ok(model);
            }
            var error = new HttpError("Invalid parameters");
            return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, error));
        }




        [HttpGet]
        public async Task<IHttpActionResult> SystemNames() {
            using (ApplicationDbContext db = new ApplicationDbContext()) {
                var output = await db.Systems.AsNoTracking().OrderBy(m => m.Name).Select(m => m.Name).ToListAsync();
                return Ok(output);
            }
        }
        




        [HttpGet]
        public async Task<IHttpActionResult> SystemNamesWithId() {
            using (ApplicationDbContext db = new ApplicationDbContext()) {
                var list = await db.Systems.AsNoTracking().OrderBy(m => m.Name).ToListAsync();
                var output = list.Select(x => new KeyValuePair<int, string>(x.Id, x.Name)).ToList();
                return Ok(output);
            }
        }





        [HttpGet]
        public async Task<IHttpActionResult> Stations(bool marketsOnly = false) {
            using (ApplicationDbContext db = new ApplicationDbContext()) {
                var list = await db.Stations.AsNoTracking().Include(m => m.System).OrderBy(m => m.Name).ToListAsync();
                if (marketsOnly) {
                    list = list.Where(m => m.HasMarket).ToList();
                }

                return Ok(list.Select(x => new StationListItem_Api {
                    StationId = x.Id,
                    Station = x.Name,
                    System = x.System.Name,
                    SystemId = x.SystemId
                }).ToList());
            }
        }


        [HttpGet]
        public async Task<IHttpActionResult> StationsInSystem(string systemName) {
            using (ApplicationDbContext db = new ApplicationDbContext()) {
                StationsListResult_Api result = new StationsListResult_Api();
                EliteTrading.Entities.System system = await db.Systems.AsNoTracking().Where(m => m.Name.ToLower() == systemName.ToLower()).FirstOrDefaultAsync();
                if (system == null) {
                    result.Message = "System Not Found"; 
                    result.Success = false;
                    return Ok(result);
                }

                var list = await db.Stations.Include(m=>m.StationType).AsNoTracking().Where(m => m.System.Name == systemName).ToListAsync();

                if (list.Any() == false) {
                    result.Message = "No Stations";
                    result.Success = false;
                    return Ok(result);
                }

                if (list.Where(m => m.HasMarket).Any() == false) {
                    result.Message = "No Stations With Market";
                    result.Success = false;
                    return Ok(result);
                }

                result.Stations = list.Where(m=>m.HasMarket).Select(x => new StationListItem_Api {
                    StationId = x.Id,
                    Station = x.Name + " (" + x.StationType.TypeName  + " " + x.DistanceFromJumpIn + " ls)"
                }).OrderBy(m => m.Station).ToList();
                result.Success = true;
                if (result.Stations.Count > 1) {
                    result.Stations.Insert(0, new StationListItem_Api { Station = "Any" });
                }
                
                return Ok(result);
            }
        }



        [HttpGet]
        public async Task<IHttpActionResult> StationNames() {
            using (ApplicationDbContext db = new ApplicationDbContext()) {
                var output = await db.Stations.AsNoTracking().OrderBy(m => m.Name).Select(m => m.Name).ToListAsync();
                return Ok(output);
            }
        }





        [HttpGet]
        public async Task<IHttpActionResult> StationNamesWithId() {
            using (ApplicationDbContext db = new ApplicationDbContext()) {
                var list = await db.Stations.AsNoTracking().Include(m => m.System).OrderBy(m => m.Name).ToListAsync();
                var output = list.Select(x => new KeyValuePair<int, string>(x.Id, x.Name)).ToList();
                return Ok(output);
            }
        }





        [HttpGet]
        public async Task<IHttpActionResult> StationsWithSystemNames(bool marketsOnly = false) {
            using (ApplicationDbContext db = new ApplicationDbContext()) {
                var list = await db.Stations.AsNoTracking().Include(m => m.System).OrderBy(m => m.Name).ToListAsync();
                if (marketsOnly) {
                    return Ok(list.Where(m => m.HasMarket).Select(x => new KeyValuePair<string, string>(x.Name, x.System.Name + " (" + x.Name + ")")).ToList());
                }
                return Ok(list.Select(x => new KeyValuePair<string, string>(x.Name, x.System.Name + " (" + x.Name + ")")).ToList());
            }
        }





        [HttpGet]
        public async Task<IHttpActionResult> StationsWithSystemNamesAndId() {
            using (ApplicationDbContext db = new ApplicationDbContext()) {
                var list = await db.Stations.AsNoTracking().Include(m => m.System).OrderBy(m => m.Name).ToListAsync();
                var output = list.Select(x => new KeyValuePair<int, string>(x.Id, x.System.Name + " (" + x.Name + ")")).ToList();
                return Ok(output);
            }
        }





        [HttpPost]
        public async Task<IHttpActionResult> Search(SearchQuery query) {
            if (ModelState.IsValid) {
                QueryService _query = new QueryService();
                SearchResultViewModel model = await _query.Search(query);
                model.Query = query;
                if (User.Identity.IsAuthenticated) {
                    ReputationService _rep = new ReputationService();
                    model.RepResult = await _rep.AddRepAsync(User.Identity.Name, ActionRep.Query);
                    using (ApplicationDbContext db = new ApplicationDbContext()) {
                        ApplicationUser user = await db.Users.FirstAsync(m => m.UserName == User.Identity.Name);
                        user.Queries += 1;
                        db.Entry(user).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                    }
                }
                return Ok(model);
            }
            var allErrors = ModelState.Values.SelectMany(v => v.Errors);
            var error = new HttpError("Invalid parameters:" + Json(allErrors));
            return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, error));
        }





        [HttpPost]
        public async Task<IHttpActionResult> FindTrades(FindTradesQuery query) {
            if (ModelState.IsValid) {
                FindTradeResult model = new FindTradeResult();
                QueryService _query = new QueryService();
                await _query.FindTrades(query, model);

                if (User.Identity.IsAuthenticated) {
                    ReputationService _rep = new ReputationService();
                    model.RepResult = await _rep.AddRepAsync(User.Identity.Name, ActionRep.Query);
                    using (ApplicationDbContext db = new ApplicationDbContext()) {
                        ApplicationUser user = await db.Users.FirstAsync(m => m.UserName == User.Identity.Name);
                        user.Queries += 1;
                        db.Entry(user).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                    }
                }

                return Ok(model);
            }
            var allErrors = ModelState.Values.SelectMany(v => v.Errors);
            var error = new HttpError("Invalid parameters:" + Json(allErrors));
            return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, error));
        }




        [HttpPost]
        public async Task<IHttpActionResult> FindHighestTradeCommodity(FindHighestTradeCommodityQuery query) {
            if (ModelState.IsValid) {
                QueryService _query = new QueryService();
                HighestTradeCommodity result = await _query.FindHighestTradeCommodity(query.SourceStationId, query.DestinationStationId);
                return Json(result);
            }
            return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid parameters"));
        }




        [HttpPost]
        public async Task<IHttpActionResult> RareTrades(RareTradesQuery query) {
            using (ApplicationDbContext db = new ApplicationDbContext()) {
                EliteTrading.Entities.System currentLocation = await db.Systems.Where(m => m.Name == query.CurrentLocation).FirstOrDefaultAsync();
                if (currentLocation != null) {
                    List<RareTrade> rareTrades = await db.RareTrades.Include(m => m.Station).Include(m => m.Station.Allegiance).Include(m => m.Station.System).ToListAsync();

                    List<RareTradesResultsViewModel> model = rareTrades.Select(m => new RareTradesResultsViewModel {
                        Id = m.Id,
                        RareTrade = m.Name,
                        Price = m.Buy,
                        Location = m.Station.System.Name + " (" + m.Station.Name + ")",
                        Allegiance = m.Station.Allegiance.Name,
                        Distance = Math.Round(Math.Sqrt(Math.Pow(Math.Abs(m.Station.System.X - currentLocation.X), 2) + Math.Pow(Math.Abs(m.Station.System.Y - currentLocation.Y), 2) + Math.Pow(Math.Abs(m.Station.System.Z - currentLocation.Z), 2)), 2),
                        DistanceFromJumpIn = m.Station.DistanceFromJumpIn
                    }).OrderBy(m => m.Distance).ToList();

                    return Json(model);
                }
            }
            return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "System Not Found"));
        }



        [HttpPost]
        public async Task<IHttpActionResult> DataLists(EliteTrading.Models.DataLists.DataListsQuery query) {
            if (ModelState.IsValid) { 
               QueryService _query = new QueryService();
               var result = await _query.DataLists(query);
               return Json(result);
            }
            var allErrors = ModelState.Values.SelectMany(v => v.Errors);
            var error = new HttpError("Invalid parameters:" + Json(allErrors));
            return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, error));
        }

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
        }
    }
}
 