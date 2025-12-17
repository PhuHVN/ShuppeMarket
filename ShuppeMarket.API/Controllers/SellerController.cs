using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MuseumSystem.Application.Dtos;
using ShuppeMarket.Application.DTOs.SellerDtos;
using ShuppeMarket.Application.Interfaces;
using ShuppeMarket.Domain.Abstractions;
using ShuppeMarket.Domain.Enums;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel;

namespace ShuppeMarket.API.Controllers
{
    [Route("api/v1/sellers")]
    [ApiController]
    public class SellerController : ControllerBase
    {
        private readonly ISellerService sellerService;
        public SellerController(ISellerService sellerService)
        {
            this.sellerService = sellerService;
        }

        [Authorize(Roles = Roles.Customer)]
        [HttpPost("register/{accountId}")]
        [SwaggerOperation(Summary = "Register a new seller account"), Description("Seller Register  ")]
        public async Task<IActionResult> RegisterSellerAccount(string accountId, [FromBody] SellerRequest sellerRequest)
        {
            var sellerResponse = await sellerService.RegisterSellerAccount(accountId, sellerRequest);
            return Ok(ApiResponse<SellerResponse>.OkResponse(sellerResponse, "Register seller account successful!", "201"));
        }

        [Authorize]
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get seller by ID"), Description("Get Seller by ID")]
        public async Task<IActionResult> GetSellerById(string id)
        {
            var sellerResponse = await sellerService.GetSellerById(id);
            return Ok(ApiResponse<SellerResponse>.OkResponse(sellerResponse, "Get seller by ID successful!", "200"));

        }

        [Authorize(Roles = Roles.Admin)]
        [HttpGet]
        [SwaggerOperation(Summary = "Get all sellers with pagination"), Description("Get all sellers with pagination")]
        public async Task<IActionResult> GetAllSellers(int pageIndex = 1, int pageSize = 10)
        {
            var sellers = await sellerService.GetAllSellers(pageIndex, pageSize);
            return Ok(ApiResponse<BasePaginatedList<SellerResponse>>.OkResponse(sellers, "Get all sellers successful!", "200"));
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete seller account by ID"), Description("Delete Seller account by ID")]
        public async Task<IActionResult> DeleteSellerAccount(string id)
        {
            var result = await sellerService.DeleteSellerAccount(id);
            return Ok(ApiResponse<string>.OkResponse(result, "Delete seller account successful!", "200"));
        }


        [Authorize(Roles = Roles.Admin)]
        [HttpPut("approve/{id}")]
        [SwaggerOperation(Summary = "Approve seller account by ID"), Description("Approve Seller account by ID")]
        public async Task<IActionResult> ApproveSellerAccount(string id)
        {
            var sellerResponse = await sellerService.ApproveSellerAccount(id);
            return Ok(ApiResponse<SellerResponse>.OkResponse(sellerResponse, "Approve seller account successful!", "200"));
        }

        [Authorize]
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Update seller account by ID"), Description("Update Seller account by ID")]
        public async Task<IActionResult> UpdateSellerAccount( [FromBody] SellerUpdateRequest sellerUpdateRequest)
        {
            var sellerResponse = await sellerService.UpdateSellerAccount( sellerUpdateRequest);
            return Ok(ApiResponse<SellerResponse>.OkResponse(sellerResponse, "Update seller account successful!", "200"));
        }
    }
}
