using Microsoft.EntityFrameworkCore;
using TicketReservationAPI.Models;

namespace TicketReservationAPI.Data
{
    public class TicketDbContext : DbContext
    {
        public TicketDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Event> Events { get; set; }
        public DbSet<Booking> Bookings { get; set; }
    }
}
