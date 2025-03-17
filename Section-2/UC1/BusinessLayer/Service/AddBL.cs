using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BusinessLayer.Interface;
using ModelLayer.DTO;
using ModelLayer.Model.Entities;
using RepositoryLayer.Interface;

namespace BusinessLayer.Service
{
    public class AddBL : IAddBL
    {
        private readonly IAddRL _addRL;
        private readonly IMapper _mapper;
        public AddBL(IAddRL addRL, IMapper mapper)
        {
            _addRL = addRL;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Contact>> GetAllContactsAsync()
        {
            var contacts = await _addRL.GetAllContactsAsync();
            return _mapper.Map<IEnumerable<Contact>>(contacts);
        }

        public async Task<Contact> GetContactByIdAsync(int id)
        {
            var contact = await _addRL.GetContactByIdAsync(id);
            return _mapper.Map<Contact>(contact);
        }

        public async Task<AddressBookEntryDTO> AddContactAsync(AddressBookEntryDTO contactDto)
        {
            var contact = _mapper.Map<Contact>(contactDto);
            var newContact = await _addRL.AddContactAsync(contact);
            return _mapper.Map<AddressBookEntryDTO>(newContact);
        }

        public async Task<bool> UpdateContactAsync(AddressBookEntryDTO contactDto)
        {
            var contact = _mapper.Map<Contact>(contactDto);
            return await _addRL.UpdateContactAsync(contact);
        }

        public async Task<bool> DeleteContactAsync(int id)
        {
            return await _addRL.DeleteContactAsync(id);
        }
    }
}
