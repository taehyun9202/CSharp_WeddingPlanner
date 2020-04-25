using System;
using System.ComponentModel.DataAnnotations;

namespace WeddingPlanner.Models
{
    public class Association
    {
        [Key]
        public int AssociationId { get; set; }
        public int UserId { get; set; }
        public int ReservationId { get; set; }
        public User Guest { get; set; }
        public Reservation Wedding { get; set; }

    }
}