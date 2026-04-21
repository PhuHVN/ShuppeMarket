using ShuppeMarket.Application.Interfaces;
using ShuppeMarket.Domain.Abstractions;
using ShuppeMarket.Domain.Entities;
using ShuppeMarket.Domain.ResultError;

namespace ShuppeMarket.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<Category>> CreateCategoryAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return Result<Category>.Fail("INVALID", "Category name cannot be null or empty");
            }
            var ExistingCategory = await _unitOfWork.GetRepository<Category>().FindAsync(c => c.Name.ToLower() == name.ToLower());
            if (ExistingCategory != null)
            {
                return Result<Category>.Fail("DUPLICATE", "Category with the same name already exists");
            }
            var category = new Category
            {
                Name = name,
                CreateAt = DateTime.UtcNow,
                Status = Domain.Enums.StatusEnum.Active
            };
            await _unitOfWork.GetRepository<Category>().InsertAsync(category);
            await _unitOfWork.SaveChangesAsync();
            return Result<Category>.Success(category);
        }

        public async Task<Result<string>> DeleteCategoryAsync(string id)
        {
            var category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(id);
            if (category == null)
            {
                return Result<string>.Fail("NOT_FOUND", "Category not found");
            }
            category.Status = Domain.Enums.StatusEnum.Inactive;
            await _unitOfWork.GetRepository<Category>().UpdateAsync(category);
            await _unitOfWork.SaveChangesAsync();
            return Result<string>.Success("Category deleted successfully");
        }

        public async Task<Result<BasePaginatedList<Category>>> GetAllCategoriesAsync(int pageIndex, int pageSize)
        {
            var query = _unitOfWork.GetRepository<Category>().Entity.Where(x => x.Status == Domain.Enums.StatusEnum.Active);
            var rs = await _unitOfWork.GetRepository<Category>().GetPagging(query, pageIndex, pageSize);
            return Result<BasePaginatedList<Category>>.Success(rs);
        }

        public async Task<Result<Category>> GetCategoryByIdAsync(string id)
        {
            var category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(id);
            if (category == null)
            {
                return Result<Category>.Fail("NOT_FOUND", "Category not found");
            }
            return Result<Category>.Success(category);
        }

        public async Task<Result<Category>> UpdateCategoryAsync(string id, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return Result<Category>.Fail("INVALID", "Category name cannot be null or empty");
            }
            var categoryTask = await _unitOfWork.GetRepository<Category>().GetByIdAsync(id);
            if (categoryTask == null)
            {
                return Result<Category>.Fail("NOT_FOUND", "Category not found");
            }
            if (categoryTask.Name.ToLower() != name.ToLower())
            {
                var existingCategory = await _unitOfWork.GetRepository<Category>().FindAsync(c => c.Name.ToLower() == name.ToLower());
                if (existingCategory != null)
                {
                    throw new ArgumentException("Category with the same name already exists", nameof(name));
                }
            }
            categoryTask.Name = name;
            await _unitOfWork.GetRepository<Category>().UpdateAsync(categoryTask);
            await _unitOfWork.SaveChangesAsync();
            return Result<Category>.Success(categoryTask);
        }
    }
}
