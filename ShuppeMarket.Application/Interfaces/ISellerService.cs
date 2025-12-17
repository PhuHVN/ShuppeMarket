using ShuppeMarket.Application.DTOs.SellerDtos;
using ShuppeMarket.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShuppeMarket.Application.Interfaces
{
    public interface ISellerService
    {
        Task<SellerResponse> RegisterSellerAccount(SellerRequest sellerRequest);
        Task<SellerResponse> GetSellerById(string id);
        Task<SellerResponse> UpdateSellerAccount(SellerUpdateRequest sellerUpdateRequest);
        Task<BasePaginatedList<SellerResponse>> GetAllSellers(int pageIndex, int pageSize);
        Task<string> DeleteSellerAccount(string id);
        Task<SellerResponse> ApproveSellerAccount(string id);

    }
}
