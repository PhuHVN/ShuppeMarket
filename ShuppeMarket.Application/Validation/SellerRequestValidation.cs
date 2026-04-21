using FluentValidation;
using ShuppeMarket.Application.DTOs.SellerDtos;

namespace ShuppeMarket.Application.Validation
{
    public class SellerRequestValidation : AbstractValidator<SellerRequest>
    {
        public SellerRequestValidation()
        {
            RuleFor(x => x.ShopName)
                .NotEmpty().WithMessage("Store name is required.")
                .MaximumLength(100).WithMessage("Store name cannot exceed 100 characters.");
            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Store address is required.");
        }
    }
}
