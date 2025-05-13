using System.ComponentModel.DataAnnotations;

namespace CLDV6211pt1.Models
{
    public class Venues
    {
        [Key]
        public int VenueID { get; set; }

        [Required]
        [StringLength(100)]
        public string VenueName { get; set; }

        [Required]
        [StringLength(200)]
        public string Location { get; set; }

        [Required]
        public int Capacity { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(500)]
        public string? ImageURL { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public IFormFile? ImageFile { get; set; }

        // Navigation property: One venue can host many events
        public List<Events> Events { get; set; } = new();
    }
}
