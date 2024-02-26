using Homies.Data;
using Homies.Data.Models;
using Homies.Models.Event;
using Homies.Services;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Homies.Tests
{
    [TestFixture]
    internal class EventServiceTests
    {
        private HomiesDbContext _dbContext;
        private EventService _eventService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<HomiesDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Use unique database name to avoid conflicts
                .Options;
            _dbContext = new HomiesDbContext(options);

            _eventService = new EventService(_dbContext);
        }

        [Test]
        public async Task AddEventAsync_ShouldAddEvent_WhenValidEventModelAndUserId()
        {
            var eventModel = new EventFormModel
            {
                Name = "Test Event",
                Description = "Test Description",
                Start = DateTime.Now,
                End = DateTime.Now.AddHours(2)
            };
            string userId = "testUserId";

            await _eventService.AddEventAsync(eventModel, userId);

            var eventIntDataBase = _dbContext.Events.FirstOrDefaultAsync(x => x.Name == eventModel.Name && x.OrganiserId == userId);

            Assert.IsNotNull(eventIntDataBase);



        }


        [Test]
        public async Task GetAllEventsAsync_ShouldReturnAllEvents()
        {
            var firstEventModel = new EventFormModel
            {
                Name = "Test Event",
                Description = "Second Test Description",
                Start = DateTime.Now,
                End = DateTime.Now.AddHours(2)
            };
            var secondEventModel = new EventFormModel
            {
                Name = "Test Event",
                Description = "Second Test Description",
                Start = DateTime.Now.AddDays(2),
                End = DateTime.Now.AddDays(3)
            };

            string userId = "testUserId";

            await _eventService.AddEventAsync(firstEventModel, userId);
            await _eventService.AddEventAsync(secondEventModel, userId);

            var result = await _eventService.GetAllEventsAsync();
            Assert.That(result.Count, Is.EqualTo(2));
        }
        [Test]
        public async Task GetEventDatailsAsync_ShouldReturnAllEventDetails()
        {
            var firstEventModel = new EventFormModel
            {
                Name = "Test Event",
                Description = "Second Test Description",
                Start = DateTime.Now,
                End = DateTime.Now.AddHours(2),
                TypeId = 2,
            };

            await _eventService.AddEventAsync(firstEventModel, "nnExisting");

            var events = _dbContext.Events.ToListAsync();

            var result = await _eventService.GetEventDetailsAsync(events.Id);

            Assert.IsNotNull(result);
            Assert.That(result.Name, Is.EqualTo(firstEventModel.Name));
            Assert.That(result.Description, Is.EqualTo(firstEventModel.Description));
        }
        [Test]
        public async Task GetEventForEditAsync_ShouldGetEventIfPesentInTheDb()
        {
            var firstEventModel = new EventFormModel
            {
                Name = "Test Event",
                Description = "Second Test Description",
                Start = DateTime.Now,
                End = DateTime.Now.AddHours(2),
                TypeId = 2,
            };

            await _eventService.AddEventAsync(firstEventModel, "nnExisting");

            var events = await _dbContext.Events.FirstAsync();
            var result = await _eventService.GetEventForEditAsync(events.Id);

            Assert.IsNotNull(result);
            Assert.That(result.Name, Is.EqualTo(firstEventModel.Name));
        }

        [Test]
        public async Task GetEventForEditAsync_ShouldReturnNullIfEventIsNotFound()
        {
            var result = await _eventService.GetEventForEditAsync(90);

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetEventOrganiserId_SHouldReturnOrganserId()
        {
            var firstEventModel = new EventFormModel
            {
                Name = "Test Event",
                Description = "Second Test Description",
                Start = DateTime.Now,
                End = DateTime.Now.AddHours(2),
                TypeId = 2,
            };
            const string userId = "userId";

            await _eventService.AddEventAsync(firstEventModel, userId);

            var events = await _dbContext.Events.FirstAsync();

            var result = await _eventService.GetEventOrganizerIdAsync(events.Id);

            Assert.IsNotNull(result);
            Assert.That(result, Is.EqualTo(userId));
        }

        [Test]
        public async Task GetUserJoinedAsync_SHouldReturnAllUsers()
        {
            const string userId = "userId";
            var testType = new Data.Models.Type
            {
                Name = "TestType",
            };
            _dbContext.Types.Add(testType);
            await _dbContext.SaveChangesAsync();
            var testEvent = new Event
            {
                Name = "Test Event",
                Description = "Second Test Description",
                Start = DateTime.Now,
                End = DateTime.Now.AddHours(2),
                TypeId = testType.Id,
                OrganiserId = userId,
            };
            _dbContext.Events.Add(testEvent);

            var allEvents = _dbContext.Events.ToList();
            _dbContext.EventsParticipants.Add(new EventParticipant()
            {
                EventId = testEvent.Id,
                HelperId = userId,
            });

            var result = await _eventService.GetUserJoinedEventsAsync(userId);
            Assert.IsNotNull(result);
            Assert.That(result.Count(), Is.EqualTo(1));

            var eventParticip = result.First();
            Assert.That(eventParticip.Id, Is.EqualTo(testEvent.Id));
            Assert.That(eventParticip.Name, Is.EqualTo(testEvent.Name));
            Assert.That(eventParticip.Type, Is.EqualTo(testEvent.Name));
        }
        [Test]
        public async Task JoinEventAsync_ShouldReturnFalseIfEventDoesNotExist()
        {
            var result = await _eventService.JoinEventAsync(99, "");

            Assert.False(result);
        }
        [Test]
        public async Task JoinEventAsync_ShouldReturnFalse_IfUserAllreadyExsist()
        {
            const string userId = "userId";
            var testType = new Data.Models.Type
            {
                Name = "TestType",
            };
            _dbContext.Types.Add(testType);
            await _dbContext.SaveChangesAsync();
            var testEvent = new Event
            {
                Name = "Test Event",
                Description = "Second Test Description",
                Start = DateTime.Now,
                End = DateTime.Now.AddHours(2),
                TypeId = testType.Id,
                OrganiserId = userId,
            };
            await _dbContext.Events.AddAsync(testEvent);
            await _dbContext.SaveChangesAsync();

            await _dbContext.EventsParticipants.AddAsync(new EventParticipant()
            {
                EventId = testEvent.Id,
                HelperId = userId,
            });
            await _dbContext.SaveChangesAsync();

            var result = await _eventService.JoinEventAsync(testEvent.Id, userId);

            Assert.False(result);
        }
        [Test]
        public async Task JoinEvtn_ShouldReturnTrue_IfUserIsOkay()
        {
            const string userId = "userId";
            var testType = new Data.Models.Type
            {
                Name = "TestType",
            };
            _dbContext.Types.Add(testType);
            await _dbContext.SaveChangesAsync();
            var testEvent = new Event
            {
                Name = "Test Event",
                Description = "Second Test Description",
                Start = DateTime.Now,
                End = DateTime.Now.AddHours(2),
                TypeId = testType.Id,
                OrganiserId = userId,
            };
            await _dbContext.Events.AddAsync(testEvent);
            await _dbContext.SaveChangesAsync();

            var result = await _eventService.JoinEventAsync(testEvent.Id, userId);

            Assert.True(result);

        }
        [Test] 
        public async Task LeaveEventAsync_SHouldReturnFalse_IfWeHaventJoinedIt()
        {
            const string userId = "user";

            var result = await _eventService.LeaveEventAsync(123, userId);

            Assert.False(result);
        }
        [Test]
        public async Task LeaveEventAsync_SHouldReturnTrue_IfWeJoinedIt()
        {
            const string userId = "userId";
            var testType = new Data.Models.Type
            {
                Name = "TestType",
            };
            _dbContext.Types.Add(testType);
            await _dbContext.SaveChangesAsync();
            var testEvent = new Event
            {
                Name = "Test Event",
                Description = "Second Test Description",
                Start = DateTime.Now,
                End = DateTime.Now.AddHours(2),
                TypeId = testType.Id,
                OrganiserId = userId,
            };
            await _dbContext.Events.AddAsync(testEvent);
            await _dbContext.SaveChangesAsync();

            string userIds = "new";
            await _eventService.JoinEventAsync(testEvent.Id, userIds);

            var result = await _eventService.LeaveEventAsync(testEvent.Id, userIds);

        }
        [Test]
        public async Task UpdateEventAsync_ShouldReturnFalse_IfEventDoesNOtExsist()
        {
            var result = await _eventService.UpdateEventAsync(999, new EventFormModel { }, "user");

            Assert.False(result);
        }
        
        [Test]
        public async Task GetAllTypesAsync_ShouldReturnTrue_IfWeUpdate()
        {
            var testType = new Data.Models.Type
            {
                Name = "TestType"
            };
            await _dbContext.Types.AddAsync(testType);
            await _dbContext.SaveChangesAsync();

            var result = await _eventService.GetAllTypesAsync();

            Assert.That(result.Count, Is.EqualTo(1));
            var singleType = result.First();
            Assert.That(singleType.Name, Is.EqualTo(testType.Name));
        }

        [Test]
        public async Task IsUserJoinedEventAsync_ShouldReturnFalseIfEvenetDoesNotExist()
        {
            var result = await _eventService.IsUserJoinedEventAsync(999, "asd");

            Assert.False(result);
        }
    }
}
