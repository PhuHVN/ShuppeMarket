using ShuppeMarket.Application.DTOs.ReviewDtos;
using ShuppeMarket.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShuppeMarket.Application.Interfaces
{
    public interface IReviewService
    {
        Task<ReviewResponse> CreateReview(string productId, ReviewRequest request);
        Task<ReviewResponse> GetReviewById(string id);
        Task<BasePaginatedList<object>> GetAllReviews(int pageIndex = 1,
             int pageSize = 10,
             string? searchTerm = null,
             string? orderBy = null,
             string? fields = null);
        Task<BasePaginatedList<object>> GetAllReviewsByProductId(string productId, int pageIndex = 1, int pageSize = 10, string? searchTerm = null, string? orderBy = null, string? fields = null);
        Task<ReviewResponse> UpdateReview(string id, ReviewRequest request);
        Task<string> DeleteReview(string id);
        Task<double> OverallStarsByProductId(string productId);
    }
}
