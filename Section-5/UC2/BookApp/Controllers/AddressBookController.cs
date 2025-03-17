using System.Security.Claims;
using BusinessLayer.Interface;
using BusinessLayer.Service;
using BusinessLayer.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.DTO;
using ModelLayer.Model.Entities;
using NUnit.Framework;

namespace BookApp.Controllers
{
    [TestFixture]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AddressBookController : Controller
    {
        //private readonly IAddBL _addBL;
        private readonly IAddressBookService _addBL;
        private readonly AddressBookEntryDTOValidator _validator;
        public AddressBookController(IAddressBookService addBL, AddressBookEntryDTOValidator validator)
        {
            _addBL = addBL;
            _validator = validator;
        }

        /// <summary>
        /// Fetch all contacts.
        /// </summary>
        // GET /api/addressbook
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AddressBookEntryDTO>>> GetContacts()
        {
            return Ok(await _addBL.GetAllContactsAsync());
        }
        /// <summary>
        /// Get contact by ID
        /// </summary>
        // GET /api/addressbook/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<AddressBookEntryDTO>> GetContact(int id)
        {
            var contact = await _addBL.GetContactByIdAsync(id);
            if (contact == null)
                return NotFound(new { message = "Contact not found" });

            return Ok(contact);
        }
        /// <summary>
        /// Add a new contact for the logged-in user
        /// </summary>
        // POST: /api/addressbook
        [HttpPost]
        public async Task<IActionResult> AddContact(AddressBookEntryDTO contactDto)
        {
            // Extract UserId from JWT Token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "Invalid user. Please login." });
            }

            int userId = int.Parse(userIdClaim.Value); // Convert UserId to integer

            var result = await _addBL.AddContactAsync(userId, contactDto);

            if (!result)
            {
                return BadRequest(new { message = "Error saving contact" });
            }

            return Ok(new { message = "Contact added successfully" });
        }
        /// <summary>
        /// Update contact
        /// </summary>
        // PUT /api/addressbook/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContact(int id, AddressBookEntryDTO contactDto)
        {
            if (id != contactDto.Id)
                return BadRequest(new { message = "ID mismatch" });

            bool updated = await _addBL.UpdateContactAsync(id, contactDto);
            if (!updated)
                return NotFound(new { message = "Contact not found" });

            return NoContent();
        }
        /// <summary>
        /// Delete contact
        /// </summary>
        // DELETE /api/addressbook/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContact(int id)
        {
            bool deleted = await _addBL.DeleteContactAsync(id);
            if (!deleted)
                return NotFound(new { message = "Contact not found" });

            return NoContent();
        }
    }
}
