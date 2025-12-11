
using Microsoft.AspNetCore.Http;
using ShuppeMarket.Application.DTOs.AccountDtos;
using ShuppeMarket.Application.DTOs.LoginDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShuppeMarket.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(DTOs.LoginDtos.LoginRequest request);
        Task<AuthResponse> LoginGoogleAsync(LoginGoogleRequest request);
        Task<AccountResponse> CurrentUser();
    }
}
