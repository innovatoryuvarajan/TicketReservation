using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketReservationAPI.Models;
using TicketReservationAPI.Repository.IRepository;

namespace TicketReservationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {

        private readonly IBookingRepository _bookingRepository;

        public readonly IEventRepository _eventRepository;

        public BookingController(IBookingRepository bookingRepository, IEventRepository eventRepository)
        {
            _bookingRepository = bookingRepository;
            _eventRepository = eventRepository;
        }

        [HttpPost]
        public async Task<IActionResult> BookTickets([FromBody] Booking booking)
        {
            var eventDetails = await _eventRepository.GetEventByIdAsync(booking.EventId);
            if (eventDetails == null || eventDetails.AvailableSeats < booking.NumberOfTickets)
                return BadRequest("Not enough seats available or event not found");
            eventDetails.AvailableSeats -= booking.NumberOfTickets;
            var bookingSuccess = await _bookingRepository.AddBookingAsync(booking);
            var eventUpdateSuccess = await _eventRepository.UpdateEventAsync(eventDetails);

            return bookingSuccess && eventUpdateSuccess ? Ok("Booking Successful") : BadRequest("Failed to book tickets");
        }

        [HttpDelete("{id}")]
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
            return result ? Ok("Booking Canceled Successfully") : BadRequest("Failed to cancel booking");
        }

        [HttpGet("{userName}")]
        public async Task<IActionResult> GetBookingForUser(string userName)
        {
            var bookings = await _bookingRepository.GetBookingForUserAsync(userName);
            return Ok(bookings);

        }

        [HttpGet]
        public async Task<IActionResult> GetAllBooking()
        {
            var booking = await _bookingRepository.GetAllBookingAsync();
            return Ok(booking);
        }


    }
}
