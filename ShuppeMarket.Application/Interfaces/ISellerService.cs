using ShuppeMarket.Application.DTOs.SellerDtos;
using ShuppeMarket.Domain.Abstractions;
using ShuppeMarket.Domain.ResultError;

namespace ShuppeMarket.Application.Interfaces
{
    public interface ISellerService
    {
        Task<Result<SellerResponse>> RegisterSellerAccount(string accountId, SellerRequest sellerRequest);
        Task<Result<SellerResponse>> GetSellerById(string id);
        Task<Result<SellerResponse>> UpdateSellerAccount(SellerUpdateRequest sellerUpdateRequest);
        Task<Result<BasePaginatedList<SellerResponse>>> GetAllSellers(int pageIndex, int pageSize);
        Task<Result<string>> DeleteSellerAccount(string id);
        Task<Result<SellerResponse>> ApproveSellerAccount(string id);
        Task<Result<SellerResponse>> GetSellerByAccountId(string accountId);
    }
}
