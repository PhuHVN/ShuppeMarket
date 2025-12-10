using ShuppeMarket.Application.Interfaces;
using ShuppeMarket.Domain.Abstractions;
using ShuppeMarket.Domain.Entities;
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
        public AccountService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<Accounts> CreateAccount()
        {
            throw new NotImplementedException();
        }

        public Task DeleteAccount(string id)
        {
            throw new NotImplementedException();
        }

        public Task<Accounts> GetAccountById(string id)
        {
            throw new NotImplementedException();
        }

        public Task<BasePaginatedList<Accounts>> GetAllAccounts()
        {
            throw new NotImplementedException();
        }

        public Task<Accounts> UpdateAccount(Accounts account)
        {
            throw new NotImplementedException();
        }
    }
}
