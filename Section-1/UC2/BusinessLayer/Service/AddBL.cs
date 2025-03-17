using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Interface;
using ModelLayer.Model.Entities;
using RepositoryLayer.Interface;

namespace BusinessLayer.Service
{
    public class AddBL : IAddBL
    {
        private readonly IAddRL _addRL;
        public AddBL(IAddRL addRL)
        {
            _addRL = addRL;
        }

        public async Task<IEnumerable<Contact>> GetAllContactsAsync()
        {
            return await _addRL.GetAllContactsAsync();
        }

        public async Task<Contact> GetContactByIdAsync(int id)
        {
            return await _addRL.GetContactByIdAsync(id);
        }

        public async Task<Contact> AddContactAsync(Contact contact)
        {
            return await _addRL.AddContactAsync(contact);
        }

        public async Task<bool> UpdateContactAsync(Contact contact)
        {
            return await _addRL.UpdateContactAsync(contact);
        }

        public async Task<bool> DeleteContactAsync(int id)
        {
            return await _addRL.DeleteContactAsync(id);
        }
    }
}
