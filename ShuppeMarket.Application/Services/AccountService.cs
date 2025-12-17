using AutoMapper;
using BCrypt.Net;
using FluentValidation;
using Microsoft.Extensions.Logging;
using ShuppeMarket.Application.DTOs.AccountDtos;
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
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AccountService> _logger;
        private readonly IMapper _mapper;
        private readonly IValidator<AccountRequest> _validator;
        private readonly IValidator<AccountUpdateRequest> _updateValidator;

        public AccountService(IUnitOfWork unitOfWork, ILogger<AccountService> logger, IMapper mapper, IValidator<AccountRequest> validator, IValidator<AccountUpdateRequest> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _validator = validator;
            _updateValidator = updateValidator;
        }

        public async Task<AccountResponse> CreateAccount(AccountRequest request)
        {
            //validate request
            await _validator.ValidateAndThrowAsync(request);

            //Business logic
            var accountExist = await _unitOfWork.GetRepository<Account>().FindAsync(x => x.Email == request.Email);
            if (accountExist != null)
            {
                throw new Exception("Account with this email already exists.");
            }
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var account = new Account
            {
                Email = request.Email,
                Password = passwordHash,
                FullName = request.FullName,
                Address = request.Address,
                PhoneNumber = request.PhoneNumber,
                CreatedAt = DateTime.UtcNow,
                Role = RoleEnum.Customer,
                Status = StatusEnum.Active
            };
            await _unitOfWork.GetRepository<Account>().InsertAsync(account);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Account created with email: {Email}", account.Email);
            return _mapper.Map<AccountResponse>(account);
        }

        public async Task DeleteAccount(string id)
        {
            var account = await _unitOfWork.GetRepository<Account>().GetByIdAsync(id);
            if (account == null)
            {
                throw new Exception("Account not found.");
            }
            account.Status = StatusEnum.Inactive;
            await _unitOfWork.GetRepository<Account>().UpdateAsync(account);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Account with id: {Id} has been deactivated.", id);
        }

        public async Task<AccountResponse> GetAccountById(string id)
        {
            var account = await _unitOfWork.GetRepository<Account>().GetByIdAsync(id);
            if (account == null)
            {
                throw new Exception("Account not found.");
            }
            return _mapper.Map<AccountResponse>(account);
        }

        public async Task<BasePaginatedList<AccountResponse>> GetAllAccounts(int pageIndex, int pageSize)
        {
            var query = _unitOfWork.GetRepository<Account>().Entity.Where(x => x.Status == StatusEnum.Active);
            var rs = await _unitOfWork.GetRepository<Account>().GetPagging(query, pageIndex, pageSize);
            return _mapper.Map<BasePaginatedList<AccountResponse>>(rs);
        }

        public async Task<AccountResponse> UpdateAccount(string id,AccountUpdateRequest request)
        {
            if(request == null)
            {
                throw new Exception("Invalid request.");
            }
            await _updateValidator.ValidateAndThrowAsync(request);
            //
            var accountExist = await _unitOfWork.GetRepository<Account>().GetByIdAsync(id);
            if (accountExist == null)
            {
                throw new Exception("Account not found.");
            }
            var isUpdate = false;
            if(!string.IsNullOrEmpty(request.FullName) && request.FullName != accountExist.FullName)
            {
                accountExist.FullName = request.FullName;
                isUpdate = true;
            }
            if(!string.IsNullOrEmpty(request.Address) && request.Address != accountExist.Address)
            {
                accountExist.Address = request.Address;
                isUpdate = true;
            }
            if(!string.IsNullOrEmpty(request.PhoneNumber) && request.PhoneNumber != accountExist.PhoneNumber)
            {
                accountExist.PhoneNumber = request.PhoneNumber;
                isUpdate = true;
            }
            if(isUpdate)
            {
                accountExist.LastUpdatedAt = DateTime.UtcNow;
                await _unitOfWork.GetRepository<Account>().UpdateAsync(accountExist);
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Account with id: {Id} has been updated.", id);
            }
            return _mapper.Map<AccountResponse>(accountExist);
        }    
    }
}
