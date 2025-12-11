using FluentValidation;
using ShuppeMarket.Application.DTOs.AccountDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShuppeMarket.Application.Validation
{
    public class AccountValidation : AbstractValidator<AccountRequest>
    {
        public AccountValidation()
        {
            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
            RuleFor(x => x.Password)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(5).WithMessage("Password must be at least 5 characters long.");
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required.");
            RuleFor(x => x.PhoneNumber)
                .Matches(@"^(0|\+84)[0-9]{9}$").WithMessage("Invalid phone number format.")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber));




        }
    }
}
