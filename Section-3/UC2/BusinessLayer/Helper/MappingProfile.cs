using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ModelLayer.DTO;
using ModelLayer.Model.Entities;

namespace BusinessLayer.Helper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Contact, AddressBookEntryDTO>().ReverseMap();
        }
    }
}
