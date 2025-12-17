using FluentValidation;
using ShuppeMarket.Application.DTOs.SellerDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShuppeMarket.Application.Validation
{
    public class SellerUpdateReValidation : AbstractValidator<SellerUpdateRequest>
    {
        public SellerUpdateReValidation()
        {
            RuleFor(x => x.ShopName)
            .NotEmpty().WithMessage("Store name is required.")
            .MaximumLength(100).WithMessage("Store name cannot exceed 100 characters.")
            .When(x => !string.IsNullOrEmpty(x.ShopName));

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^(0|\+84)[0-9]{9}$").WithMessage("Invalid phone number format.")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber));           
        }
    }
}
