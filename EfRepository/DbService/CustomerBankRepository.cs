using EfRepository.DbEntity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfRepository.DbService
{
    //負責提供使用者帳戶的讀取功能
    public class CustomerBankRepository : ICustomerBankRepository
    {
        private readonly JacoBankContext _jacoBankContext;

        public CustomerBankRepository(JacoBankContext jacoBankContext)
        {
            _jacoBankContext = jacoBankContext;
        }

        //根據 CustomerId 取得該使用者第一筆帳戶資訊
        public async Task<CustomerBankInfo> GetBankInfoByIdAsync(int customerId)
        {
            return await _jacoBankContext.CustomerBankInfos
                .FirstOrDefaultAsync(x => x.CustomerId == customerId);
        }

        //根據銀行名稱與帳號查詢單一帳戶資訊
        public async Task<CustomerBankInfo> GetBankInfoByAccountAsync(string bankName, string accountNumber)
        {
            return await _jacoBankContext.CustomerBankInfos
                .FirstOrDefaultAsync(b => b.BankName == bankName && b.AccountNumber == accountNumber);
        }

        //取得指定 CustomerId 的所有帳戶清單
        public async Task<List<CustomerBankInfo>> GetAccountsByCustomerIdAsync(int customerId)
        {
            return await _jacoBankContext.CustomerBankInfos
                .Where(x => x.CustomerId == customerId)
                .ToListAsync();
        }

        //將所有暫存變更儲存到資料庫
        public async Task SaveAsync()
        {
            await _jacoBankContext.SaveChangesAsync();
        }
    }
}
