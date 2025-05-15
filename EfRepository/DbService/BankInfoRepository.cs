using EfRepository.DbEntity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace EfRepository.DbService
{
    public class BankInfoRepository : IBankInfoRepository
    {
        private readonly JacoBankContext _jacoBankContext;

        public BankInfoRepository(JacoBankContext jacoBankContext)
        {
            _jacoBankContext = jacoBankContext;
        }

        //從資料庫查詢客戶的交易資訊
        //根據輸入的條件（使用者 ID、帳號、交易類型、日期範圍、跨行標記）執行 LINQ 查詢
        public async Task<List<BankTransInfo>> GetTransListAsync(BankTransInfo query)
        {
            var queryable = from trans in _jacoBankContext.CustomerTransInfos
                            join bank in _jacoBankContext.CustomerBankInfos on trans.BankInfoId equals bank.BankInfoId
                            join customer in _jacoBankContext.CustomerInfos on trans.CustomerId equals customer.CustomerId
                            select new BankTransInfo
                            {
                                CustomerId = customer.CustomerId,             // 使用者 ID
                                CustomerName = customer.Name,                 // 使用者名稱
                                BankInfoId = bank.BankInfoId,                 // 帳戶資訊 ID
                                BankName = bank.BankName,                     // 銀行名稱
                                AccountNumber = bank.AccountNumber,           // 帳號
                                Balance = bank.Balance,                       // 帳戶餘額
                                TransId = trans.TransId,                      // 交易 ID
                                TransDate = trans.TransDate,                  // 交易日期
                                ReceiverBankName = trans.ReceiverBankName,    // 收款銀行 (跨行)
                                ReceiverAccount = trans.ReceiverAccount,      // 收款帳號 (跨行)
                                TransType = trans.TransType,                  // 交易類型
                                Amount = trans.Amount,                        // 金額
                                Note = trans.Note,                            // 備註

                                // 將查詢條件的日期範圍帶入，以便後續在 View 顯示使用
                                StartDate = query.StartDate,
                                EndDate = query.EndDate
                            };

            //按條件動態篩選：
            //CustomerId 不為 0：只查該使用者所有交易
            if (query.CustomerId != 0)
                queryable = queryable.Where(q => q.CustomerId == query.CustomerId);

            //TransType 不為空：過濾指定類型 (存款/提款)
            if (!string.IsNullOrEmpty(query.TransType))
                queryable = queryable.Where(q => q.TransType == query.TransType);

            //AccountNumber 不為空：帳號模糊搜尋
            if (!string.IsNullOrEmpty(query.AccountNumber))
                queryable = queryable.Where(q => q.AccountNumber.Contains(query.AccountNumber));

            //StartDate/EndDate 有值：指定日期範圍內的交易
            if (query.StartDate.HasValue)
                queryable = queryable.Where(q => q.TransDate >= query.StartDate.Value);
            if (query.EndDate.HasValue)
                queryable = queryable.Where(q => q.TransDate <= query.EndDate.Value);

            //IsCrossBank 為 true：只顯示跨行交易
            if (query.IsCrossBank)
                queryable = queryable.Where(q => !string.IsNullOrEmpty(q.ReceiverBankName));

            //最後依交易日期降序排序
            return await queryable
                        .OrderByDescending(q => q.TransDate)
                        .ToListAsync();
        }
    }
}
