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
        private readonly IValidator<AccountRequest> _registerValidator;
        private readonly IEmailService _emailService;
        private readonly IOtpCacheService _otpCacheService;

        public AuthService(IUnitOfWork unitOfWork, IGenerateTokenService generateTokenService, IValidator<LoginRequest> validator, IHttpContextAccessor httpContextAccessor, IMapper mapper, IValidator<AccountRequest> registerValidator, IEmailService emailService, IOtpCacheService otpCacheService)
        {
            _unitOfWork = unitOfWork;
            _generateTokenService = generateTokenService;
            _validator = validator;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _registerValidator = registerValidator;
            _emailService = emailService;
            _otpCacheService = otpCacheService;
        }
        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            await _validator.ValidateAndThrowAsync(request);
            //
            var user = await _unitOfWork.GetRepository<Accounts>()
                .FindAsync(x => x.Email == request.Email && x.Status != StatusEnum.Inactive);
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
                var user = await _unitOfWork.GetRepository<Accounts>().FindAsync(x => x.Email == email && x.Status != StatusEnum.Inactive);
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

        public async Task<string> RegisterAsync(AccountRequest request)
        {
            await _registerValidator.ValidateAndThrowAsync(request);

            var existingUser = await _unitOfWork.GetRepository<Accounts>()
                .FindAsync(x => x.Email == request.Email);
            if (existingUser != null)
            {
                if (existingUser.Status == StatusEnum.Active)
                {
                    throw new Exception("Email is already registered.");
                }
                else if (existingUser.Status == StatusEnum.Inactive)
                {
                    //resend OTP
                    var otp1 = _otpCacheService.GenerateOTP();
                    await _otpCacheService.StoreOtpAsync(existingUser.Email, otp1, TimeSpan.FromMinutes(10));
                    await _emailService.SendEmailAsync(existingUser.Email, "Verify your account", $"Your OTP code is: {otp1}");
                    return "Account exists but not verified. OTP resent.";
                }

            }
            var newUser = new Accounts
            {
                Email = request.Email,
                FullName = request.FullName,
                Address = request.Address,
                PhoneNumber = request.PhoneNumber,
                CreatedAt = DateTime.UtcNow,
                Status = StatusEnum.Inactive,
                Role = RoleEnum.Customer,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };
            await _unitOfWork.GetRepository<Accounts>().InsertAsync(newUser);
            await _unitOfWork.SaveChangeAsync();

            // Send OTP email for verification
            var otp = _otpCacheService.GenerateOTP();
            await _otpCacheService.StoreOtpAsync(newUser.Email, otp, TimeSpan.FromMinutes(10));
            await _emailService.SendEmailAsync(newUser.Email, "Verify your account", $"Your OTP code is: {otp}");

            return "Registration successful. Please check your email for the OTP to verify your account.";

        }

        public async Task<string> VerifyOtp(string email, string otp)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(otp))
            {
                throw new ArgumentException("Email and OTP must be provided.");
            }
            var cachedOtp = await _otpCacheService.RetrieveOtpAsync(email);
            if (cachedOtp == null)
            {
                throw new ArgumentException("Invalid or expired OTP.");
            }
            if (cachedOtp != otp)
            {
                throw new ArgumentException("Invalid OTP.");
            }
            await _otpCacheService.RemoveOtpAsync(email);
            var user = await _unitOfWork.GetRepository<Accounts>()
                .FindAsync(x => x.Email == email && x.Status == StatusEnum.Inactive);
            if (user == null)
            {
                throw new ArgumentException("User not found or already verified.");
            }
            user.Status = StatusEnum.Active;
            await _unitOfWork.GetRepository<Accounts>().UpdateAsync(user);
            await _unitOfWork.SaveChangeAsync();
            return "OTP verified successfully.";
        }
    }
}
