using TicketReservationAPI.Models;

namespace TicketReservationAPI.Repository.IRepository
{
    public interface IEventRepository
    {
        Task<bool> AddEventAsync(Event eventDetails);
        Task<bool> UpdateEventAsync(Event eventDetails);
        Task<bool> DeleteEventAsync(int id);
        Task<Event> GetEventByIdAsync(int id);
        Task<IEnumerable<Event>> GetAllEventsAsync();
    }
}
