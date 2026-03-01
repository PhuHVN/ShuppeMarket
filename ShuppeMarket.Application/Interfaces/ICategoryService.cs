using ShuppeMarket.Domain.Abstractions;
using ShuppeMarket.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShuppeMarket.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<Category> CreateCategoryAsync(string name);
        Task<BasePaginatedList<Category>> GetAllCategoriesAsync(int pageIndex, int pageSize);
        Task<Category> GetCategoryByIdAsync(string id);
        Task<Category> UpdateCategoryAsync(string id, string name);
        Task<string> DeleteCategoryAsync(string id);
    }
}
