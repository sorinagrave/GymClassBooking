using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ClassBooking.Models;
using ClassBooking.Database;
using ClassBooking.Authorisation;
using ClassBooking.Attributes;

namespace ClassBooking.Controllers
{
    public class GymMemberController : Controller
    {
        private GymContext db = new GymContext();

        // GET: GymMember
        public ActionResult Index()
        {
            return View(db.GymMembers.ToList());
        }

        // GET: GymMember/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GymMember gymMember = db.GymMembers.Find(id);
            if (gymMember == null)
            {
                return HttpNotFound();
            }
            return View(gymMember);
        }

        // GET: GymMember/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: GymMember/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ClassBookingAuthorise(Level = AuthorisationLevel.Administrator)]
        public ActionResult Create([Bind(Include = "GymMemberId,StaffId,FirstName,LastName,PhoneNumber,EmailAddress, IsAdmin")] GymMember gymMember)
        {
            if (ModelState.IsValid)
            {
                db.GymMembers.Add(gymMember);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(gymMember);
        }

        // GET: GymMember/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GymMember gymMember = db.GymMembers.Find(id);
            if (gymMember == null)
            {
                return HttpNotFound();
            }
            return View(gymMember);
        }

        // POST: GymMember/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ClassBookingAuthorise(Level = AuthorisationLevel.Administrator)]
        public ActionResult Edit([Bind(Include = "GymMemberId,StaffId,FirstName,LastName,PhoneNumber,EmailAddress,IsAdmin")] GymMember gymMember)
        {
            if (ModelState.IsValid)
            {
                db.Entry(gymMember).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(gymMember);
        }

        // GET: GymMember/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GymMember gymMember = db.GymMembers.Find(id);
            if (gymMember == null)
            {
                return HttpNotFound();
            }
            return View(gymMember);
        }

        // POST: GymMember/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [ClassBookingAuthorise(Level = AuthorisationLevel.Administrator)]
        public ActionResult DeleteConfirmed(int id)
        {
            GymMember gymMember = db.GymMembers.Find(id);
            db.GymMembers.Remove(gymMember);
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
