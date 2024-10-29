using System.ComponentModel.DataAnnotations.Schema;

namespace TicketReservationAPI.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string EventName { get; set; }

        public string UserName { get; set; }
        public int NumberOfTickets { get; set; }
        public DateTime BookingDate { get; set; }
        public string BookingReference { get; set; }

    }
}
