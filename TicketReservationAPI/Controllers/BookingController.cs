using Microsoft.AspNetCore.Mvc;
using TicketReservationAPI.Models;
using TicketReservationAPI.Repository.IRepository;
using System.Threading.Tasks;

namespace TicketReservationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IEventRepository _eventRepository;

        public BookingController(IBookingRepository bookingRepository, IEventRepository eventRepository)
        {
            _bookingRepository = bookingRepository;
            _eventRepository = eventRepository;
        }

        [HttpPost("Book")]
        public async Task<IActionResult> BookTickets([FromBody] Booking booking)
        {
            var bookingSuccess = await _bookingRepository.AddBookingAsync(booking);
            if (!bookingSuccess)
                return BadRequest("Booking failed. Check event availability.");

            return Ok("Tickets booked successfully.");
        }

        [HttpDelete("Cancel/{id}")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var booking = await _bookingRepository.GetBookingByIdAsync(id);
            if (booking == null)
                return NotFound("Booking not found");

            var eventDetails = await _eventRepository.GetEventByIdAsync(booking.EventId);
            if (eventDetails != null)
            {
                eventDetails.AvailableSeats += booking.NumberOfTickets;
                await _eventRepository.UpdateEventAsync(eventDetails);
            }

            var result = await _bookingRepository.DeleteBookingAsync(id);
            return result ? Ok("Booking canceled successfully") : BadRequest("Failed to cancel booking");
        }

        [HttpGet("UserBookings/{userName}")]
        public async Task<IActionResult> GetBookingForUser(string userName)
        {
            var bookings = await _bookingRepository.GetBookingForUserAsync(userName);
            return Ok(bookings);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllBooking()
        {
            var bookings = await _bookingRepository.GetAllBookingAsync();
            return Ok(bookings);
        }
    }
}
