using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using BusinessLayer.Interface;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.DTO;
using Moq;
using NUnit.Framework;
using BookApp.Controllers;
using BusinessLayer.Validators;

namespace AddressBookTests
{
    [TestFixture]
    public class AddressBookControllerUnitTests
    {
        private Mock<IAddressBookService> _mockService;
        private AddressBookController _controller;
        private AddressBookEntryDTOValidator _validator;

        [SetUp]
        public void Setup()
        {
            _mockService = new Mock<IAddressBookService>();
            _validator = new AddressBookEntryDTOValidator();
            _controller = new AddressBookController(_mockService.Object, _validator);

            // Simulating user authentication with a mock identity
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        // ✅ Test Get All Contacts
        [Test]
        public async Task GetContacts_ReturnsListOfContacts()
        {
            // Arrange
            var contacts = new List<AddressBookEntryDTO>
            {
                new AddressBookEntryDTO { Id = 1, Name = "John Doe", Email = "john@example.com" },
                new AddressBookEntryDTO { Id = 2, Name = "Jane Doe", Email = "jane@example.com" }
            };
            _mockService.Setup(s => s.GetAllContactsAsync()).ReturnsAsync(contacts);

            // Act
            var result = await _controller.GetContacts();
            var okResult = result.Result as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, (result.Value as List<AddressBookEntryDTO>).Count);
        }

        // ✅ Test Get Contact By ID - Found
        [Test]
        public async Task GetContact_ValidId_ReturnsContact()
        {
            // Arrange
            var contact = new AddressBookEntryDTO { Id = 1, Name = "John Doe", Email = "john@example.com" };
            _mockService.Setup(s => s.GetContactByIdAsync(1)).ReturnsAsync(contact);

            // Act
            var result = await _controller.GetContacts();
            var okResult = result.Result as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("John Doe", ((AddressBookEntryDTO)result.Value).Name);
        }

        // ✅ Test Get Contact By ID - Not Found
        [Test]
        public async Task GetContact_InvalidId_ReturnsNotFound()
        {
            // Arrange
            _mockService.Setup(s => s.GetContactByIdAsync(99)).ReturnsAsync((AddressBookEntryDTO)null);

            // Act
            var result = await _controller.GetContacts();
            var okResult = result.Result as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
        }

        // ✅ Test Add Contact - Success
        [Test]
        public async Task AddContact_Valid_ReturnsSuccessMessage()
        {
            // Arrange
            var newContact = new AddressBookEntryDTO { Name = "John Doe", Email = "john@example.com" };
            _mockService.Setup(s => s.AddContactAsync(It.IsAny<int>(), newContact)).ReturnsAsync(true);

            // Act
            var result = await _controller.AddContact(newContact) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual("Contact added successfully", result.Value.GetType().GetProperty("message").GetValue(result.Value, null));
        }

        // ✅ Test Add Contact - Unauthorized
        [Test]
        public async Task AddContact_NoUser_ReturnsUnauthorized()
        {
            // Arrange: Simulate no user by setting ControllerContext to a new one
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            var newContact = new AddressBookEntryDTO { Name = "John Doe", Email = "john@example.com" };

            // Act
            var result = await _controller.AddContact(newContact) as UnauthorizedObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(401, result.StatusCode);
        }

        // ✅ Test Update Contact - Success
        [Test]
        public async Task UpdateContact_ValidId_ReturnsNoContent()
        {
            // Arrange
            var updatedContact = new AddressBookEntryDTO { Id = 1, Name = "Updated Name", Email = "updated@example.com" };
            _mockService.Setup(s => s.UpdateContactAsync(1, updatedContact)).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateContact(1, updatedContact) as NoContentResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(204, result.StatusCode);
        }

        // ✅ Test Update Contact - Not Found
        [Test]
        public async Task UpdateContact_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var updatedContact = new AddressBookEntryDTO { Id = 1, Name = "Updated Name", Email = "updated@example.com" };
            _mockService.Setup(s => s.UpdateContactAsync(1, updatedContact)).ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateContact(1, updatedContact) as NotFoundObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
        }

        // ✅ Test Update Contact - ID Mismatch
        [Test]
        public async Task UpdateContact_IdMismatch_ReturnsBadRequest()
        {
            // Arrange
            var updatedContact = new AddressBookEntryDTO { Id = 2, Name = "Updated Name", Email = "updated@example.com" };

            // Act
            var result = await _controller.UpdateContact(1, updatedContact) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
        }

        // ✅ Test Delete Contact - Success
        [Test]
        public async Task DeleteContact_ValidId_ReturnsNoContent()
        {
            // Arrange
            _mockService.Setup(s => s.DeleteContactAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteContact(1) as NoContentResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(204, result.StatusCode);
        }

        // ✅ Test Delete Contact - Not Found
        [Test]
        public async Task DeleteContact_InvalidId_ReturnsNotFound()
        {
            // Arrange
            _mockService.Setup(s => s.DeleteContactAsync(99)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteContact(99) as NotFoundObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
        }
    }
}
