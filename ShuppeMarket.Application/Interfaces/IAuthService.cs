using ShuppeMarket.Application.DTOs.AccountDtos;
using ShuppeMarket.Application.DTOs.AuthDtos;
using ShuppeMarket.Domain.Entities;
using ShuppeMarket.Domain.ResultError;

namespace ShuppeMarket.Application.Interfaces
{
    public interface IAuthService
    {
        Task<Result<AuthResponse>> LoginAsync(LoginRequest request);
        Task<Result<AuthResponse>> LoginGoogleAsync(LoginGoogleRequest request);
        Task<Result<string>> RegisterAsync(AccountRequest request);
        Task<Result<AccountResponse>> CurrentUser();
        Task<Result<string>> VerifyOtp(string email, string otp);
        Task<Result<Account>> GetCurrentUserLoginAsync();
    }
}
