using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShuppeMarket.Application.DTOs.ProductDtos;
using ShuppeMarket.Application.Interfaces;
using ShuppeMarket.Domain.Abstractions;
using ShuppeMarket.Domain.Entities;
using ShuppeMarket.Domain.Enums;
using ShuppeMarket.Domain.ResultError;
using System.Security.Claims;

namespace ShuppeMarket.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProductService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IValidator<ProductRequest> _productRequestValidator;
        private readonly IMapper mapper;
        private readonly ICloudinaryService _cloudinary;
        public ProductService(IUnitOfWork unitOfWork, ILogger<ProductService> logger, IHttpContextAccessor httpContextAccessor, IValidator<ProductRequest> productRequestValidator, IMapper mapper, ICloudinaryService cloudinary)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _productRequestValidator = productRequestValidator;
            this.mapper = mapper;
            _cloudinary = cloudinary;
        }
        public async Task<Result<ProductResponse>> CreateProductAsync(ProductRequest request)
        {
            await _productRequestValidator.ValidateAndThrowAsync(request);

            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                _logger.LogWarning("User ID not found in HTTP context.");
                return Result<ProductResponse>.Fail("UNAUTHORIZED", "User not authenticated.");
            }
            var seller = await _unitOfWork.GetRepository<Seller>().FindAsync(x => x.AccountId == userId && x.Account.Status == StatusEnum.Active && x.Account.Role == RoleEnum.Seller);
            if (seller == null)
            {
                _logger.LogWarning("Only sellers can create products.");
                return Result<ProductResponse>.Fail("FORBIDDEN", "Only sellers can create products.");
            }
            var ImgUrl = string.Empty;
            if (request.Image != null)
            {
                ImgUrl = await _cloudinary.UploadImageAsync(request.Image);
                if (ImgUrl == null)
                {
                    _logger.LogError("Image upload failed.");
                    return Result<ProductResponse>.Fail("IMAGE_UPLOAD_FAILED", "Failed to upload image.");
                }
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
                Quantity = request.Quantity,
                ImageUrl = ImgUrl,
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

            return Result<ProductResponse>.Success(mapper.Map<ProductResponse>(product));
        }
        public async Task<Result<string>> DeleteProductAsync(string productId)
        {
            var product = await _unitOfWork.GetRepository<Product>().GetByIdAsync(productId);
            if (product == null)
            {
                _logger.LogWarning($"Product with ID {productId} not found.");
                return Result<string>.Fail("NOT_FOUND", $"Product not found.");
            }
            product.Status = StatusEnum.Inactive;
            await _unitOfWork.GetRepository<Product>().UpdateAsync(product);
            return Result<string>.Success("Product deleted successfully.");

        }

        public async Task<Result<BasePaginatedList<object>>> GetAllProductsAsync(int pageIndex = 1,
            int pageSize = 10,
            string? searchTerm = null,
            string? orderBy = null,
            string? fields = null)
        {
            var query = _unitOfWork.GetRepository<Product>().Entity.Where(x => x.Status == StatusEnum.Active);

            if (!string.IsNullOrEmpty(orderBy))
            {
                query = query.OrderByDescending(x => x.CreateAt);
            }
            var map = mapper.ConfigurationProvider;
            var fieldsToSearch = new[] { "Name", "Description" };
            var dtos = _unitOfWork.GetRepository<Product>().Entity.ProjectTo<ProductResponse>(map).AsQueryable();
            var result = await _unitOfWork.GetRepository<Product>().GetAllWithPaggingSortSelectionFieldAsync<Product, ProductResponse>(query, map, searchTerm, fieldsToSearch, orderBy, fields, pageIndex, pageSize);
            return Result<BasePaginatedList<object>>.Success(result);
        }

        public async Task<Result<ProductResponse>> GetProductByIdAsync(string productId)
        {
            var product = await _unitOfWork.GetRepository<Product>().FindAsync(x => x.Id == productId, x => x.Include(x => x.Seller.Account).Include(x => x.CategoryProducts).ThenInclude(x => x.Category));
            if (product == null)
            {
                return Result<ProductResponse>.Fail(Error.NotFound);
            }
            var mappedProduct = mapper.Map<ProductResponse>(product);
            return Result<ProductResponse>.Success(mappedProduct);
        }

        public Task<Result<ProductResponse>> UpdateProductAsync(string id, ProductUpdateRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
