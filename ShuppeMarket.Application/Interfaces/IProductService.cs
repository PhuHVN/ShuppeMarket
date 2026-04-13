using ShuppeMarket.Application.DTOs.ProductDtos;
using ShuppeMarket.Domain.Abstractions;
using ShuppeMarket.Domain.ResultError;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Google.Apis.Requests.BatchRequest;

namespace ShuppeMarket.Application.Interfaces
{
    public interface IProductService
    {
        Task<ProductResponse> CreateProductAsync( ProductRequest request);
        Task<ProductResponse> UpdateProductAsync(string id, ProductUpdateRequest request);
        Task<string> DeleteProductAsync(string productId);
        Task<BasePaginatedList<object>> GetAllProductsAsync(int pageIndex = 1,
             int pageSize = 10,
             string? searchTerm = null,
             string? orderBy = null,
             string? fields = null);
       
        Task<Result<ProductResponse>> GetProductByIdAsync(string productId);
    }
}
