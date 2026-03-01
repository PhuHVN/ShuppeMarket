using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using ShuppeMarket.Application.DTOs.ProductDtos;
using ShuppeMarket.Application.Interfaces;
using ShuppeMarket.Domain.Abstractions;
using ShuppeMarket.Domain.Entities;
using ShuppeMarket.Domain.Enums;
using ShuppeMarket.Domain.ResultError;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ShuppeMarket.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProductService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IValidator<ProductRequest> _productRequestValidator;
        private readonly IMapper mapper;
        public ProductService(IUnitOfWork unitOfWork, ILogger<ProductService> logger, IHttpContextAccessor httpContextAccessor, IValidator<ProductRequest> productRequestValidator, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _productRequestValidator = productRequestValidator;
            this.mapper = mapper;
        }
        public async Task<ProductResponse> CreateProductAsync(ProductRequest request)
        {
            await _productRequestValidator.ValidateAndThrowAsync(request);

            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                _logger.LogWarning("User ID not found in HTTP context.");
                throw new UnauthorizedAccessException("User is not authenticated.");
            }
            var seller = await _unitOfWork.GetRepository<Seller>().FindAsync(x => x.AccountId == userId && x.Account.Status == StatusEnum.Active && x.Account.Role == RoleEnum.Seller);
            if (seller == null)
            {
                _logger.LogWarning("Only sellers can create products.");
                throw new UnauthorizedAccessException("Only sellers can create products.");
            }

            var categories = await _unitOfWork.GetRepository<Category>().FilterByAsync(x => request.CategoryIds.Contains(x.Id));
            if (categories.Count() != request.CategoryIds.Count)
            {
                _logger.LogWarning("One or more category IDs are invalid.");
                throw new ArgumentException("One or more category IDs are invalid.");
            }

            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                CreateAt = DateTime.UtcNow,
                SellerId = seller.Id,
                Status = StatusEnum.Active

            };
            foreach (var category in categories)
            {
                product.CategoryProducts.Add(new CategoryProduct
                {
                    CategoryId = category.Id,
                    ProductId = product.Id
                });
            }

            await _unitOfWork.GetRepository<Product>().InsertAsync(product);
            await _unitOfWork.SaveChangesAsync();

            return mapper.Map<ProductResponse>(product);
        }
        public async Task<string> DeleteProductAsync(string productId)
        {
            var product = await _unitOfWork.GetRepository<Product>().GetByIdAsync(productId);
            if (product == null)
            {
                _logger.LogWarning($"Product with ID {productId} not found.");
                throw new KeyNotFoundException("Product not found.");
            }
            product.Status = StatusEnum.Inactive;
            await _unitOfWork.GetRepository<Product>().UpdateAsync(product);
            return "Delete successful !";

        }

        public async Task<BasePaginatedList<ProductResponse>> GetAllProductsAsync(int pageIndex, int pageSize)
        {
            var query = _unitOfWork.GetRepository<Product>().Entity.Include(x => x.Seller).Include(x => x.Seller.Account).Include(x => x.CategoryProducts).ThenInclude(x => x.Category);
            var result = await _unitOfWork.GetRepository<Product>().GetPagging(query, pageIndex, pageSize);
            return mapper.Map<BasePaginatedList<ProductResponse>>(result);
        }

        public async Task<Result<ProductResponse>> GetProductByIdAsync(string productId)
        {
            var product = await _unitOfWork.GetRepository<Product>().FindAsync(x => x.Id == productId , x => x.Include(x => x.Seller.Account).Include(x => x.CategoryProducts).ThenInclude(x => x.Category));
            if(product == null)
            {
               return Result<ProductResponse>.Fail(Error.NotFound);
            }
            var mappedProduct = mapper.Map<ProductResponse>(product);
            return Result<ProductResponse>.Success(mappedProduct);
        }

        public Task<ProductResponse> UpdateProductAsync(string id, ProductUpdateRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
