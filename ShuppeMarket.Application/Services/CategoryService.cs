using ShuppeMarket.Application.Interfaces;
using ShuppeMarket.Domain.Abstractions;
using ShuppeMarket.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShuppeMarket.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Category> CreateCategoryAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Category name cannot be null or empty", nameof(name));
            }
            var ExistingCategory = await _unitOfWork.GetRepository<Category>().FindAsync(c => c.Name.ToLower() == name.ToLower());
            if (ExistingCategory != null)
            {
                throw new ArgumentException("Category with the same name already exists", nameof(name));
            }
            var category = new Category
            {
                Name = name,
                CreateAt = DateTime.UtcNow,
                Status = Domain.Enums.StatusEnum.Active
            };
            await _unitOfWork.GetRepository<Category>().InsertAsync(category);
            await _unitOfWork.SaveChangesAsync();
            return category;
        }

        public async Task<string> DeleteCategoryAsync(string id)
        {
            var category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(id);
            if (category == null)
            {
                throw new ArgumentException("Category not found", nameof(id));
            }
            category.Status = Domain.Enums.StatusEnum.Inactive;
            await _unitOfWork.GetRepository<Category>().UpdateAsync(category);
            return "Category deleted successfully";
        }

        public async Task<BasePaginatedList<Category>> GetAllCategoriesAsync(int pageIndex, int pageSize )
        {
            var query = _unitOfWork.GetRepository<Category>().Entity.Where(x => x.Status == Domain.Enums.StatusEnum.Active);
            var rs =await _unitOfWork.GetRepository<Category>().GetPagging(query, pageIndex, pageSize);
            return rs;
        }

        public Task<Category?> GetCategoryByIdAsync(string id)
        {
            var category = _unitOfWork.GetRepository<Category>().GetByIdAsync(id);
            if (category == null)
            {
                throw new ArgumentException("Category not found", nameof(id));
            }
            return category;
        }

        public async Task<Category> UpdateCategoryAsync(string id, string name)
        {
            if(string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Category name cannot be null or empty", nameof(name));
            }
            var categoryTask = await _unitOfWork.GetRepository<Category>().GetByIdAsync(id);
            if(categoryTask == null)
            {
                throw new ArgumentException("Category not found", nameof(id));
            }
           if(categoryTask.Name.ToLower() != name.ToLower())
            {
                var existingCategory = await _unitOfWork.GetRepository<Category>().FindAsync(c => c.Name.ToLower() == name.ToLower());
                if(existingCategory != null)
                {
                    throw new ArgumentException("Category with the same name already exists", nameof(name));
                }
            }
            categoryTask.Name = name;
            await _unitOfWork.GetRepository<Category>().UpdateAsync(categoryTask);
            await _unitOfWork.SaveChangesAsync();
            return  categoryTask;
        }
    }
}
