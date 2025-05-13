using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CLDV6211pt1.Models
{
    public class Bookings
    {
        [Key]
        public int BookingID { get; set; }

        [ForeignKey("Customer")]
        public int CustomerID { get; set; }

        [ForeignKey("Events")]
        public int EventID { get; set; }

        public DateTime BookingDate { get; set; }

        public int SeatsBooked { get; set; }

        public string? BookingStatus { get; set; }

        // Navigation properties (singular)
        public Events? Events { get; set; }
    }
}