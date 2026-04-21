using ShuppeMarket.Domain.Abstractions;
using ShuppeMarket.Domain.Entities;
using ShuppeMarket.Domain.ResultError;

namespace ShuppeMarket.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<Result<Category>> CreateCategoryAsync(string name);
        Task<Result<BasePaginatedList<Category>>> GetAllCategoriesAsync(int pageIndex, int pageSize);
        Task<Result<Category>> GetCategoryByIdAsync(string id);
        Task<Result<Category>> UpdateCategoryAsync(string id, string name);
        Task<Result<string>> DeleteCategoryAsync(string id);
    }
}
