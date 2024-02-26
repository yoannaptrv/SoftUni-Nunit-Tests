using GetGreeting;
using Moq;

namespace GetGreeting.Tests
{
    public class GreetingProviderTests
    {
        private GreetingProvider _greetingProvider;
        private Mock<ITimeProvider> _mockedTimeProvider;

        [SetUp]
        public void Setup()
        {
            _mockedTimeProvider = new Mock<ITimeProvider>();
            _greetingProvider = new GreetingProvider(_mockedTimeProvider.Object);
        }

        [Test]
        public void GetGreeting_shouldReturnMorningMessage_WhenItsMorning()
        {
            _mockedTimeProvider.Setup(x => x.GetCurrentTime()).Returns(new DateTime(2000, 2, 2, 9, 9, 9));

            var result = _greetingProvider.GetGreeting();

            Assert.AreEqual("Good morning!", result);
        }
        [Test]
        public void GetGreeting_shouldReturnAfternoonMessage_whenItsAfternoon()
        {
            _mockedTimeProvider.Setup(x => x.GetCurrentTime()).Returns(new DateTime(2000, 2, 2, 13, 13, 13));

            var result = _greetingProvider.GetGreeting();

            Assert.That(result, Is.EqualTo("Good afternoon!"));
        }
        [Test]
        public void GetGreeting_shouldReturnEveningMessage_whenItsEvening()
        {
            _mockedTimeProvider.Setup(x => x.GetCurrentTime()).Returns(new DateTime(2000, 2, 2, 19, 19, 19));

            var result = _greetingProvider.GetGreeting();

            Assert.That(result, Is.EqualTo("Good evening!"));
        }
        [Test]
        public void GetGreeting_shouldReturnNightMessage_whenItsNight()
        {
            _mockedTimeProvider.Setup(x => x.GetCurrentTime()).Returns(new DateTime(2000, 2, 2, 4, 4, 19));

            var result = _greetingProvider.GetGreeting();

            Assert.That(result, Is.EqualTo("Good night!"));
        }
        [TestCase("Good morning!", 8)]
        [TestCase("Good afternoon!", 13)]
        [TestCase("Good evening!", 19)]
        [TestCase("Good night!", 4)]
        public void GetGreeting_shouldReturnCorrectMessage_whenTimeIsCorrect(string expectedMessage, int currentHour)
        {
            _mockedTimeProvider.Setup(x => x.GetCurrentTime()).Returns(new DateTime(2000, 2, 2, currentHour, 4, 19));

            var result = _greetingProvider.GetGreeting();

            Assert.That(result, Is.EqualTo(expectedMessage));
        }
    }
}