using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ClassBooking.Models;
using ClassBooking.Database;
using System;
using ClassBooking.Authorisation;
using ClassBooking.Attributes;

namespace ClassBooking.Controllers
{
    public class GymClassController : Controller
    {
        private GymContext db = new GymContext();

        // GET: GymClass
        public ActionResult Index()
        {
            return View(db.GymClass.ToList().Select(v => v.ToViewModel()));
        }

        // GET: GymClass/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GymClass gymClass = db.GymClass.Find(id);
            if (gymClass == null)
            {
                return HttpNotFound();
            }
            gymClass.Bookings = db.MemberClassBookings.Where(bk => bk.GymClassId == id).ToList();
            gymClass.nBookings = db.MemberClassBookings.Where(bk => bk.GymClassId == id && !bk.Waiting).Count();

            return View(gymClass);
        }

        // GET: GymClass/Create
        public ActionResult Create()
        {
            GymClass gymClass = new GymClass();
            gymClass.ClassDate = DateTime.Now.AddDays(1).ToString("dd/MM/yyyy");
            gymClass.Types = db.GymClassTypes.ToList();
            return View(gymClass);
        }

        // POST: GymClass/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ClassBookingAuthorise(Level=AuthorisationLevel.Administrator)]
        public ActionResult Create([Bind(Include = "GymClassId,ClassTime,ClassDate,GymClassTypeId,Description,MaxCapacity,MaxWaitList")] GymClass gymClass)
        {
            if (ModelState.IsValid)
            {
                db.GymClass.Add(gymClass.ToModel());
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(gymClass);
        }

        // GET: GymClass/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GymClass gymClass = db.GymClass.Find(id).ToViewModel();
            if (gymClass == null)
            {
                return HttpNotFound();
            }
            gymClass.Bookings = db.MemberClassBookings.Where(b => b.GymClassId == id).ToList();
            gymClass.Types = db.GymClassTypes.ToList();
            return View(gymClass);
        }

        // POST: GymClass/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ClassBookingAuthorise(Level = AuthorisationLevel.Administrator)]
        public ActionResult Edit([Bind(Include = "GymClassId,Description,GymClassTypeId,ClassTime,ClassDate,MaxCapacity,MaxWaitList")] GymClass gymClass)
        {
            if (ModelState.IsValid)
            {
                db.Entry(gymClass.ToModel()).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(gymClass);
        }

        // GET: GymClass/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GymClass gymClass = db.GymClass.Find(id);
            if (gymClass == null)
            {
                return HttpNotFound();
            }
            return View(gymClass);
        }

        // POST: GymClass/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [ClassBookingAuthorise(Level = AuthorisationLevel.Administrator)]
        public ActionResult DeleteConfirmed(int id)
        {
            GymClass gymClass = db.GymClass.Find(id);
            db.GymClass.Remove(gymClass);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [ClassBookingAuthorise(Level = AuthorisationLevel.Administrator)]
        public ActionResult CancelBooking(int classId, int memberId)
        {
            GymClass cl = db.GymClass.Find(classId);
            if (cl == null)
            {
                ViewBag.Error = "Class not found";
                return View("BookingError");
            }
            var booking = db.MemberClassBookings.Where(bk => bk.GymClassId == cl.GymClassId && bk.GymMemberId == memberId).FirstOrDefault();
            if (booking != null)
            {
                db.Entry(booking).State = EntityState.Deleted;
                db.SaveChanges();
            }
            else
            {
                ViewBag.Error = "Booking could not be found!";
                return View("BookingError");
            }
            return RedirectToAction("Details", new { id = classId });
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
