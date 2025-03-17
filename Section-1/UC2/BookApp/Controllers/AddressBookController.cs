using BusinessLayer.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Model.Entities;
using NUnit.Framework;

namespace BookApp.Controllers
{
    [TestFixture]
    [Route("api/[controller]")]
    [ApiController]
    public class AddressBookController : Controller
    {
        public IAddBL _addBL;
        public AddressBookController(IAddBL addBL)
        {
            _addBL = addBL;
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
        public async Task<ActionResult<Contact>> CreateContact(Contact contact)
        {
            var newContact = await _addBL.AddContactAsync(contact);
            return CreatedAtAction(nameof(GetContact), new { id = newContact.Id }, newContact);
        }

        // PUT /api/addressbook/{id} → Update contact
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContact(int id, Contact contact)
        {
            if (id != contact.Id)
                return BadRequest(new { message = "ID mismatch" });

            bool updated = await _addBL.UpdateContactAsync(contact);
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
