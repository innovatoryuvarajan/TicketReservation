using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TicketReservationAPI.Data;
using TicketReservationAPI.Models;
using TicketReservationAPI.Repository.IRepository;

namespace TicketReservationAPI.Repository
{
    public class EventRepository : IEventRepository
    {
        private readonly TicketDbContext _context;

        public EventRepository(TicketDbContext context)
        {
            _context = context;
        }
        public async Task<bool> AddEventAsync(Event eventDetails)
        {
            await _context.Events.AddAsync(eventDetails);
            var result=await _context.SaveChangesAsync();
          Console.WriteLine("While Calling Add Event : ");
            Console.WriteLine(result);
            return result > 0;
        }

        public async Task<bool> DeleteEventAsync(int id)
        {
            var eventDetails = await _context.Events.FindAsync(id);
            if(eventDetails!=null)
            {
                _context.Events.Remove(eventDetails);
                var result = await _context.SaveChangesAsync();
                Console.WriteLine("While Calling Delete Event : ");
                Console.WriteLine(result);
                return result > 0;
            }
            return false;
            
        }

        public async Task<IEnumerable<Event>> GetAllEventsAsync() => await _context.Events.ToListAsync();


        public async Task<Event> GetEventByIdAsync(int id) => await _context.Events.FindAsync(id);
        

        public async Task<bool> UpdateEventAsync(Event eventDetails)
        {
            var existingEvent = await _context.Events.FirstOrDefaultAsync(e=>e.Id==eventDetails.Id);
            if(existingEvent!=null)
            {
                existingEvent.EventName= eventDetails.EventName;
                existingEvent.EventDate= eventDetails.EventDate;
                existingEvent.Venue= eventDetails.Venue;
                existingEvent.TotalSeats= eventDetails.TotalSeats;
                existingEvent.AvailableSeats= eventDetails.AvailableSeats;

                var result=await _context.SaveChangesAsync();
                Console.WriteLine("While Calling Update Event : ");
                Console.WriteLine(result);
                return result > 0;

            }
            return false;
        }
    }
}
