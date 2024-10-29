using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Mvc;
using TicketReservationAPI.Controllers;
using TicketReservationAPI.Models;
using TicketReservationAPI.Repository.IRepository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TicketReservationAPI.Tests.Controllers
{
    public class BookingControllerTests
    {
        private BookingController _controller;
        private Mock<IBookingRepository> _mockBookingRepository;
        private Mock<IEventRepository> _mockEventRepository;

        [SetUp]
        public void Setup()
        {
            _mockBookingRepository = new Mock<IBookingRepository>();
            _mockEventRepository = new Mock<IEventRepository>();
            _controller = new BookingController(_mockBookingRepository.Object, _mockEventRepository.Object);
        }

        [Test]
        public async Task BookTickets_ValidBooking_ReturnsOk()
        {
            // Arrange
            var booking = new Booking { Id = 1, EventId = 1, NumberOfTickets = 2 };
            _mockBookingRepository.Setup(repo => repo.AddBookingAsync(booking)).ReturnsAsync(true);

            // Act
            var result = await _controller.BookTickets(booking);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.AreEqual("Tickets booked successfully.", ((OkObjectResult)result).Value);
        }

        [Test]
        public async Task CancelBooking_BookingNotFound_ReturnsNotFound()
        {
            // Arrange
            int bookingId = 1;
            _mockBookingRepository.Setup(repo => repo.GetBookingByIdAsync(bookingId)).ReturnsAsync((Booking)null);

            // Act
            var result = await _controller.CancelBooking(bookingId);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            Assert.AreEqual("Booking not found", ((NotFoundObjectResult)result).Value);
        }

        [Test]
        public async Task GetBookingForUser_UserHasBookings_ReturnsOkWithBookings()
        {
            // Arrange
            string userName = "testuser";
            var bookings = new List<Booking> { new Booking { UserName = userName, EventId = 1, NumberOfTickets = 2 } };
            _mockBookingRepository.Setup(repo => repo.GetBookingForUserAsync(userName)).ReturnsAsync(bookings);

            // Act
            var result = await _controller.GetBookingForUser(userName);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.AreEqual(bookings, ((OkObjectResult)result).Value);
        }

        [Test]
        public async Task GetAllBooking_ReturnsOkWithAllBookings()
        {
            // Arrange
            var bookings = new List<Booking> { new Booking { Id = 1, EventId = 1, NumberOfTickets = 2 } };
            _mockBookingRepository.Setup(repo => repo.GetAllBookingAsync()).ReturnsAsync(bookings);

            // Act
            var result = await _controller.GetAllBooking();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.AreEqual(bookings, ((OkObjectResult)result).Value);
        }
    }
}
