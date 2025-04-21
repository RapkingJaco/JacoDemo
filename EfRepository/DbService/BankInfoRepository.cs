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
        public async Task<List<BankTransInfo>> GetTransListAsync(BankTransInfo query)
        {
            var queryable = from trans in _jacoBankContext.CustomerTransInfos
                            join bank in _jacoBankContext.CustomerBankInfos on trans.BankInfoId equals bank.BankInfoId
                            join customer in _jacoBankContext.CustomerInfos on trans.CustomerId equals customer.CustomerId
                            select new BankTransInfo
                            {
                                CustomerId = customer.CustomerId,
                                CustomerName = customer.Name,
                                BankInfoId = bank.BankInfoId,
                                BankName = bank.BankName,
                                AccountNumber = bank.AccountNumber,
                                Balance = bank.Balance,
                                TransId = trans.TransId,
                                TransDate = trans.TransDate,
                                StartDate = query.StartDate,
                                EndDate = query.EndDate,
                                TransType = trans.TransType,
                                Amount = trans.Amount,
                                ReceiverBankName = trans.ReceiverBankName,
                                ReceiverAccount = trans.ReceiverAccount,
                                Note = trans.Note                                
                            };

            if (query.CustomerId != 0)
                queryable = queryable.Where(q => q.CustomerId == query.CustomerId);

            if (!string.IsNullOrEmpty(query.TransType))
                queryable = queryable.Where(q => q.TransType == query.TransType);

            if (!string.IsNullOrEmpty(query.AccountNumber))
                queryable = queryable.Where(q => q.AccountNumber.Contains(query.AccountNumber));

            if (query.StartDate.HasValue)
                queryable = queryable.Where(q => q.TransDate >= query.StartDate.Value);

            if (query.EndDate.HasValue)
                queryable = queryable.Where(q => q.TransDate <= query.EndDate.Value);

            if (query.IsCrossBank)
                queryable = queryable.Where(q => !string.IsNullOrEmpty(q.ReceiverBankName));

            return await queryable.OrderByDescending(q => q.TransDate).ToListAsync();
        }
    }
}
