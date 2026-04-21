using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShuppeMarket.Application.DTOs.ReviewDtos;
using ShuppeMarket.Application.Interfaces;
using ShuppeMarket.Domain.Abstractions;
using ShuppeMarket.Domain.Entities;
using ShuppeMarket.Domain.ResultError;

namespace ShuppeMarket.Application.Services
{
    public class ReviewService : IReviewService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ReviewService> _logger;
        private readonly IMapper _mapper;
        private readonly IAuthService auth;

        public ReviewService(IUnitOfWork unitOfWork, ILogger<ReviewService> logger, IMapper mapper, IAuthService auth)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            this.auth = auth;
        }

        public async Task<Result<ReviewResponse>> CreateReview(string productId, ReviewRequest request)
        {
            var userResult = await auth.GetCurrentUserLoginAsync();
            if (!userResult.IsSuccess || userResult.Value == null)
            {
                return Result<ReviewResponse>.Fail("UNAUTHORIZED", "User not found");
            }
            var product = await _unitOfWork.GetRepository<Product>().FindAsync(x => x.Id == productId);
            if (product == null)
            {
                throw new Exception("Product not found");
            }
            var review = new Review
            {
                ProductId = product.Id,
                AccountId = userResult.Value.Id,
                Rating = request.Rating,
                Comment = request.Comment,
                CreateAt = DateTime.UtcNow
            };
            await _unitOfWork.GetRepository<Review>().InsertAsync(review);
            await _unitOfWork.SaveChangesAsync();
            return Result<ReviewResponse>.Success(_mapper.Map<ReviewResponse>(review));

        }

        public async Task<Result<string>> DeleteReview(string id)
        {
            var review = await _unitOfWork.GetRepository<Review>().FindAsync(x => x.Id == id);
            if (review == null)
            {
                return Result<string>.Fail("NOT_FOUND", "Review not found");
            }
            await _unitOfWork.GetRepository<Review>().DeleteAsync(review);
            await _unitOfWork.SaveChangesAsync();
            return Result<string>.Success("Review deleted successfully");
        }

        public async Task<Result<BasePaginatedList<object>>> GetAllReviews(int pageIndex = 1, int pageSize = 10, string? searchTerm = null, string? orderBy = null, string? fields = null)
        {
            var query = _unitOfWork.GetRepository<Review>().Entity.Include(x => x.Account).Include(x => x.Product).AsQueryable();
            if (!string.IsNullOrEmpty(orderBy))
            {
                query = query.OrderByDescending(x => x.CreateAt);
            }
            var mapp = _mapper.ConfigurationProvider;
            var fieldsToSearch = new[] { "Comment" };
            var result = await _unitOfWork.GetRepository<Review>().GetAllWithPaggingSortSelectionFieldAsync<Review, ReviewResponse>(query, mapp, searchTerm, fieldsToSearch, orderBy, fields, pageIndex, pageSize);
            return Result<BasePaginatedList<object>>.Success(result);
        }
        public async Task<Result<BasePaginatedList<object>>> GetAllReviewsByProductId(string productId, int pageIndex = 1, int pageSize = 10, string? searchTerm = null, string? orderBy = null, string? fields = null)
        {
            var query = _unitOfWork.GetRepository<Review>().Entity.Include(x => x.Account).Include(x => x.Product).Where(x => x.ProductId == productId).AsQueryable();
            if (!string.IsNullOrEmpty(orderBy))
            {
                query = query.OrderByDescending(x => x.CreateAt);
            }
            var mapp = _mapper.ConfigurationProvider;
            var fieldsToSearch = new[] { "Comment" };
            var result = await _unitOfWork.GetRepository<Review>().GetAllWithPaggingSortSelectionFieldAsync<Review, ReviewResponse>(query, mapp, searchTerm, fieldsToSearch, orderBy, fields, pageIndex, pageSize);
            return Result<BasePaginatedList<object>>.Success(result);
        }

        public async Task<Result<ReviewResponse>> GetReviewById(string id)
        {
            var review = await _unitOfWork.GetRepository<Review>().FindAsync(x => x.Id == id, include: x => x.Include(x => x.Account).Include(x => x.Product));
            if (review == null)
            {
                return Result<ReviewResponse>.Fail("NOT_FOUND", "Review not found");
            }
            return Result<ReviewResponse>.Success(_mapper.Map<ReviewResponse>(review));
        }

        public async Task<Result<double>> OverallStarsByProductId(string productId)
        {
            var reviews = await _unitOfWork.GetRepository<Review>().Entity
                .Where(x => x.ProductId == productId)
                .ToListAsync();

            if (reviews == null || reviews.Count == 0)
            {
                return Result<double>.Success(0);
            }

            var averageStars = reviews.Average(x => x.Rating);
            return Result<double>.Success(averageStars);
        }

        public Task<Result<ReviewResponse>> UpdateReview(string id, ReviewRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
