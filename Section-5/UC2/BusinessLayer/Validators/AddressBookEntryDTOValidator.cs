using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using ModelLayer.DTO;

namespace BusinessLayer.Validators
{
    public class AddressBookEntryDTOValidator : AbstractValidator<AddressBookEntryDTO>
    {
        public AddressBookEntryDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(50).WithMessage("Name must be under 50 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone is required")
                .Matches(@"^\d{10}$").WithMessage("Phone must be a 10-digit number");
        }
    }
}
