using ClassBooking.Attributes;
using ClassBooking.Authorisation;
using ClassBooking.Database;
using ClassBooking.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ClassBooking.Controllers
{
    public class HomeController : Controller
    {
       
        private GymContext db = new GymContext();

        // GET: GymClassTypes
        public ActionResult Index(int memberId = 0)
        {
            GymMemberBookings gmb = new GymMemberBookings();
            List<GymClass> allFutureClasses = db.GymClass.Where(cl => cl.ClassDateTime > DateTime.Now).ToList();
            GymMember gm;
            if (memberId == 0)
            {
                gm = GetCurrentGymMember();
            }
            else
            {
                gm = db.GymMembers.Find(memberId);
            }
            if (gm != null)
            {
                foreach (GymClass cl in allFutureClasses)
                {
                    cl.nBookings = db.MemberClassBookings.Where(bk => bk.GymClassId == cl.GymClassId && !bk.Waiting).Count();                 
                    var memberBooking = db.MemberClassBookings.Where(bk => bk.GymClassId == cl.GymClassId && bk.GymMemberId == gm.GymMemberId).FirstOrDefault();
                    if (memberBooking != null)
                    {
                        cl.MemberStatus = memberBooking.Waiting ? MemberClassStatus.BookedWaiting : MemberClassStatus.BookedClass;
                    }
                    else
                    {
                        if (cl.nBookings < cl.MaxCapacity)
                        {
                            cl.MemberStatus = MemberClassStatus.EligibleClass;
                        }
                        else
                        {
                            int nAllBookings = db.MemberClassBookings.Where(bk => bk.GymClassId == cl.GymClassId).Count();
                            cl.MemberStatus = (nAllBookings < cl.MaxCapacity + cl.MaxWaitList ? MemberClassStatus.EligibleWaiting : MemberClassStatus.None);
                        }
                    }
                    cl.ToViewModel();
                }
            }
            gmb.GymClasses = allFutureClasses;
            gmb.CurrentMember = gm;
            gmb.AllMembers = db.GymMembers.ToList();
            foreach(var memb in gmb.AllMembers)
            {
                memb.FullName = String.Format("{0} {1}", memb.FirstName, memb.LastName);
            }
            return View(gmb);
        }
        [ClassBookingAuthorise(Level = AuthorisationLevel.Member)]
        public ActionResult Book(int classId)
        {
            GymMember gm = GetCurrentGymMember();
            if(gm == null)
            {
                ViewBag.Error = "Member not found";
                return View("BookingError");
            }

            return BookClassMember(classId, gm.GymMemberId);
        }
        [ClassBookingAuthorise(Level = AuthorisationLevel.Administrator)]
        public ActionResult BookMember(int classId, int memberId)
        {
            return BookClassMember(classId, memberId);
        }
        private ActionResult BookClassMember(int classId, int memberId)
        {
            GymClass cl = db.GymClass.Find(classId);
            if (cl == null)
            {
                ViewBag.Error = "Class not found";
                return View("BookingError");
            }
            int nAllBooked = db.MemberClassBookings.Where(bk => bk.GymClassId == cl.GymClassId).Count();
            int nBooked = db.MemberClassBookings.Where(bk => bk.GymClassId == cl.GymClassId && !bk.Waiting).Count();
            if (nAllBooked < cl.MaxCapacity + cl.MaxWaitList)
            {
                GymClassBooking booking = new GymClassBooking();
                booking.GymClassId = classId;
                booking.GymMemberId = memberId;
                booking.Waiting = nBooked >= cl.MaxCapacity;
                db.Entry(booking).State = EntityState.Added;
                db.SaveChanges();
            }
            else
            {
                ViewBag.Error = "Class is fully booked!";
                return View("BookingError");
            }
            return RedirectToAction("Index",new { memberId = memberId });
        }
        [ClassBookingAuthorise(Level = AuthorisationLevel.Member)]
        public ActionResult Cancel(int classId)
        {
            GymMember gm = GetCurrentGymMember();
            if (gm == null)
            {
                ViewBag.Error = "Member not found";
                return View("BookingError");
            }
            return CancelMemberBooking(classId, gm.GymMemberId);
        }
        [ClassBookingAuthorise(Level = AuthorisationLevel.Administrator)]
        public ActionResult CancelMember(int classId, int memberId)
        {
            return CancelMemberBooking(classId, memberId);
        }
        private ActionResult CancelMemberBooking(int classId, int memberId)
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
            return RedirectToAction("Index",new {memberId = memberId });
        }
        private GymMember GetCurrentGymMember()
        {
            var nameParts = User.Identity.Name.Split('\\');
            if (nameParts.Length < 1) return null;
            string sName = nameParts[1];
            return db.GymMembers.SingleOrDefault(m => m.StaffId == sName);
        }
    }
}