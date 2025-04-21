using EfRepository.DbEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfRepository.DbService
{
    public interface ICustomerBankRepository
    {
        Task<List<CustomerBankInfo>> GetAccountsByCustomerIdAsync(int customerId);
        Task<CustomerBankInfo> GetBankInfoByAccountAsync(string bankName, string accountNumber);
        Task<CustomerBankInfo> GetBankInfoByIdAsync(int id);
        Task SaveAsync();
    }
}
