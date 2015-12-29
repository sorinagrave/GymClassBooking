using ClassBooking.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace ClassBooking.Database
{
    public class GymContext : DbContext
    {
        public GymContext() : base("GymContext")
        {
        }
        public DbSet<GymClassType> GymClassTypes { get; set; }
        public DbSet<GymClass> GymClass { get; set; }
        public DbSet<GymMember> GymMembers { get; set; }
        public DbSet<GymClassBooking> MemberClassBookings { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Gym Booking Database does not pluralize table names
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}