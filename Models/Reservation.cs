using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace WeddingPlanner.Models
{
    public class Reservation
    {
        [Key]
        public int ReservationId { get;set; }
        [Required]
        public string GroomName { get;set; }
        [Required]
        public string BrideName { get;set; }
        [Required]
        [FutureDate]
        public DateTime Date { get;set; }

        [Required]
        public string Address { get;set; }
        public User Creator { get;set; }
        public int UserId { get;set; }
        public List<Association> Guests { get;set; }

    }
    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(value == null)
            {
                return new ValidationResult("Please Select the Date");
            }
            DateTime compare;
            if(value is DateTime)
            {
                compare = (DateTime)value;
                if(compare < DateTime.Now)
                {
                    return new ValidationResult("Please Select the Date in the Future");
                }
                else
                {
                    return ValidationResult.Success;
                }
            }
            else
            {
                return new ValidationResult("Not a Valid Date");
            }
        }
    }

}