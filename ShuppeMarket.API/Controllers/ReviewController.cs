using Microsoft.AspNetCore.Mvc;
using ShuppeMarket.Application.DTOs;
using ShuppeMarket.Application.DTOs.ReviewDtos;
using ShuppeMarket.Application.Interfaces;
using ShuppeMarket.Domain.Abstractions;

namespace ShuppeMarket.API.Controllers
{
    [Route("api/v1/reviews")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService reviewService;

        public ReviewController(IReviewService reviewService)
        {
            this.reviewService = reviewService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllReviews(int pageIndex = 1, int pageSize = 10, string? searchTerm = null, string? orderBy = null, string? fields = null)
        {
            var reviews = await reviewService.GetAllReviews(pageIndex, pageSize, searchTerm, orderBy, fields);
            return Ok(ApiResponse<BasePaginatedList<object>>.OkResponse(reviews.Value, "Get successfully", "200"));
        }

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetReviewsByProductId(string productId, int pageIndex = 1, int pageSize = 10, string? searchTerm = null, string? orderBy = null, string? fields = null)
        {
            var reviews = await reviewService.GetAllReviewsByProductId(productId, pageIndex, pageSize, searchTerm, orderBy, fields);
            return Ok(ApiResponse<BasePaginatedList<object>>.OkResponse(reviews.Value, "Get successfully", "200"));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReviewById(string id)
        {
            var review = await reviewService.GetReviewById(id);
            return Ok(ApiResponse<ReviewResponse>.OkResponse(review.Value, "Get successfully", "200"));
        }
        [HttpPost("product/{productId}")]
        public async Task<IActionResult> CreateReview(string productId, [FromBody] ReviewRequest request)
        {
            var review = await reviewService.CreateReview(productId, request);
            return Ok(ApiResponse<ReviewResponse>.OkResponse(review.Value, "Created successfully", "200"));
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReview(string id, [FromBody] ReviewRequest request)
        {
            var review = await reviewService.UpdateReview(id, request);
            if (review == null)
            {
                return NotFound();
            }
            return Ok(ApiResponse<ReviewResponse>.OkResponse(review.Value, "Updated successfully", "200"));
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var review = await reviewService.DeleteReview(id);
            return NoContent();
        }
        [HttpGet("product/{productId}/overall-stars")]
        public async Task<IActionResult> GetOverallStarsByProductId(string productId)
        {
            var overallStars = await reviewService.OverallStarsByProductId(productId);
            return Ok(ApiResponse<double>.OkResponse(overallStars.Value, "Get overall stars successfully", "200"));
        }
    }
}
