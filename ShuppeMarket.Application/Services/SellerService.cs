using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using ShuppeMarket.Application.DTOs.SellerDtos;
using ShuppeMarket.Application.Interfaces;
using ShuppeMarket.Domain.Abstractions;
using ShuppeMarket.Domain.Entities;
using ShuppeMarket.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShuppeMarket.Application.Services
{
    public class SellerService : ISellerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SellerService> _logger;
        private readonly IValidator<SellerRequest> _validator;
        private readonly IValidator<SellerUpdateRequest> _validatorUpdate;
        private readonly IMapper _mapper;

        public SellerService(IUnitOfWork unitOfWork, ILogger<SellerService> logger, IValidator<SellerRequest> validator, IValidator<SellerUpdateRequest> validatorUpdate, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _validator = validator;
            _validatorUpdate = validatorUpdate;
            _mapper = mapper;
        }

        public async Task<SellerResponse> RegisterSellerAccount(string accountId, SellerRequest sellerRequest)
        {
            await _validator.ValidateAndThrowAsync(sellerRequest);

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var user = await _unitOfWork.GetRepository<Account>().GetByIdAsync(accountId);
                if (user == null)
                {
                    throw new KeyNotFoundException($"Account with ID {accountId} not found.");
                }
                if (user.Role == RoleEnum.Seller)
                {
                    throw new InvalidOperationException("Account is already registered as a seller.");
                }
                var existingSeller = await _unitOfWork.GetRepository<Seller>()
                    .FindAsync(s => s.AccountId == accountId);
                if (existingSeller != null)
                {
                    throw new InvalidOperationException("Seller account already exists for this user.");
                }

                //update account

                var isUpdate = false;
                if (!string.IsNullOrEmpty(sellerRequest.PhoneNumber) && user.PhoneNumber != sellerRequest.PhoneNumber)
                {
                    user.PhoneNumber = sellerRequest.PhoneNumber;
                    isUpdate = true;
                }
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
                return _mapper.Map<SellerResponse>(newSeller);

            }
            catch (Exception ex)
            {
                await _unitOfWork.RollBackAsync();
                _logger.LogError(ex, "Error registering seller account for AccountId: {AccountId}", accountId);
                throw;
            }
        }

        public async Task<SellerResponse> GetSellerById(string id)
        {
            var seller = await _unitOfWork.GetRepository<Seller>().GetByIdAsync(id);
            if (seller == null)
            {
                throw new KeyNotFoundException($"Seller with ID {id} not found.");
            }
            return _mapper.Map<SellerResponse>(seller);
        }

        public async Task<BasePaginatedList<SellerResponse>> GetAllSellers(int pageIndex, int pageSize)
        {
            var sellersQuery = _unitOfWork.GetRepository<Seller>().Entity.Where(x => x.Account.Status == StatusEnum.Active);
            var rs = await _unitOfWork.GetRepository<Seller>().GetPagging(sellersQuery, pageIndex, pageSize);
            return _mapper.Map<BasePaginatedList<SellerResponse>>(rs);

        }

        public async Task<SellerResponse> ApproveSellerAccount(string id)
        {
            var seller = await _unitOfWork.GetRepository<Seller>().GetByIdAsync(id);
            if (seller == null)
            {
                throw new KeyNotFoundException($"Seller with ID {id} not found.");
            }
            var account = await _unitOfWork.GetRepository<Account>().GetByIdAsync(seller.AccountId);
            if (account == null)
            {
                throw new KeyNotFoundException($"Account with ID {seller.AccountId} not found.");
            }
            account.Status = StatusEnum.Active;
            await _unitOfWork.GetRepository<Account>().UpdateAsync(account);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<SellerResponse>(seller);
        }

        public async Task<string> DeleteSellerAccount(string id)
        {
            var seller = await _unitOfWork.GetRepository<Seller>().GetByIdAsync(id);
            if (seller == null)
            {
                throw new KeyNotFoundException($"Seller with ID {id} not found.");
            }
            var account = await _unitOfWork.GetRepository<Account>().GetByIdAsync(seller.AccountId);
            if (account == null)
            {
                throw new KeyNotFoundException($"Account with ID {seller.AccountId} not found.");
            }
            account.Status = StatusEnum.Inactive;
            await _unitOfWork.GetRepository<Account>().UpdateAsync(account);
            await _unitOfWork.SaveChangesAsync();
            return "Seller account deleted successfully.";
        }

        public async Task<SellerResponse> UpdateSellerAccount(string id, SellerUpdateRequest sellerUpdateRequest)
        {
            await _validatorUpdate.ValidateAndThrowAsync(sellerUpdateRequest);
            var seller = await _unitOfWork.GetRepository<Seller>().GetByIdAsync(id);
            if (seller == null)
            {
                throw new KeyNotFoundException("Seller with ID {id} not found.");
            }
            var account = await _unitOfWork.GetRepository<Account>().GetByIdAsync(seller.AccountId);
            if (account == null)
            {
                throw new KeyNotFoundException($"Account with ID {seller.AccountId} not found.");
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
            if (!string.IsNullOrEmpty(sellerUpdateRequest.Address) && sellerUpdateRequest.Address != account.Address)
            {
                account.Address = sellerUpdateRequest.Address;
                isUpdate = true;
            }
            if (!string.IsNullOrEmpty(sellerUpdateRequest.PhoneNumber) && sellerUpdateRequest.PhoneNumber != account.PhoneNumber)
            {
                account.PhoneNumber = sellerUpdateRequest.PhoneNumber;
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
                    account.LastUpdatedAt = DateTime.UtcNow;
                    await _unitOfWork.GetRepository<Seller>().UpdateAsync(seller);
                    await _unitOfWork.GetRepository<Account>().UpdateAsync(account);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitTransactionAsync();
                }
                catch
                {
                    await _unitOfWork.RollBackAsync();
                    _logger.LogError("Error updating seller account for SellerId: {SellerId}", id);
                    throw;
                }
            }
            return _mapper.Map<SellerResponse>(seller);
        }
    }
}
