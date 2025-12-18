using ShuppeMarket.Application.DTOs.ProductDtos;
using ShuppeMarket.Application.Interfaces;
using ShuppeMarket.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShuppeMarket.Application.Services
{
    public class ProductService : IProductService
    {
        public Task<ProductResponse> CreateProductAsync(ProductRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ProductResponse> DeleteProductAsync(string productId)
        {
            throw new NotImplementedException();
        }

        public Task<BasePaginatedList<ProductResponse>> GetAllProductsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ProductResponse> GetProductByIdAsync(string productId)
        {
            throw new NotImplementedException();
        }

        public Task<ProductResponse> UpdateProductAsync(string id, ProductUpdateRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
