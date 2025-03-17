using BusinessLayer.Interface;
using BusinessLayer.Validators;
using FluentValidation;
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
    public class AddressBookController : Controller
    {
        private readonly IAddBL _addBL;
        private readonly AddressBookEntryDTOValidator _validator;
        public AddressBookController(IAddBL addBL, AddressBookEntryDTOValidator validator)
        {
            _addBL = addBL;
            _validator = validator;
        }

        // GET /api/addressbook → Fetch all contacts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contact>>> GetContacts()
        {
            return Ok(await _addBL.GetAllContactsAsync());
        }

        // GET /api/addressbook/{id} → Get contact by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Contact>> GetContact(int id)
        {
            var contact = await _addBL.GetContactByIdAsync(id);
            if (contact == null)
                return NotFound(new { message = "Contact not found" });

            return Ok(contact);
        }

        // POST /api/addressbook → Add a new contact
        [HttpPost]
        public async Task<ActionResult<AddressBookEntryDTO>> CreateContact(AddressBookEntryDTO contactDto)
        {
            var validationResult = await _validator.ValidateAsync(contactDto);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var newContact = await _addBL.AddContactAsync(contactDto);
            return CreatedAtAction(nameof(GetContact), new { id = newContact.Id }, newContact);
        }

        // PUT /api/addressbook/{id} → Update contact
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContact(int id, AddressBookEntryDTO contactDto)
        {
            if (id != contactDto.Id)
                return BadRequest(new { message = "ID mismatch" });

            bool updated = await _addBL.UpdateContactAsync(contactDto);
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
