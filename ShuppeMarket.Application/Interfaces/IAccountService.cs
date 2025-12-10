using ShuppeMarket.Domain.Abstractions;
using ShuppeMarket.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShuppeMarket.Application.Interfaces
{
    public interface IAccountService
    {
        Task<Accounts> CreateAccount();
        Task<Accounts> UpdateAccount(Accounts account);
        Task<Accounts> GetAccountById(string id);
        Task<BasePaginatedList<Accounts>> GetAllAccounts();
        Task DeleteAccount(string id);
    }
}
