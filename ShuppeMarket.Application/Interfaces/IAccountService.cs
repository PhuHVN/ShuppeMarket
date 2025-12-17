using ShuppeMarket.Application.DTOs.AccountDtos;
using ShuppeMarket.Domain.Abstractions;
using ShuppeMarket.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShuppeMarket.Application.Interfaces
{
    public interface IAccountService
    {
        Task<AccountResponse> CreateAccount(AccountRequest request);
        Task<AccountResponse> UpdateAccount(string id,AccountUpdateRequest request);
        Task<AccountResponse> GetAccountById(string id);
        Task<BasePaginatedList<AccountResponse>> GetAllAccounts(int pageIndex, int pageSize);
        Task DeleteAccount(string id);
        Task<AccountResponse> AssignSellerAccount(string id,AccountUpdateRequest request);
        Task<AccountResponse> ConfirmSellerAccount(string id);
    }
}
