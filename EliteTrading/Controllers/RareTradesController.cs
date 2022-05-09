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

namespace EliteTrading.Controllers
{
    [Authorize(Roles = "Elite")]
    public class RareTradesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: RareTrades
        public async Task<ActionResult> Index()
        {
            var rareTrades = db.RareTrades.Include(r => r.Station).OrderBy(r=>r.Name);
            return View(await rareTrades.ToListAsync());
        }

        // GET: RareTrades/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RareTrade rareTrade = await db.RareTrades.FindAsync(id);
            if (rareTrade == null)
            {
                return HttpNotFound();
            }
            return View(rareTrade);
        }

        // GET: RareTrades/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RareTrades/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,StationId,Name,Buy")] RareTrade rareTrade)
        {
            if (ModelState.IsValid)
            {
                db.RareTrades.Add(rareTrade);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.StationId = new SelectList(db.Stations, "Id", "Name", rareTrade.StationId);
            return View(rareTrade);
        }

        // GET: RareTrades/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RareTrade rareTrade = await db.RareTrades.FindAsync(id);
            if (rareTrade == null)
            {
                return HttpNotFound();
            }
            ViewBag.StationId = new SelectList(db.Stations, "Id", "Name", rareTrade.StationId);
            return View(rareTrade);
        }

        // POST: RareTrades/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,StationId,Name,Buy")] RareTrade rareTrade)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rareTrade).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.StationId = new SelectList(db.Stations, "Id", "Name", rareTrade.StationId);
            return View(rareTrade);
        }

        // GET: RareTrades/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RareTrade rareTrade = await db.RareTrades.FindAsync(id);
            if (rareTrade == null)
            {
                return HttpNotFound();
            }
            return View(rareTrade);
        }

        // POST: RareTrades/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            RareTrade rareTrade = await db.RareTrades.FindAsync(id);
            db.RareTrades.Remove(rareTrade);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: RareTrades
        public ActionResult Import() {
            return View();
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
