using ShuppeMarket.Application.DTOs.ReviewDtos;
using ShuppeMarket.Domain.Abstractions;
using ShuppeMarket.Domain.ResultError;

namespace ShuppeMarket.Application.Interfaces
{
    public interface IReviewService
    {
        Task<Result<ReviewResponse>> CreateReview(string productId, ReviewRequest request);
        Task<Result<ReviewResponse>> GetReviewById(string id);
        Task<Result<BasePaginatedList<object>>> GetAllReviews(int pageIndex = 1,
             int pageSize = 10,
             string? searchTerm = null,
             string? orderBy = null,
             string? fields = null);
        Task<Result<BasePaginatedList<object>>> GetAllReviewsByProductId(string productId, int pageIndex = 1, int pageSize = 10, string? searchTerm = null, string? orderBy = null, string? fields = null);
        Task<Result<ReviewResponse>> UpdateReview(string id, ReviewRequest request);
        Task<Result<string>> DeleteReview(string id);
        Task<Result<double>> OverallStarsByProductId(string productId);
    }
}
