using Microsoft.EntityFrameworkCore;
using TicketReservationAPI.Data;
using TicketReservationAPI.Models;
using TicketReservationAPI.Repository.IRepository;

namespace TicketReservationAPI.Repository
{
    public class BookingRepository : IBookingRepository
    {
        private readonly TicketDbContext _context;

        public BookingRepository(TicketDbContext context)
        {
            _context = context;
        }
        public async Task<bool> AddBookingAsync(Booking booking)
        {
            await _context.Bookings.AddAsync(booking);
            var result=await _context.SaveChangesAsync();
            Console.WriteLine("While Calling Add Booking : ");
            Console.WriteLine(result);
            return result > 0;

        }

        public async Task<bool> DeleteBookingAsync(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if(booking!=null)
            {
                _context.Bookings.Remove(booking);
                var result=await _context.SaveChangesAsync();
                Console.WriteLine("While Calling Delete Booking : ");
                Console.WriteLine(result);
                return result > 0;
            }
            return false;
        }

        public async Task<IEnumerable<Booking>> GetAllBookingAsync() => await _context.Bookings.ToListAsync();




        public async Task<Booking> GetBookingByIdAsync(int id) => await _context.Bookings.FindAsync(id);


        public async Task<IEnumerable<Booking>> GetBookingForUserAsync(string userName)
        {
            return await _context.Bookings.Where(b=>b.UserName==userName).ToListAsync();
        }
    }
}
