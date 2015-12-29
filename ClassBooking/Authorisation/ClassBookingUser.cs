using ClassBooking.Database;
using ClassBooking.Models;
using System.Linq;
using System.Security.Principal;

namespace ClassBooking.Authorisation
{
    public static class ClassBookingUser
    {
        public static bool IsAdmin(this IPrincipal user)
        {
            var gm = FindGymMember(user);
            return gm != null && gm.IsAdmin;
        }

        public static bool IsMember(this IPrincipal user)
        {
            return FindGymMember(user) != null;
        }
        private static GymMember FindGymMember(IPrincipal user)
        {
            GymContext db = new GymContext();
            var nameParts = user.Identity.Name.Split('\\');
            if (nameParts.Length < 1) return null;
            string sName = nameParts[1];
            return db.GymMembers.SingleOrDefault(m => m.StaffId == sName);
        } 
    }
}