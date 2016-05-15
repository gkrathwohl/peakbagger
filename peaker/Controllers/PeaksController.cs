using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using peaker.Models;

namespace peaker.Controllers
{
    public class PeaksController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Peaks
        public ActionResult Index()
        {
            return View(db.Peaks.ToList());
        }

        // GET: Peaks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Peak peak = db.Peaks.Find(id);
            if (peak == null)
            {
                return HttpNotFound();
            }
            return View(peak);
        }

        // GET: Peaks/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Peaks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,STCTYFIPS,ELEV_METER,Latitude,Longitude")] Peak peak)
        {
            if (ModelState.IsValid)
            {
                db.Peaks.Add(peak);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(peak);
        }

        // GET: Peaks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Peak peak = db.Peaks.Find(id);
            if (peak == null)
            {
                return HttpNotFound();
            }
            return View(peak);
        }

        // POST: Peaks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,STCTYFIPS,ELEV_METER,Latitude,Longitude")] Peak peak)
        {
            if (ModelState.IsValid)
            {
                db.Entry(peak).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(peak);
        }

        // GET: Peaks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Peak peak = db.Peaks.Find(id);
            if (peak == null)
            {
                return HttpNotFound();
            }
            return View(peak);
        }

        // POST: Peaks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Peak peak = db.Peaks.Find(id);
            db.Peaks.Remove(peak);
            db.SaveChanges();
            return RedirectToAction("Index");
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
