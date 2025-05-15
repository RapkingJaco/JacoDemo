using EfRepository.DbEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfRepository.DbService
{
    //將交易紀錄寫入資料庫
    public class CustomerTransRepository : ICustomerTransRepository
    {
        private readonly JacoBankContext _jacoBankContext;

        public CustomerTransRepository(JacoBankContext jacoBankContext)
        {
            _jacoBankContext = jacoBankContext;
        }
        //新增一筆 CustomerTransInfo 交易紀錄
        public async Task AddTransactionAsync(CustomerTransInfo customerTransInfo)
        {
            // 將交易實體加入 DbContext，等待之後 SaveChanges 一併提交
            await _jacoBankContext.CustomerTransInfos.AddAsync(customerTransInfo);
        }
    }
}
