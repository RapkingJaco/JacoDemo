using EfRepository.DbEntity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfRepository.DbService
{
    public class CustomerBankRepository : ICustomerBankRepository
    {
        private readonly JacoBankContext _jacoBankContext;

        public CustomerBankRepository(JacoBankContext jacoBankContext)
        {
            _jacoBankContext = jacoBankContext;
        }

        public async Task<CustomerBankInfo> GetBankInfoByIdAsync(int customerId)
        {
            return await _jacoBankContext.CustomerBankInfos
                .FirstOrDefaultAsync(x => x.CustomerId == customerId);
        }

        public async Task<CustomerBankInfo> GetBankInfoByAccountAsync(string bankName, string accountNumber)
        {
            return await _jacoBankContext.CustomerBankInfos
                .FirstOrDefaultAsync(b => b.BankName == bankName && b.AccountNumber == accountNumber);
        }
        public async Task<List<CustomerBankInfo>> GetAccountsByCustomerIdAsync(int customerId)
        {
            return await _jacoBankContext.CustomerBankInfos
                .Where(x => x.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task SaveAsync()
        {
            await _jacoBankContext.SaveChangesAsync();
        }
    }
}
