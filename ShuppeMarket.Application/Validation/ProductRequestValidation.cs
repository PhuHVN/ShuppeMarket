using FluentValidation;
using ShuppeMarket.Application.DTOs.ProductDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShuppeMarket.Application.Validation
{
    public class ProductRequestValidation : AbstractValidator<ProductRequest>
    {
        public ProductRequestValidation() {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product name is required.")
                .MaximumLength(100).WithMessage("Product name must not exceed 100 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Product description is required.")
                .MaximumLength(500).WithMessage("Product description must not exceed 500 characters.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Product price must be greater than zero.");
            RuleFor(x => x.CategoryIds)
                .NotNull().WithMessage("Category IDs cannot be null.")
                .NotEmpty().WithMessage("At least one category ID is required.");

        }
    }
}
