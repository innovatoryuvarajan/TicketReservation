using TicketReservationAPI.Models;

namespace TicketReservationAPI.Repository.IRepository
{
    public interface IBookingRepository
    {
        Task<bool> AddBookingAsync(Booking booking);
        Task<bool> DeleteBookingAsync(int id);
        Task<Booking> GetBookingByIdAsync(int id);
        Task<IEnumerable<Booking>> GetBookingForUserAsync(string userName);
        Task<IEnumerable<Booking>> GetAllBookingAsync();


    }
}
