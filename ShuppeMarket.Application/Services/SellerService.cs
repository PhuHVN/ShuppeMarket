using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShuppeMarket.Application.DTOs.SellerDtos;
using ShuppeMarket.Application.Interfaces;
using ShuppeMarket.Domain.Abstractions;
using ShuppeMarket.Domain.Entities;
using ShuppeMarket.Domain.Enums;
using ShuppeMarket.Domain.ResultError;
using System.Security.Claims;


namespace ShuppeMarket.Application.Services
{
    public class SellerService : ISellerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SellerService> _logger;
        private readonly IValidator<SellerRequest> _validator;
        private readonly IValidator<SellerUpdateRequest> _validatorUpdate;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SellerService(IUnitOfWork unitOfWork, ILogger<SellerService> logger, IValidator<SellerRequest> validator, IValidator<SellerUpdateRequest> validatorUpdate, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _validator = validator;
            _validatorUpdate = validatorUpdate;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<Result<SellerResponse>> RegisterSellerAccount(string accountId, SellerRequest sellerRequest)
        {
            await _validator.ValidateAndThrowAsync(sellerRequest);

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var user = await _unitOfWork.GetRepository<Account>().GetByIdAsync(accountId);
                if (user == null)
                {
                    return Result<SellerResponse>.Fail("NOT_FOUND", $"Account not found.");
                }
                if (user.Role == RoleEnum.Seller)
                {
                    return Result<SellerResponse>.Fail("ALREADY_EXISTS", "Account is already registered as a seller.");
                }
                var existingSeller = await _unitOfWork.GetRepository<Seller>()
                    .FindAsync(s => s.AccountId == accountId);
                if (existingSeller != null)
                {
                    return Result<SellerResponse>.Fail("ALREADY_EXISTS", "Seller account already exists for this user.");
                }

                //update account

                var isUpdate = false;
                if (!string.IsNullOrEmpty(sellerRequest.Address) && user.Address != sellerRequest.Address)
                {
                    user.Address = sellerRequest.Address;
                    isUpdate = true;
                }
                if (isUpdate)
                {
                    user.Status = StatusEnum.Pending;
                    user.Role = RoleEnum.Seller;
                    user.LastUpdatedAt = DateTime.UtcNow;
                    await _unitOfWork.GetRepository<Account>().UpdateAsync(user);
                }

                //seller
                var newSeller = new Seller
                {
                    AccountId = user.Id,
                    ShopName = sellerRequest.ShopName,
                    Description = sellerRequest.Description,
                    LogoUrl = sellerRequest.LogoUrl,
                };
                await _unitOfWork.GetRepository<Seller>().InsertAsync(newSeller);
                //save db
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
                _logger.LogInformation("Seller account registered successfully for AccountId: {AccountId}", accountId);
                return Result<SellerResponse>.Success(_mapper.Map<SellerResponse>(newSeller));

            }
            catch (Exception ex)
            {
                await _unitOfWork.RollBackAsync();
                _logger.LogError(ex, "Error registering seller account for AccountId: {AccountId}", accountId);
                throw;
            }
        }

        public async Task<Result<SellerResponse>> GetSellerById(string id)
        {
            var seller = await _unitOfWork.GetRepository<Seller>().FindAsync(x => x.Id == id, x => x.Include(x => x.Account));
            if (seller == null)
            {
                return Result<SellerResponse>.Fail("NOT_FOUND", $"Seller not found.");
            }
            return Result<SellerResponse>.Success(_mapper.Map<SellerResponse>(seller));
        }

        public async Task<Result<BasePaginatedList<SellerResponse>>> GetAllSellers(int pageIndex, int pageSize)
        {
            var sellersQuery = _unitOfWork.GetRepository<Seller>().Entity;
            sellersQuery = sellersQuery.Include(s => s.Account);
            var rs = await _unitOfWork.GetRepository<Seller>().GetPagging(sellersQuery, pageIndex, pageSize);
            return Result<BasePaginatedList<SellerResponse>>.Success(_mapper.Map<BasePaginatedList<SellerResponse>>(rs));

        }

