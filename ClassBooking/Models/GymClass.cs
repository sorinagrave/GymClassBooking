using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace ClassBooking.Models
{
    [Table("GymClass")]
    public class GymClass
    {
        public int GymClassId { get; set; }
        [Required]
        [Display(Name = "Class Type")]
        public int GymClassTypeId { get; set; }
        [Display(Name = "Class Type")]
        public virtual GymClassType Type { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Required]
        [Display(Name = "Class date time")]
        public DateTime ClassDateTime { get; set; }
        [Required]
        [Display(Name = "Maximum Capacity")]
        public int MaxCapacity { get; set; }
        [Required]
        [Display(Name = "Maximum Wait List")]
        public int MaxWaitList { get; set; }
        [Required]
        [NotMapped]
        [Display(Name = "Class Time (hh:mm)")]
        public string ClassTime { get; set;}
        [Required]
        [NotMapped]
        [Display(Name = "Class Date (dd-mm-yy)")]
        public string ClassDate { get; set; }

        [NotMapped]
        public IEnumerable<GymClassType> Types { get; set; }

        public virtual IEnumerable<GymClassBooking> Bookings { get; set; }

        [NotMapped]
        [Display(Name = "Number of Bookings")]
        public int nBookings { get; set; }

        [NotMapped]
        public MemberClassStatus MemberStatus { get; set; }
    }
    public static class GymClassMapper
    {
        public static GymClass ToViewModel(this GymClass gymClass)
        {
            gymClass.ClassTime = gymClass.ClassDateTime.ToString("HH:mm");
            gymClass.ClassDate = gymClass.ClassDateTime.ToString("dd/MM/yyyy");
            return gymClass;
        }

        public static GymClass ToModel(this GymClass gymClass)
        {
            DateTime d;
            bool dateOk = DateTime.TryParseExact(gymClass.ClassDate + " " + gymClass.ClassTime,"dd/MM/yyyy HH:mm",CultureInfo.InvariantCulture, DateTimeStyles.None, out d);
            if (dateOk)
            {
                gymClass.ClassDateTime = d;
                return gymClass;
            }
            return null;
        }
    }
}