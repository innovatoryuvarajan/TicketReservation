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

        [Test]
        public async Task AddBookingAsync_ValidBooking_ReturnsTrue()
        {
            // Arrange
            var eventId = 1;
            var eventDetails = new Event { Id = eventId, AvailableSeats = 10 };
            _eventRepositoryMock.Setup(repo => repo.GetEventByIdAsync(eventId)).ReturnsAsync(eventDetails);
            var booking = new Booking
            {
                EventId = eventId,
                EventName = "Concert",
                UserName = "user1",
                NumberOfTickets = 5
            };

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
            var eventDetails = new Event { Id = eventId, AvailableSeats = 3 };
            _eventRepositoryMock.Setup(repo => repo.GetEventByIdAsync(eventId)).ReturnsAsync(eventDetails);
            var booking = new Booking
            {
                EventId = eventId,
                EventName = "Concert",
                UserName = "user1",
                NumberOfTickets = 5
            };

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
            var booking = new Booking
            {
                EventId = 1,
                EventName = "Concert",
                UserName = "user1",
                NumberOfTickets = 2
            };

            // Mock event repository to ensure there are enough seats available
            var eventDetails = new Event { Id = booking.EventId, AvailableSeats = 10 };
            _eventRepositoryMock.Setup(repo => repo.GetEventByIdAsync(booking.EventId)).ReturnsAsync(eventDetails);

            // Use the repository method to add the booking
            await _bookingRepository.AddBookingAsync(booking);

            // Act
            var result = await _bookingRepository.DeleteBookingAsync(booking.Id);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNull(await _context.Bookings.FindAsync(booking.Id));
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
            var booking1 = new Booking
            {
                EventId = 1,
                EventName = "Concert A",
                UserName = "user1",
                NumberOfTickets = 2,
                BookingDate = DateTime.Now,
                BookingReference = "BK-123456"
            };
            var booking2 = new Booking
            {
                EventId = 2,
                EventName = "Concert B",
                UserName = "user2",
                NumberOfTickets = 3,
                BookingDate = DateTime.Now,
                BookingReference = "BK-654321"
            };

            await _context.Bookings.AddRangeAsync(booking1, booking2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _bookingRepository.GetAllBookingAsync();

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.Any(b => b.BookingReference == "BK-123456"));
            Assert.IsTrue(result.Any(b => b.BookingReference == "BK-654321"));
        }

        [Test]
        public async Task GetBookingByIdAsync_ValidId_ReturnsBooking()
        {
            // Arrange
            var booking = new Booking
            {
                Id = 1,
                EventId = 1,
                EventName = "Concert A",
                UserName = "user1",
                NumberOfTickets = 2,
                BookingDate = DateTime.Now,
                BookingReference = "BK-123456"
            };

            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();

            // Act
            var result = await _bookingRepository.GetBookingByIdAsync(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Concert A", result.EventName);
            Assert.AreEqual("user1", result.UserName);
            Assert.AreEqual("BK-123456", result.BookingReference);
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
            var booking1 = new Booking
            {
                EventId = 1,
                EventName = "Concert A",
                UserName = "user1",
                NumberOfTickets = 2,
                BookingDate = DateTime.Now,
                BookingReference = "BK-123456"
            };
            var booking2 = new Booking
            {
                EventId = 2,
                EventName = "Concert B",
                UserName = "user1",
                NumberOfTickets = 3,
                BookingDate = DateTime.Now,
                BookingReference = "BK-654321"
            };

            await _context.Bookings.AddRangeAsync(booking1, booking2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _bookingRepository.GetBookingForUserAsync("user1");

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.Any(b => b.BookingReference == "BK-123456"));
            Assert.IsTrue(result.Any(b => b.BookingReference == "BK-654321"));
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