        public async Task<Result<SellerResponse>> ApproveSellerAccount(string sellerId)
        {
            var seller = await _unitOfWork.GetRepository<Seller>().FindAsync(x => x.Id == sellerId, q => q.Include(x => x.Account));
            if (seller == null)
            {
                return Result<SellerResponse>.Fail("NOT_FOUND", $"Seller not found.");
            }
            var account = await _unitOfWork.GetRepository<Account>().GetByIdAsync(seller.AccountId);
            if (account == null)
            {
                return Result<SellerResponse>.Fail("NOT_FOUND", $"Account not found.");
            }
            account.Status = StatusEnum.Active;
            account.Role = RoleEnum.Seller;
            account.LastUpdatedAt = DateTime.UtcNow;
            await _unitOfWork.GetRepository<Account>().UpdateAsync(account);
            await _unitOfWork.SaveChangesAsync();
            return Result<SellerResponse>.Success(_mapper.Map<SellerResponse>(seller));
        }

        public async Task<Result<string>> DeleteSellerAccount(string id)
        {
            var seller = await _unitOfWork.GetRepository<Seller>().GetByIdAsync(id);
            if (seller == null)
            {
                return Result<string>.Fail("NOT_FOUND", $"Seller not found.");
            }
            var account = await _unitOfWork.GetRepository<Account>().GetByIdAsync(seller.AccountId);
            if (account == null)
            {
                return Result<string>.Fail("NOT_FOUND", $"Account not found.");
            }
            account.Status = StatusEnum.Inactive;
            await _unitOfWork.GetRepository<Account>().UpdateAsync(account);
            await _unitOfWork.SaveChangesAsync();
            return Result<string>.Success("Seller account deleted successfully.");
        }

        public async Task<Result<SellerResponse>> GetSellerByAccountId(string accountId)
        {
            var seller = await _unitOfWork.GetRepository<Seller>().FindAsync(x => x.AccountId == accountId, q => q.Include(x => x.Account));
            if (seller == null)
            {
                return Result<SellerResponse>.Fail("NOT_FOUND", $"Seller not found.");
            }
            return Result<SellerResponse>.Success(_mapper.Map<SellerResponse>(seller));
        }
        public async Task<Result<SellerResponse>> UpdateSellerAccount(SellerUpdateRequest sellerUpdateRequest)
        {
            await _validatorUpdate.ValidateAndThrowAsync(sellerUpdateRequest);
            // 
            var accountId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(accountId))
            {
                return Result<SellerResponse>.Fail("UNAUTHORIZED", "Unauthorized to update this account.");
            }

            var seller = await _unitOfWork.GetRepository<Seller>().FindAsync(x => x.AccountId == accountId, q => q.Include(x => x.Account));
            if (seller == null)
            {
                return Result<SellerResponse>.Fail("NOT_FOUND", $"Seller not found.");
            }
            if (seller.Account.Status == StatusEnum.Inactive)
            {
                return Result<SellerResponse>.Fail("INACTIVE", "Seller account is inactive.");
            }
            var isUpdate = false;
            if (!string.IsNullOrEmpty(sellerUpdateRequest.ShopName) && sellerUpdateRequest.ShopName != seller.ShopName)
            {
                seller.ShopName = sellerUpdateRequest.ShopName;
                isUpdate = true;
            }
            if (!string.IsNullOrEmpty(sellerUpdateRequest.Description) && sellerUpdateRequest.Description != seller.Description)
            {
                seller.Description = sellerUpdateRequest.Description;
                isUpdate = true;
            }
            if (!string.IsNullOrEmpty(sellerUpdateRequest.LogoUrl) && sellerUpdateRequest.LogoUrl != seller.LogoUrl)
            {
                seller.LogoUrl = sellerUpdateRequest.LogoUrl;
                isUpdate = true;
            }
            if (isUpdate)
            {
                await _unitOfWork.BeginTransactionAsync();
                try
                {

                    await _unitOfWork.GetRepository<Seller>().UpdateAsync(seller);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitTransactionAsync();
                }
                catch
                {
                    await _unitOfWork.RollBackAsync();
                    _logger.LogError("Error updating seller account for SellerId: {SellerId}", seller.Id);
                    throw;
                }
            }
            return Result<SellerResponse>.Success(_mapper.Map<SellerResponse>(seller));
        }
    }
}
