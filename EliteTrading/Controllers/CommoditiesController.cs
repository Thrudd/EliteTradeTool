using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EliteTrading.Entities;
using EliteTrading.Services;

namespace EliteTrading.Controllers {
    [Authorize(Roles = "Master")]
    public class CommoditiesController : Controller {
        

        // GET: Commodities
        public async Task<ActionResult> Index() {
            using (ApplicationDbContext db = new ApplicationDbContext()) {
                var commodities = db.Commodities.Include(c => c.Category).OrderBy(c => c.Category.Name).ThenBy(c => c.Name);
                return View(await commodities.ToListAsync());
            }
        }

        // GET: Commodities/Details/5
        public async Task<ActionResult> Details(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            using (ApplicationDbContext db = new ApplicationDbContext()) {
                Commodity commodity = await db.Commodities.FindAsync(id);
                if (commodity == null) {
                    return HttpNotFound();
                }
                return View(commodity);
            }
        }

        // GET: Commodities/Create
        public ActionResult Create() {
            using (ApplicationDbContext db = new ApplicationDbContext()) {
                var list = db.Categories.OrderBy(c => c.Name).ToList();
                ViewBag.CategoryId = new SelectList(list, "Id", "Name");
                return View();
            }
        }

        // POST: Commodities/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,CategoryId,Name,GalacticAveragePrice, Max, Min")] Commodity commodity) {
            using (ApplicationDbContext db = new ApplicationDbContext()) {
                if (ModelState.IsValid) {
                    db.Commodities.Add(commodity);
                    await db.SaveChangesAsync();
                    ReputationService _rep = new ReputationService();
                    RepResult repResult = await _rep.AddRepAsync(User.Identity.Name, ActionRep.AddEditCommodity);
                    return RedirectToAction("Index");
                }

                ViewBag.CategoryId = new SelectList(db.Categories.OrderBy(c => c.Name), "Id", "Name", commodity.CategoryId);
                return View(commodity);
            }
        }

        // GET: Commodities/Edit/5
        public async Task<ActionResult> Edit(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            using (ApplicationDbContext db = new ApplicationDbContext()) {
                Commodity commodity = await db.Commodities.FindAsync(id);
                if (commodity == null) {
                    return HttpNotFound();
                }
                var cats = await db.Categories.OrderBy(c => c.Name).ToListAsync();
                ViewBag.CategoryId = new SelectList(cats, "Id", "Name", commodity.CategoryId);
                return View(commodity);
            }
        }

        // POST: Commodities/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,CategoryId,Name,GalacticAveragePrice, Max, Min")] Commodity commodity) {
            using (ApplicationDbContext db = new ApplicationDbContext()) {
                if (ModelState.IsValid) {
                    db.Entry(commodity).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    ReputationService _rep = new ReputationService();
                    RepResult repResult = await _rep.AddRepAsync(User.Identity.Name, ActionRep.AddEditCommodity);
                    return RedirectToAction("Index");
                }
                ViewBag.CategoryId = new SelectList(db.Categories.OrderBy(c => c.Name), "Id", "Name", commodity.CategoryId);
                return View(commodity);
            }
        }

        // GET: Commodities/Delete/5
        [Authorize(Roles = "Dangerous")]
        public async Task<ActionResult> Delete(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            using (ApplicationDbContext db = new ApplicationDbContext()) {
                Commodity commodity = await db.Commodities.FindAsync(id);
                if (commodity == null) {
                    return HttpNotFound();
                }
                return View(commodity);
            }
        }

        // POST: Commodities/Delete/5
        [Authorize(Roles = "Dangerous")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id) {
            using (ApplicationDbContext db = new ApplicationDbContext()) {
                Commodity commodity = await db.Commodities.FindAsync(id);
                db.Commodities.Remove(commodity);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
        }

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
        }
    }
}
