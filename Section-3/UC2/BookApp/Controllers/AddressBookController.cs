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

        // GET /api/addressbook → Fetch all contacts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AddressBookEntryDTO>>> GetContacts()
        {
            return Ok(await _addBL.GetAllContactsAsync());
        }

        // GET /api/addressbook/{id} → Get contact by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<AddressBookEntryDTO>> GetContact(int id)
        {
            var contact = await _addBL.GetContactByIdAsync(id);
            if (contact == null)
                return NotFound(new { message = "Contact not found" });

            return Ok(contact);
        }

        // POST: /api/addressbook → Add a new contact for the logged-in user
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

        // PUT /api/addressbook/{id} → Update contact
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

        // DELETE /api/addressbook/{id} → Delete contact
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
