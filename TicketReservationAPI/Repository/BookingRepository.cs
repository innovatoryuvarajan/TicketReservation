using Microsoft.EntityFrameworkCore;
using TicketReservationAPI.Data;
using TicketReservationAPI.Models;
using TicketReservationAPI.Repository.IRepository;
using System.Threading.Tasks;

namespace TicketReservationAPI.Repository
{
    public class BookingRepository : IBookingRepository
    {
        private readonly TicketDbContext _context;
        private readonly IEventRepository _eventRepository;

        public BookingRepository(TicketDbContext context, IEventRepository eventRepository)
        {
            _context = context;
            _eventRepository = eventRepository;
        }

        public async Task<bool> AddBookingAsync(Booking booking)
        {
            var eventDetails = await _eventRepository.GetEventByIdAsync(booking.EventId);
            if (eventDetails == null || eventDetails.AvailableSeats < booking.NumberOfTickets)
                return false;

            eventDetails.AvailableSeats -= booking.NumberOfTickets;
            await _context.Bookings.AddAsync(booking);
            await _eventRepository.UpdateEventAsync(eventDetails);
           // var result = await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteBookingAsync(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<IEnumerable<Booking>> GetAllBookingAsync() => await _context.Bookings.ToListAsync();

        public async Task<Booking> GetBookingByIdAsync(int id) => await _context.Bookings.FindAsync(id);

        public async Task<IEnumerable<Booking>> GetBookingForUserAsync(string userName)
        {
            return await _context.Bookings.Where(b => b.UserName == userName).ToListAsync();
        }
    }
}
