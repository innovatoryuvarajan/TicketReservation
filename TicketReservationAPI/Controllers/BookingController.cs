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
            var eventDetails = await _eventRepository.GetEventByIdAsync(booking.EventId);
            if (eventDetails == null || eventDetails.AvailableSeats < booking.NumberOfTickets)
                return BadRequest("Not enough seats available or event not found");

            eventDetails.AvailableSeats -= booking.NumberOfTickets;
            var bookingSuccess = await _bookingRepository.AddBookingAsync(booking);
            var eventUpdateSuccess = await _eventRepository.UpdateEventAsync(eventDetails);

            Console.WriteLine(bookingSuccess+"==>"+ eventUpdateSuccess);
            Console.WriteLine(eventDetails);
            return bookingSuccess && eventUpdateSuccess ? Ok("Booking successful") : BadRequest("Failed to book tickets");
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
