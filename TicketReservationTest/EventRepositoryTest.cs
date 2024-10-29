using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TicketReservationAPI.Data;
using TicketReservationAPI.Models;
using TicketReservationAPI.Repository;

namespace TicketReservationAPI.Tests.Repository
{
    public class EventRepositoryTests
    {
        private EventRepository _repository;
        private TicketDbContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<TicketDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new TicketDbContext(options);
            _repository = new EventRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task AddEventAsync_ValidEvent_ReturnsTrue()
        {
            var eventDetails = new Event
            {
                Id = 1,
                EventName = "Concert",
                EventDate = DateTime.UtcNow,
                Venue = "Main Hall",
                TotalSeats = 200,
                AvailableSeats = 200
            };

            var result = await _repository.AddEventAsync(eventDetails);

            Assert.IsTrue(result);
            var addedEvent = await _context.Events.FindAsync(1);
            Assert.IsNotNull(addedEvent);
            Assert.AreEqual("Concert", addedEvent.EventName);
        }

        [Test]
        public void AddEventAsync_NullEvent_ThrowsArgumentNullException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _repository.AddEventAsync(null));
        }

        [Test]
        public async Task DeleteEventAsync_EventExists_ReturnsTrue()
        {
            var eventDetails = new Event
            {
                Id = 1,
                EventName = "Concert",
                EventDate = DateTime.UtcNow,
                Venue = "Main Hall",
                TotalSeats = 200,
                AvailableSeats = 200
            };
            _context.Events.Add(eventDetails);
            await _context.SaveChangesAsync();

            var result = await _repository.DeleteEventAsync(1);

            Assert.IsTrue(result);
            Assert.IsNull(await _context.Events.FindAsync(1));
        }

        [Test]
        public async Task DeleteEventAsync_EventDoesNotExist_ReturnsFalse()
        {
            var result = await _repository.DeleteEventAsync(1);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task GetAllEventsAsync_ReturnsEventList()
        {
            var events = new List<Event>
            {
                new Event { Id = 1, EventName = "Concert", EventDate = DateTime.UtcNow, Venue = "Main Hall", TotalSeats = 200, AvailableSeats = 200 },
                new Event { Id = 2, EventName = "Conference", EventDate = DateTime.UtcNow.AddDays(1), Venue = "Conference Room", TotalSeats = 100, AvailableSeats = 100 }
            };
            await _context.Events.AddRangeAsync(events);
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllEventsAsync();

            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("Concert", result.First().EventName);
        }

        [Test]
        public async Task GetEventByIdAsync_EventExists_ReturnsEvent()
        {
            var eventDetails = new Event
            {
                Id = 1,
                EventName = "Concert",
                EventDate = DateTime.UtcNow,
                Venue = "Main Hall",
                TotalSeats = 200,
                AvailableSeats = 200
            };
            _context.Events.Add(eventDetails);
            await _context.SaveChangesAsync();

            var result = await _repository.GetEventByIdAsync(1);

            Assert.IsNotNull(result);
            Assert.AreEqual("Concert", result.EventName);
        }

        [Test]
        public async Task GetEventByIdAsync_EventDoesNotExist_ReturnsNull()
        {
            var result = await _repository.GetEventByIdAsync(1);

            Assert.IsNull(result);
        }

        [Test]
        public async Task UpdateEventAsync_EventExists_ReturnsTrue()
        {
            var eventDetails = new Event
            {
                Id = 1,
                EventName = "Concert",
                EventDate = DateTime.UtcNow,
                Venue = "Main Hall",
                TotalSeats = 200,
                AvailableSeats = 100
            };
            _context.Events.Add(eventDetails);
            await _context.SaveChangesAsync();

            eventDetails.EventName = "Updated Concert";
            eventDetails.AvailableSeats = 80;
            var result = await _repository.UpdateEventAsync(eventDetails);

            Assert.IsTrue(result);
            var updatedEvent = await _context.Events.FindAsync(1);
            Assert.AreEqual("Updated Concert", updatedEvent.EventName);
            Assert.AreEqual(80, updatedEvent.AvailableSeats);
        }

        [Test]
        public async Task UpdateEventAsync_EventDoesNotExist_ReturnsFalse()
        {
            var eventDetails = new Event { Id = 1, EventName = "Nonexistent Event", EventDate = DateTime.UtcNow, Venue = "Hall", TotalSeats = 50, AvailableSeats = 50 };
            var result = await _repository.UpdateEventAsync(eventDetails);

            Assert.IsFalse(result);
        }

        [Test]
        public void UpdateEventAsync_NullEvent_ThrowsArgumentNullException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _repository.UpdateEventAsync(null));
        }
    }
}
