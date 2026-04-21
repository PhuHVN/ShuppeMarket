using Microsoft.AspNetCore.Mvc;
using ShuppeMarket.Application.DTOs;
using ShuppeMarket.Application.DTOs.AccountDtos;
using ShuppeMarket.Application.DTOs.AuthDtos;
using ShuppeMarket.Application.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel;

namespace ShuppeMarket.API.Controllers
{
    [Route("api/v1/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;
        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }

        [HttpPost("login")]
        [SwaggerOperation("Login Email"), Description("User login with email and password")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var response = await authService.LoginAsync(request);
            return Ok(ApiResponse<AuthResponse>.OkResponse(response.Value, "Login successful", "200"));
        }

        [HttpPost("login-google")]
        [SwaggerOperation("Login Google"), Description("User login with google account")]
        public async Task<IActionResult> LoginGoogle([FromBody] LoginGoogleRequest request)
        {
            var response = await authService.LoginGoogleAsync(request);
            return Ok(ApiResponse<AuthResponse>.OkResponse(response.Value, "Login google successful", "200"));
        }

        [HttpGet("me")]
        [SwaggerOperation("Get Current User"), Description("Get current logged in user details")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var response = await authService.CurrentUser();
            return Ok(ApiResponse<AccountResponse>.OkResponse(response.Value, "Current user retrieved successfully", "200"));
        }

        [HttpPost("register")]
        [SwaggerOperation("Register User"), Description("Register a new user account")]
        public async Task<IActionResult> Register([FromBody] AccountRequest request)
        {
            var message = await authService.RegisterAsync(request);
            return Ok(ApiResponse<string>.OkResponse(message.Value, "User registered successfully", "201"));
        }

        [HttpPost("verify-otp")]
        [SwaggerOperation("Verify OTP"), Description("Verify OTP for user registration")]
        public async Task<IActionResult> VerifyOtp([FromQuery] string email, [FromQuery] string otp)
        {
            var message = await authService.VerifyOtp(email, otp);
            return Ok(ApiResponse<string>.OkResponse(message.Value, "OTP verified successfully", "200"));
        }
    }
}
