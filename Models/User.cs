using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace WeddingPlanner.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        [MinLength(2)]
        public string FirstName { get; set; }
        [Required]
        [MinLength(2)]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get;set; }
        
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string Confirm { get;set; }
        public List<Reservation> Plans { get;set; }
        public List<Association> Attending { get;set; }
        public DateTime Date { get; set; } = DateTime.Now;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}