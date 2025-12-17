using FluentValidation;
using ShuppeMarket.Application.DTOs.SellerDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShuppeMarket.Application.Validation
{
    public class SellerRequestValidation : AbstractValidator<SellerRequest>
    {
        public SellerRequestValidation()
        {
            RuleFor(x => x.ShopName)
                .NotEmpty().WithMessage("Store name is required.")
                .MaximumLength(100).WithMessage("Store name cannot exceed 100 characters.");
            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^(0|\+84)[0-9]{9}$").WithMessage("Invalid phone number format.");
            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Store address is required.");
        }
    }
}
