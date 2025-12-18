using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MuseumSystem.Application.Dtos;
using ShuppeMarket.Application.Interfaces;
using ShuppeMarket.Domain.Abstractions;
using ShuppeMarket.Domain.Entities;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel;

namespace ShuppeMarket.API.Controllers
{
    [Route("api/v1/categories")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpPost("{name}")]
        [SwaggerOperation("Create a new category"),Description("Creates a new category")]
        public async Task<IActionResult> CreateCategory(string name)
        {
            var category = await _categoryService.CreateCategoryAsync(name);
            return Ok(ApiResponse<Category>.OkResponse(category, "Create category successful!", "200"));
        }

        [HttpGet]
        [SwaggerOperation("Get all categories"), Description("Retrieves a paginated list of all categories")]
        public async Task<IActionResult> GetAllCategories([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var categories = await _categoryService.GetAllCategoriesAsync(pageIndex, pageSize);
            return Ok(ApiResponse<BasePaginatedList<Category>>.OkResponse(categories, "Get all categories successful!", "200"));
        }

        [HttpGet("{id}")]
        [SwaggerOperation("Get category by ID"), Description("Retrieves a category by its ID")]
        public async Task<IActionResult> GetCategoryById([FromRoute] string id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            return Ok(ApiResponse<Category>.OkResponse(category, "Get category by ID successful!", "200"));
        }

        [HttpPut("{id}/{name}")]
        [SwaggerOperation("Update category"), Description("Updates an existing category")]
        public async Task<IActionResult> UpdateCategory(string id, string name)
        {
            var category = await _categoryService.UpdateCategoryAsync(id, name);
            return Ok(ApiResponse<Category>.OkResponse(category, "Update category successful!", "200"));
        }
        [HttpDelete("{id}")]
        [SwaggerOperation("Delete category"), Description("Deletes a category by its ID")]
        public async Task<IActionResult> DeleteCategory([FromRoute] string id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);
            return Ok(ApiResponse<string>.OkResponse(result, "Delete category successful!", "200"));
        }
    }
}
