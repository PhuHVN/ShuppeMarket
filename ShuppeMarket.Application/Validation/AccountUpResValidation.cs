using FluentValidation;
using ShuppeMarket.Application.DTOs.AccountDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShuppeMarket.Application.Validation
{
    public class AccountUpResValidation: AbstractValidator<AccountUpdateRequest>
    {
        public AccountUpResValidation()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required.");
            RuleFor(x => x.PhoneNumber)
                .Matches(@"^(0|\+84)[0-9]{9}$").WithMessage("Invalid phone number format.")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber));
            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is required.");
        }
    }
}
