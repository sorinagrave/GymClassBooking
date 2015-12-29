using System.Collections.Generic;

namespace ClassBooking.Models
{
    public class GymMemberBookings
    {
        public IList<GymClass> GymClasses { get; set; }
        public GymMember CurrentMember { get; set; }
        public IList<GymMember> AllMembers { get; set; }
    }
}