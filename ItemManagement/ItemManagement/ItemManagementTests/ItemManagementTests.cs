using NUnit.Framework;
using Moq;
using ItemManagementApp.Services;
using ItemManagementLib.Repositories;
using ItemManagementLib.Models;
using System.Collections.Generic;
using System.Linq;

namespace ItemManagement.Tests
{
    [TestFixture]
    public class ItemServiceTests
    {
        private ItemService _itemService;
        private Mock<IItemRepository> _mockItemRepository;
        

        [SetUp]
        public void Setup()
        {
            // Arrange: Create a mock instance of IItemRepository
            _mockItemRepository = new Mock<IItemRepository>();

            // Instantiate ItemService with the mocked repository
            _itemService = new ItemService(_mockItemRepository.Object);
        }

        [Test]
        public void AddItem_ShouldCallAddItemOnRepository()
        {
            // Act: Call AddItem on the service
            

            // Assert: Verify that AddItem was called on the repository
            
        }
        [Test]
        public void GetItemById_ShouldReturnItemById()
        {
            var item = new Item { Id = 1, Name = "Test" };
            _mockItemRepository.Setup(x => x.GetItemById(item.Id)).Returns(item);

            var result = _itemService.GetItemById(item.Id);

            Assert.NotNull(result);
            Assert.That(result.Name, Is.EqualTo(item.Name));
            _mockItemRepository.Verify(x => x.GetItemById(item.Id), Times.Once());
        }

        [Test]
        public void GetAllItems_ShouldReturnAllItems()
        {
            var items = new List<Item>() { new Item { Id = 1, Name = "Sample," } };
            _mockItemRepository.Setup(x => x.GetAllItems()).Returns(items);

            var result = _itemService.GetAllItems();

            Assert.NotNull(result);
            Assert.That(result.Count(), Is.EqualTo(1));
            _mockItemRepository.Verify(x => x.GetAllItems(), Times.Once());
            
        }

        [Test]
        public void GetItem_ShouldReturnNull_IfItemDoesNotExists()
        {
            Item item = null;
            _mockItemRepository.Setup(x => x.GetItemById(It.IsAny<int>())).Returns(item);

            var result = _itemService.GetItemById(123);

            Assert.Null(result);
            _mockItemRepository.Verify(x => x.GetItemById(It.IsAny<int>()), Times.Once());

        }
        [Test]
        public void AddItem_ShouldAddItem()
        {
            var item = new Item { Name = "Test" };
            _mockItemRepository.Setup(x => x.AddItem(It.IsAny<Item>()));

            _itemService.AddItem(item.Name);

            _mockItemRepository.Verify(x => x.AddItem(It.IsAny<Item>()), Times.Once());

        }
        [Test]
        public void AddItem_ShouldThrowError()
        {
            var item = "";
            _mockItemRepository.Setup(x => x.AddItem(It.IsAny<Item>())).Throws<ArgumentException>();

            Assert.Throws<ArgumentException>(() => _itemService.AddItem(item));


            _mockItemRepository.Verify(x => x.AddItem(It.IsAny<Item>()), Times.Once());

        }
        [Test]
        public void UpdateItem_ShouldNotUpdateItem_IfItemDoesNotExcist()
        {
            var invaliditem = 1;
            _mockItemRepository.Setup(x => x.GetItemById(invaliditem)).Returns<Item>(null);
            _mockItemRepository.Setup(x => x.UpdateItem(It.IsAny<Item>()));

            _itemService.UpdateItem(invaliditem, "DoesNotMatter");

            _mockItemRepository.Verify(x => x.GetItemById(invaliditem), Times.Once());
            _mockItemRepository.Verify(x => x.UpdateItem(It.IsAny<Item>()), Times.Never);

        }
        [Test]
        public void UpdateItem_ShouldThrowException_IfItemNameIsInvalid()
        {
            var item = new Item { Name = "Sample", Id = 1};
            _mockItemRepository.Setup(x => x.GetItemById(item.Id)).Returns(item);
            _mockItemRepository.Setup(x => x.UpdateItem(It.IsAny<Item>())).Throws<ArgumentException>();

            Assert.Throws<ArgumentException>(() => _itemService.UpdateItem(item.Id, ""));

            _mockItemRepository.Verify(x => x.GetItemById(item.Id), Times.Once());
            _mockItemRepository.Verify(x => x.UpdateItem(It.IsAny<Item>()), Times.Once());

        }
        [Test]
        public void UpdateItem_ShouldUpdateItem_IfItemNameIsValid()
        {
            var item = new Item { Name = "Sample", Id = 1 };
            _mockItemRepository.Setup(x => x.GetItemById(item.Id)).Returns(item);
            _mockItemRepository.Setup(x => x.UpdateItem(It.IsAny<Item>()));

            _itemService.UpdateItem(item.Id, "Sample Updated");

            _mockItemRepository.Verify(x => x.GetItemById(item.Id), Times.Once());
            _mockItemRepository.Verify(x => x.UpdateItem(It.IsAny<Item>()), Times.Once());

        }
        [Test]
        public void DeleteItem_ShouldDeleteItem()
        {
            var item = 12;
            _mockItemRepository.Setup(x => x.DeleteItem(item));


            _itemService.DeleteItem(item);

            _mockItemRepository.Verify(x => x.DeleteItem(item), Times.Once());
        }
        [TestCase("", false)]
        [TestCase("", false)]
        [TestCase(null, false)]
        [TestCase("aaaaaaaaaaaaaa", false)]
        [TestCase("SampleName", false)]
        public void ValidateItemName_ShouldReturnCorrect(string name, bool isValid)
        {
            var item = "Sample";

            var result = _itemService.ValidateItemName(item);

            Assert.That(result, Is.EqualTo(isValid));
        }
    }
}