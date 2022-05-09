using EliteTrading.Entities;
using EliteTrading.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EliteTrading.Controllers {
    [Authorize(Roles = "Elite")]
    public class NewsController : Controller {
        readonly NewsService _data = new NewsService();
        //
        // GET: /Notification/
        public ActionResult Index(string result = null) {

            News model = _data.GetAdminNews();
            if (model == null) {
                model = new News {
                    Id = 0,
                    ShowFrom = DateTime.Now,
                    ShowTo = DateTime.Now.AddHours(2)
                };
            }
            ViewBag.Result = result;
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(News model) {
            if (ModelState.IsValid) {
                _data.SetNews(model);
            }
            return RedirectToAction("Index", new { result = "Notificaton Set" });
        }

        public ActionResult Clear() {
            _data.Clear();
            News model = _data.GetAdminNews();
            return RedirectToAction("Index", new { result = "Notificaton Cleared" });
        }
    }
}