using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassBooking.Models
{
    [Table("GymClassType")]
    [JsonObject]
    public class GymClassType
    {
        public int GymClassTypeId { get; set; }
        [Display(Name = "Class Type")]
        [Required]
        public string Name { get; set; }
        [Display(Name = "Class Type Description")]
        public string Description { get; set; }

        [Display(Name = "Maximum capacity")]
        [Required]
        public int MaxCapacity { get; set; }

        [Display(Name = "Maximum wait list")]
        [Required]
        public int MaxWaitList { get; set; }

        [Display(Name = "Day of the week")]
        public DayOfWeek DayOfTheWeek { get; set; }

        [Display(Name = "Class time")]
        public string ClassTime { get; set; }
    }
}