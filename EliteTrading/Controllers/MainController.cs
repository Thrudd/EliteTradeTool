using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using EliteTrading.Entities;
using EliteTrading.Models.ViewModels;
using EliteTrading.Services;
using EliteTrading.Models;
using EliteTrading.Extensions;
using EliteTrading.Models.Search;

namespace EliteTrading.Controllers {
    public class MainController : AjaxAwareController {

        // GET: Query
        public ActionResult Index() {
            NewsService notificationService = new NewsService();
            ViewBag.News = notificationService.GetNews();

            return View();
        }


        public ActionResult Holding() {
            return View();
        }



        public ActionResult Thanks() {
            return View();
        }



        public ActionResult TradeCalculator() {
            return View();
        }



        public ActionResult FindTrades() {
            return View();
        }

        public ActionResult Privacy() {
            return View();
        }

        public async Task<ActionResult> DataLists() {
            using (ApplicationDbContext db = new ApplicationDbContext()) {
                var commodities = await db.Commodities.AsNoTracking().OrderBy(c => c.Name).ToListAsync();

                ViewBag.CommodityId = new SelectList(commodities, "Id", "Name");

                if (IsAjaxRequest) {
                    return PartialView();
                } else {
                    ViewBag.Layout = "~/Views/Shared/_Layout.cshtml";
                    return View();
                }
            }
        }

        public ActionResult RareTrades() {
            if (IsAjaxRequest) {
                return PartialView();
            } else {
                ViewBag.Layout = "~/Views/Shared/_Layout.cshtml";
                return View();
            }
        }


        //[OutputCache(Duration = 3600, VaryByParam = "none")]
        public async Task<ActionResult> SearchTab() {
            using (ApplicationDbContext db = new ApplicationDbContext()) {
                SearchQueryViewModel model = new SearchQueryViewModel();

                model.Commodities = await db.Commodities.OrderBy(c => c.Name).Select(x => new SelectListItem() {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToListAsync();

                model.Economies = await db.Economy.OrderBy(c => c.Name).Select(x => new SelectListItem() {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToListAsync();

                model.Governments = await db.Governments.OrderBy(c => c.Name).Select(x => new SelectListItem() {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToListAsync();

                model.Allegiances = await db.Allegiances.OrderBy(c => c.Name).Select(x => new SelectListItem() {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToListAsync();

                return PartialView(model);
            }
        }



        [OutputCache(Duration = 60, VaryByParam = "id")]
        public async Task<ActionResult> StationDetails(int id) {
            if (ModelState.IsValid) {

                QueryService _query = new QueryService();
                StationDetailViewModel model = await _query.GetStationDetail(id);
                if (User.Identity.IsAuthenticated) {
                    ReputationService _rep = new ReputationService();
                    model.RepResult = await _rep.AddRepAsync(User.Identity.Name, ActionRep.Query);
                }
                if (IsAjaxRequest) {
                    return PartialView("_StationDetails", model);
                } else {
                    ViewBag.Layout = "~/Views/Shared/_Layout.cshtml";
                    return View("_StationDetails", model);
                } 
            }
            return Content("Invalid Search");
        }



        public async Task<ActionResult> Contributors() {
            using (ApplicationDbContext db = new ApplicationDbContext()) {
                var users = await db.Users.Where(m => m.Updates > 0).OrderByDescending(m => m.Rank).Take(50).ToListAsync();

                return View(users);
            }
        }


        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
        }
    }
}