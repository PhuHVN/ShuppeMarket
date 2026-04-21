using ShuppeMarket.Application.DTOs.ProductDtos;
using ShuppeMarket.Domain.Abstractions;
using ShuppeMarket.Domain.ResultError;

namespace ShuppeMarket.Application.Interfaces
{
    public interface IProductService
    {
        Task<Result<ProductResponse>> CreateProductAsync(ProductRequest request);
        Task<Result<ProductResponse>> UpdateProductAsync(string id, ProductUpdateRequest request);
        Task<Result<string>> DeleteProductAsync(string productId);
        Task<Result<BasePaginatedList<object>>> GetAllProductsAsync(int pageIndex = 1,
             int pageSize = 10,
             string? searchTerm = null,
             string? orderBy = null,
             string? fields = null);

        Task<Result<ProductResponse>> GetProductByIdAsync(string productId);
    }
}
