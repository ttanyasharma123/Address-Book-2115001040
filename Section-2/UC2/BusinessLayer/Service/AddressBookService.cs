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
    public class AddressBookService : IAddressBookService
    {
        private readonly IAddRL _repository;
        private readonly IMapper _mapper;

        public AddressBookService(IAddRL repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<IEnumerable<AddressBookEntryDTO>> GetAllContactsAsync()
        {
            var contacts = await _repository.GetAllContactsAsync();
            return _mapper.Map<IEnumerable<AddressBookEntryDTO>>(contacts);
        }

        public async Task<AddressBookEntryDTO> GetContactByIdAsync(int id)
        {
            var contact = await _repository.GetContactByIdAsync(id);
            return contact != null ? _mapper.Map<AddressBookEntryDTO>(contact) : null;
        }

        public async Task<AddressBookEntryDTO> AddContactAsync(AddressBookEntryDTO contactDto)
        {
            var contact = _mapper.Map<Contact>(contactDto);
            var newContact = await _repository.AddContactAsync(contact);
            return _mapper.Map<AddressBookEntryDTO>(newContact);
        }

        public async Task<bool> UpdateContactAsync(int id, AddressBookEntryDTO contactDto)
        {
            var existingContact = await _repository.GetContactByIdAsync(id);
            if (existingContact == null) return false;

            _mapper.Map(contactDto, existingContact);
            return await _repository.UpdateContactAsync(existingContact);
        }

        public async Task<bool> DeleteContactAsync(int id)
        {
            return await _repository.DeleteContactAsync(id);
        }
    }
}
