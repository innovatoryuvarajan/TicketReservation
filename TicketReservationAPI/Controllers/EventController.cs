using Microsoft.AspNetCore.Mvc;
using TicketReservationAPI.Models;
using TicketReservationAPI.Repository.IRepository;
using System.Threading.Tasks;

namespace TicketReservationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventRepository _eventRepository;

        public EventController(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddEvent([FromBody] Event eventDetails)
        {
            var result = await _eventRepository.AddEventAsync(eventDetails);
            return result ? Ok("Event added successfully") : BadRequest("Failed to add event");
        }

        [HttpPut("Edit/{id}")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] Event eventDetails)
        {
            eventDetails.Id = id;
            var result = await _eventRepository.UpdateEventAsync(eventDetails);
            return result ? Ok("Event updated successfully") : NotFound("Event not found");
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var result = await _eventRepository.DeleteEventAsync(id);
            return result ? Ok("Event deleted successfully") : NotFound("Event not found");
        }

        [HttpGet("Get/{id}")]
        public async Task<IActionResult> GetEvent(int id)
        {
            var eventDetails = await _eventRepository.GetEventByIdAsync(id);
            return eventDetails != null ? Ok(eventDetails) : NotFound("Event not found");
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllEvents()
        {
            var events = await _eventRepository.GetAllEventsAsync();
            return Ok(events);
        }
    }
}
