using AutoMapper;
using FluentValidation;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Http;
using ShuppeMarket.Application.DTOs.AccountDtos;
using ShuppeMarket.Application.DTOs.AuthDtos;
using ShuppeMarket.Application.Interfaces;
using ShuppeMarket.Domain.Abstractions;
using ShuppeMarket.Domain.Entities;
using ShuppeMarket.Domain.Enums;
using ShuppeMarket.Domain.ResultError;
using System.Security.Claims;

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
        public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request)
        {
            await _validator.ValidateAndThrowAsync(request);
            //
            var user = await _unitOfWork.GetRepository<Account>()
                .FindAsync(x => x.Email == request.Email && x.Status != StatusEnum.Inactive);
            if (user == null)
            {
                return Result<AuthResponse>.Fail("UNAUTHORIZED", "Invalid email or password.");
            }
            //verify passwordv
            var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
            if (!isPasswordValid)
            {
                return Result<AuthResponse>.Fail("UNAUTHORIZED", "Invalid email or password.");
            }

            var token = _generateTokenService.GenerateToken(user);
            var authResponse = new AuthResponse
            {
                Token = token.ToString()
            };
            return Result<AuthResponse>.Success(authResponse);
        }

        public async Task<Result<AuthResponse>> LoginGoogleAsync(LoginGoogleRequest request)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken);
                var email = payload.Email;
                var fullName = payload.Name;
                var user = await _unitOfWork.GetRepository<Account>().FindAsync(x => x.Email == email && x.Status != StatusEnum.Inactive);
                if (user == null)
                {
                    var newUser = new Account
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
                    await _unitOfWork.GetRepository<Account>().InsertAsync(newUser);
                    await _unitOfWork.SaveChangesAsync();
                    user = newUser;
                }
                var token = _generateTokenService.GenerateToken(user);
                var authResponse = new AuthResponse
                {
                    Token = token
                };
                return Result<AuthResponse>.Success(authResponse);
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

        public async Task<Result<AccountResponse>> CurrentUser()
        {
            var context = _httpContextAccessor.HttpContext;
            var userId = context?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Result<AccountResponse>.Fail(Error.Unauthorized);
            }
            var user = await _unitOfWork.GetRepository<Account>().GetByIdAsync(userId);
            if (user == null)
            {
                return Result<AccountResponse>.Fail(Error.NotFound);
            }
            return Result<AccountResponse>.Success(_mapper.Map<AccountResponse>(user));

        }

        public async Task<Result<string>> RegisterAsync(AccountRequest request)
        {
            await _registerValidator.ValidateAndThrowAsync(request);

            var existingUser = await _unitOfWork.GetRepository<Account>()
                .FindAsync(x => x.Email == request.Email);
            if (existingUser != null)
            {
                if (existingUser.Status == StatusEnum.Active)
                {
                    return Result<string>.Fail("CONFLICT", "Email is already registered.");
                }
                else if (existingUser.Status == StatusEnum.Inactive)
                {
                    //resend OTP
                    var otp1 = _otpCacheService.GenerateOTP();
                    await _otpCacheService.StoreOtpAsync(existingUser.Email, otp1, TimeSpan.FromMinutes(10));
                    await _emailService.SendCodeOtpEmailAsync(existingUser.Email, otp1);
                    return Result<string>.Fail("INVALID", "Account exists but not verified. OTP resent.");
                }

            }
            var newUser = new Account
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
            await _unitOfWork.GetRepository<Account>().InsertAsync(newUser);
            await _unitOfWork.SaveChangesAsync();

            // Send OTP email for verification
            var otp = _otpCacheService.GenerateOTP();
            await _otpCacheService.StoreOtpAsync(newUser.Email, otp, TimeSpan.FromMinutes(10));
            await _emailService.SendCodeOtpEmailAsync(newUser.Email, otp);

            return Result<string>.Success("Registration successful. Please check your email for the OTP to verify your account.");

        }
        public async Task<Result<Account>> GetCurrentUserLoginAsync()
        {
            var context = _httpContextAccessor.HttpContext;
            var userId = context?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Result<Account>.Fail("UNAUTHORIZED", "User not authenticated.");
            }
            var user = await _unitOfWork.GetRepository<Account>().FindAsync(x => x.Id == userId && x.Status == StatusEnum.Active);
            if (user == null)
            {
                return Result<Account>.Fail("UNAUTHORIZED", "User not found.");
            }
            return Result<Account>.Success(user);
        }

        public async Task<Result<string>> VerifyOtp(string email, string otp)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(otp))
            {
                return Result<string>.Fail("INVALID", "Email and OTP must be provided.");
            }
            var cachedOtp = await _otpCacheService.RetrieveOtpAsync(email);
            if (!cachedOtp.IsSuccess)
            {
                return Result<string>.Fail("INVALID", "Invalid or expired OTP.");
            }
            if (cachedOtp.Value != otp)
            {
                return Result<string>.Fail("INVALID", "Invalid OTP.");
            }
            await _otpCacheService.RemoveOtpAsync(email);
            var user = await _unitOfWork.GetRepository<Account>()
                .FindAsync(x => x.Email == email && x.Status == StatusEnum.Inactive);
            if (user == null)
            {
                return Result<string>.Fail("INVALID", "User not found or already verified.");
            }
            user.Status = StatusEnum.Active;
            await _unitOfWork.GetRepository<Account>().UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
            return Result<string>.Success("OTP verified successfully.");
        }
    }
}
