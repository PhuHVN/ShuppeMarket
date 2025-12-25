using ShuppeMarket.Application.DTOs.ProductDtos;
using ShuppeMarket.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShuppeMarket.Application.Interfaces
{
    public interface IProductService
    {
        Task<ProductResponse> CreateProductAsync( ProductRequest request);
        Task<ProductResponse> UpdateProductAsync(string id, ProductUpdateRequest request);
        Task<string> DeleteProductAsync(string productId);
        Task<BasePaginatedList<ProductResponse>> GetAllProductsAsync(int pageIndex, int pageSize);
        Task<ProductResponse> GetProductByIdAsync(string productId);
    }
}
