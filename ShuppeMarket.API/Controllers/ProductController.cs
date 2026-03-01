using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MuseumSystem.Application.Dtos;
using ShuppeMarket.Application.DTOs.ProductDtos;
using ShuppeMarket.Application.Interfaces;
using ShuppeMarket.Domain.Abstractions;
using ShuppeMarket.Domain.ResultError;
using Swashbuckle.AspNetCore.Annotations;

namespace ShuppeMarket.API.Controllers
{
    [Route("api/v1/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Get product by ID",
            Description = "Retrieve a product by its unique identifier."
        )]
        public async Task<IActionResult> GetProductByIdAsync(string id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product.IsFailure)
            {
                return NotFound(ApiResponse<ProductResponse>.NotFoundResponse("Product not found"));
            }
            return Ok(ApiResponse<ProductResponse>.OkResponse(product.Value, "Get product successful!", "200"));
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Get all products",
            Description = "Retrieve a paginated list of all products."
        )]
        public async Task<IActionResult> GetAllProductsAsync([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var products = await _productService.GetAllProductsAsync(pageIndex, pageSize);
            return Ok(ApiResponse<BasePaginatedList<ProductResponse>>.OkResponse(products, "Get products successful!", "200"));
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Create a new product",
            Description = "Create a new product with the provided details."
        )]
        public async Task<IActionResult> CreateProductAsync([FromBody] ProductRequest request)
        {
            var product = await _productService.CreateProductAsync(request);
            return Ok(ApiResponse<ProductResponse>.OkResponse(product, "Create product successful!", "201"));
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Delete a product",
            Description = "Delete a product by its unique identifier."
        )]
        public async Task<IActionResult> DeleteProductAsync(string id)
        {
            var result = await _productService.DeleteProductAsync(id);
            return Ok(ApiResponse<string>.OkResponse(result, "Delete product successful!", "200"));
        }

        
    }
}
