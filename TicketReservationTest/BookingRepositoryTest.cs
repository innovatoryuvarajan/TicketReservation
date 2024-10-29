using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TicketReservationAPI.Data;
using TicketReservationAPI.Models;
using TicketReservationAPI.Repository;
using TicketReservationAPI.Repository.IRepository;
using Moq;

namespace TicketReservationAPI.Tests
{
    [TestFixture]
    public class BookingRepositoryTests
    {
        private TicketDbContext _context;
        private BookingRepository _bookingRepository;
        private Mock<IEventRepository> _eventRepositoryMock;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<TicketDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new TicketDbContext(options);
            _eventRepositoryMock = new Mock<IEventRepository>();
            _bookingRepository = new BookingRepository(_context, _eventRepositoryMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        private Event CreateEvent(int eventId, int availableSeats)
        {
            return new Event { Id = eventId, AvailableSeats = availableSeats };
        }

        private Booking CreateBooking(string userName, int eventId, int numberOfTickets)
        {
            return new Booking
            {
                EventId = eventId,
                EventName = "Concert",
                UserName = userName,
                NumberOfTickets = numberOfTickets,
                BookingReference = $"BK-{new Random().Next(100000, 999999)}", // Generate BookingReference here
                BookingDate = DateTime.Now
            };
        }

        [Test]
        public async Task AddBookingAsync_ValidBooking_ReturnsTrue()
        {
            // Arrange
            var eventId = 1;
            var eventDetails = CreateEvent(eventId, 10);
            _eventRepositoryMock.Setup(repo => repo.GetEventByIdAsync(eventId)).ReturnsAsync(eventDetails);
            var booking = CreateBooking("user1", eventId, 5);

            // Act
            var result = await _bookingRepository.AddBookingAsync(booking);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(5, eventDetails.AvailableSeats);
        }

        [Test]
        public async Task AddBookingAsync_InsufficientSeats_ReturnsFalse()
        {
            // Arrange
            var eventId = 1;
            var eventDetails = CreateEvent(eventId, 3);
            _eventRepositoryMock.Setup(repo => repo.GetEventByIdAsync(eventId)).ReturnsAsync(eventDetails);
            var booking = CreateBooking("user1", eventId, 5);

            // Act
            var result = await _bookingRepository.AddBookingAsync(booking);

            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(3, eventDetails.AvailableSeats); // Ensure seats are unchanged
            Assert.AreEqual(0, _context.Bookings.Count());
        }

        [Test]
        public async Task DeleteBookingAsync_ValidBooking_ReturnsTrue()
        {
            // Arrange
            var eventId = 1; // Set a valid eventId
            var eventDetails = CreateEvent(eventId, 10);
            _eventRepositoryMock.Setup(repo => repo.GetEventByIdAsync(eventId)).ReturnsAsync(eventDetails);

            // Create a booking
            var booking = CreateBooking("user1", eventId, 2);

            // Use the repository method to add the booking
            await _bookingRepository.AddBookingAsync(booking);

            // Act
            var result = await _bookingRepository.DeleteBookingAsync(booking.Id); // Ensure we're using booking.Id

            // Assert
            Assert.IsTrue(result);
            Assert.IsNull(await _context.Bookings.FindAsync(booking.Id)); // Confirm the booking has been deleted
        }


        [Test]
        public async Task DeleteBookingAsync_InvalidBookingId_ReturnsFalse()
        {
            // Act
            var result = await _bookingRepository.DeleteBookingAsync(99);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public async Task GetAllBookingAsync_ReturnsAllBookings()
        {
            // Arrange
            var booking1 = CreateBooking("user1", 1, 2);
            var booking2 = CreateBooking("user2", 2, 3);

            await _context.Bookings.AddRangeAsync(booking1, booking2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _bookingRepository.GetAllBookingAsync();

            // Assert
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task GetBookingByIdAsync_ValidId_ReturnsBooking()
        {
            // Arrange
            var eventId = 1; // Make sure this ID matches an existing event
            var eventDetails = CreateEvent(eventId, 10); // Create event with available seats
            _eventRepositoryMock.Setup(repo => repo.GetEventByIdAsync(eventId)).ReturnsAsync(eventDetails);

            var booking = CreateBooking("user1", eventId, 2); // Create a booking object
            await _bookingRepository.AddBookingAsync(booking); // Add the booking to the repository

            // Act
            var result = await _bookingRepository.GetBookingByIdAsync(booking.Id); // Use booking.Id

            // Assert
            Assert.IsNotNull(result); // Ensure the result is not null
            Assert.AreEqual("user1", result.UserName); // Check that the user name matches
            Assert.AreEqual(eventId, result.EventId); // Ensure the event ID matches
        }

        [Test]
        public async Task GetBookingByIdAsync_InvalidId_ReturnsNull()
        {
            // Act
            var result = await _bookingRepository.GetBookingByIdAsync(99);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetBookingForUserAsync_ValidUserName_ReturnsBookings()
        {
            // Arrange
            var booking1 = CreateBooking("user1", 1, 2);
            var booking2 = CreateBooking("user1", 2, 3);

            await _context.Bookings.AddRangeAsync(booking1, booking2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _bookingRepository.GetBookingForUserAsync("user1");

            // Assert
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task GetBookingForUserAsync_InvalidUserName_ReturnsEmptyList()
        {
            // Act
            var result = await _bookingRepository.GetBookingForUserAsync("nonexistentuser");

            // Assert
            Assert.IsEmpty(result);
        }
    }
}
