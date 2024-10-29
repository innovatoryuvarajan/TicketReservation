using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketReservationAPI.Models;
using TicketReservationAPI.Repository.IRepository;

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

        [HttpPost]
        public async Task<IActionResult> AddEvent([FromBody] Event eventDetails)
        {
            var result = await _eventRepository.AddEventAsync(eventDetails);
            return result ? Ok("Event added successfully") : BadRequest("Failed to add event");
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateResult(int id, [FromBody] Event eventdetails)
        {
            eventdetails.Id = id;
            var result = await _eventRepository.UpdateEventAsync(eventdetails);
            return result ? Ok("Event Updated Successfully") : NotFound("Event not Found");

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var result=await _eventRepository.DeleteEventAsync(id);
            return result ? Ok("Event Deleted Successfully") : NotFound("Event not found");

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEvent(int id)
        {
            var eventDetails = await _eventRepository.GetEventByIdAsync(id);
            return eventDetails != null ? Ok(eventDetails) : NotFound("Event not found");

        }

        [HttpGet]
        public async Task<IActionResult> GetAllEvents()
        {
            var events = await _eventRepository.GetAllEventsAsync();
            return Ok(events);
        }
    }
}
