using Microsoft.EntityFrameworkCore;
using TicketReservationAPI.Models;

namespace TicketReservationAPI.Data
{
    public class TicketDbContext : DbContext
    {
        public TicketDbContext(DbContextOptions options) : base(options)
        {
        }

        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<Booking> Bookings { get; set; }
    }
}
