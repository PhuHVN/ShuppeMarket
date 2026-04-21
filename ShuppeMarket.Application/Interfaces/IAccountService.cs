using ShuppeMarket.Application.DTOs.AccountDtos;
using ShuppeMarket.Domain.Abstractions;
using ShuppeMarket.Domain.ResultError;

namespace ShuppeMarket.Application.Interfaces
{
    public interface IAccountService
    {
        Task<Result<AccountResponse>> CreateAccount(AccountRequest request);
        Task<Result<AccountResponse>> UpdateAccount(AccountUpdateRequest request);
        Task<Result<AccountResponse>> GetAccountById(string id);
        Task<Result<BasePaginatedList<AccountResponse>>> GetAllAccounts(int pageIndex, int pageSize);
        Task<Result<string>> DeleteAccount(string id);

    }
}
