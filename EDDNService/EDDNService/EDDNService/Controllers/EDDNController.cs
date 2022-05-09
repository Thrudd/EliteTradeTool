using System;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Collections.Generic;

using EDDNService.Services;
using EDDNService.Extensions;
using EliteTrading.Entities;

namespace EDDNService.Controllers { 
    [Authorize(Users = "zebidie@hotmail.com")]
    public class EDDNController : Controller {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: EDDN
        public ActionResult Index() {
            return View();
        }

        [HttpPost]
        public ActionResult EDDN(bool start) {
            if (start) {
                EDDNService.Services.EDDN.StartListening();
            } else {
                EDDNService.Services.EDDN.StopListening();
            }
            return View();
        }

        public async Task<CustomJsonResult> EDDNLog(int id = 0) {
            var list = await db.EDDNLogs.OrderByDescending(m => m.TimeStamp).Take(50).ToListAsync();
            CustomJsonResult result = new CustomJsonResult();
            result.Data = list;
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }

        [AllowAnonymous]
        public JsonResult EDDNStatus() {
            return Json(new { Connected = EDDNService.Services.EDDN.Connected, Status = EDDNService.Services.EDDN.GetThreadStatus() }, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing) {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}