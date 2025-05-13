using System.Configuration;
using CLDV6211pt1.Models;
using Microsoft.EntityFrameworkCore;

namespace EventEase.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
   

        }


        public DbSet<Bookings> Bookings { get; set; } = null!; // Ensure this matches your model class
        public DbSet<Events> Events { get; set; } = null!; // Ensure this matches your model class
        public DbSet<Venues> Venues { get; set; } = null!; // Ensure this matches your model class
    }
}
