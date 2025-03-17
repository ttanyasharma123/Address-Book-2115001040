using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.DTO;

namespace BusinessLayer.Interface
{
    public interface IAddressBookService
    {
        Task<IEnumerable<AddressBookEntryDTO>> GetAllContactsAsync();
        Task<AddressBookEntryDTO> GetContactByIdAsync(int id);
        Task<AddressBookEntryDTO> AddContactAsync(AddressBookEntryDTO contactDto);
        Task<bool> UpdateContactAsync(int id, AddressBookEntryDTO contactDto);
        Task<bool> DeleteContactAsync(int id);
    }
}
