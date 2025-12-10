using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MuseumSystem.Application.Dtos;
using ShuppeMarket.Application.DTOs.AccountDtos;
using ShuppeMarket.Application.Interfaces;
using ShuppeMarket.Domain.Abstractions;
using Swashbuckle.AspNetCore.Annotations;

namespace ShuppeMarket.API.Controllers
{
    [Route("api/v1/accounts")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create a new account", Description = "Creates a new account with the provided details.")]
        public async Task<IActionResult> CreateAccount([FromBody] AccountRequest request)
        {
            var account = await _accountService.CreateAccount(request);
            return Ok(ApiResponse<AccountResponse>.OkResponse(account, "Account created successfully", "201"));
        }
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get account by ID", Description = "Retrieves the account details for the specified account ID.")]
        public async Task<IActionResult> GetAccountById(string id)
        {
            var account = await _accountService.GetAccountById(id);
            return Ok(ApiResponse<AccountResponse>.OkResponse(account, "Account retrieved successfully", "200"));
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get all accounts", Description = "Retrieves a paginated list of all accounts.")]
        public async Task<IActionResult> GetAllAccounts(int pageIndex = 1, int pageSize = 10)
        {
            var accounts = await _accountService.GetAllAccounts(pageIndex, pageSize);
            return Ok(ApiResponse<BasePaginatedList<AccountResponse>>.OkResponse(accounts, "Accounts retrieved successfully", "200"));
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Update account", Description = "Updates the details of an existing account.")]
        public async Task<IActionResult> UpdateAccount(string id, [FromBody] AccountUpdateRequest request)
        {
            var account = await _accountService.UpdateAccount(id, request);
            return Ok(ApiResponse<AccountResponse>.OkResponse(account, "Account updated successfully", "200"));
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete account", Description = "Deletes the account with the specified ID.")]
        public async Task<IActionResult> DeleteAccount(string id)
        {
            await _accountService.DeleteAccount(id);
            return Ok(ApiResponse<string>.OkResponse(null, "Account deleted successfully", "200"));
        }
    }
}
