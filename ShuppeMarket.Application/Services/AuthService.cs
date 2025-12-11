using AutoMapper;
using BCrypt.Net;
using FluentValidation;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Http;
using ShuppeMarket.Application.DTOs.AccountDtos;
using ShuppeMarket.Application.DTOs.LoginDtos;
using ShuppeMarket.Application.Interfaces;
using ShuppeMarket.Domain.Abstractions;
using ShuppeMarket.Domain.Entities;
using ShuppeMarket.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ShuppeMarket.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenerateTokenService _generateTokenService;
        private readonly IValidator<LoginRequest> _validator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        public AuthService(IUnitOfWork unitOfWork, IGenerateTokenService generateTokenService, IValidator<LoginRequest> validator, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _generateTokenService = generateTokenService;
            _validator = validator;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }
        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            await _validator.ValidateAndThrowAsync(request);
            //
            var user = await _unitOfWork.GetRepository<Accounts>()
                .FindAsync(x => x.Email == request.Email);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }
            //verify passwordv
            var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
            if (!isPasswordValid)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            var token = _generateTokenService.GenerateToken(user);
            return new AuthResponse
            {
                UserId = user.Id.ToString(),
                Role = user.Role.ToString(),
                Token = token.ToString()
            };

        }

        public async Task<AuthResponse> LoginGoogleAsync(LoginGoogleRequest request)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken);
                var email = payload.Email;
                var fullName = payload.Name;
                var user = await _unitOfWork.GetRepository<Accounts>().FindAsync(x => x.Email == email);
                if (user == null)
                {
                    var newUser = new Accounts
                    {
                        Email = email,
                        FullName = fullName,
                        Address = "",
                        PhoneNumber = "",
                        CreatedAt = DateTime.UtcNow,
                        Status = StatusEnum.Active,
                        Role = RoleEnum.Customer,
                        Password = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString()) 
                    };
                    await _unitOfWork.GetRepository<Accounts>().InsertAsync(newUser);
                    await _unitOfWork.SaveChangeAsync();
                    user = newUser;
                }
                var token = _generateTokenService.GenerateToken(user);
                return new AuthResponse
                {
                    UserId = user.Id.ToString(),
                    Role = user.Role.ToString(),
                    Token = token
                };
            }
            catch (InvalidJwtException)
            {
                throw new UnauthorizedAccessException("Google IdToken is invalid or expired.");
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Google login failed: {ex.Message}");
            }
        }

        public async Task<AccountResponse> CurrentUser()
        {
            var context = _httpContextAccessor.HttpContext;
            var userId = context?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User not authenticated.");
            }
            var user = await _unitOfWork.GetRepository<Accounts>().GetByIdAsync(userId);
            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found.");
            }
            return _mapper.Map<AccountResponse>(user);

        }
    }
}
