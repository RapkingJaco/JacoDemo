using EfRepository.DbEntity;
using JacoBankAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace JacoBankAPI.Service
{
    public class ApiTransferService : IApiTransferService
    {
        private readonly JacoBankContext _jacoBankContext;
        private readonly ILogger<ApiTransferService> _logger;

        public ApiTransferService(JacoBankContext jacoBankContext, ILogger<ApiTransferService> logger)
        {
            _jacoBankContext = jacoBankContext;
            _logger = logger;
        }
        public async Task<TransferResponseModel> ExecuteAsync(TransferRequestModel transferRequest)
        {
            var sender = await _jacoBankContext.CustomerBankInfos.FindAsync(transferRequest.FromBankInfoId);
            if (sender == null)
            {
                return new TransferResponseModel
                {
                    IsSuccess = false,
                    Message = "帳戶不存在或不是本人"
                };
            }
            if (sender.Balance < transferRequest.Amount)
            {
                return new TransferResponseModel
                {
                    IsSuccess = false,
                    Message = "餘額不足"
                };
            }
            var receiver = await _jacoBankContext.CustomerBankInfos.FirstOrDefaultAsync(x => x.BankName == transferRequest.ToBankName && x.AccountNumber == transferRequest.ToAccountNumber);

            if (receiver == null)
            {
                return new TransferResponseModel
                {
                    IsSuccess = false,
                    Message = "收款銀行或帳號不存在"
                };
            }

            sender.Balance -= transferRequest.Amount;
            receiver.Balance += transferRequest.Amount;

            var nowdate = DateTime.Now;
            _jacoBankContext.CustomerTransInfos.Add(new CustomerTransInfo
            {
                CustomerId = sender.CustomerId,
                BankInfoId = sender.BankInfoId,
                TransType = "轉出",
                Amount = transferRequest.Amount,
                Note = transferRequest.Note,
                TransDate = nowdate,
                ReceiverBankName = transferRequest.ToBankName,
                ReceiverAccount = transferRequest.ToAccountNumber
            });

            _jacoBankContext.CustomerTransInfos.Add(new CustomerTransInfo
            {
                CustomerId = receiver.CustomerId,
                BankInfoId = receiver.BankInfoId,
                TransType = "轉入",
                Amount = transferRequest.Amount,
                Note = transferRequest.Note,
                TransDate = nowdate,
                ReceiverBankName = sender.BankName,
                ReceiverAccount = sender.AccountNumber
            });

            await _jacoBankContext.SaveChangesAsync();
            return new TransferResponseModel { IsSuccess = true, Message = "轉帳成功" };


        }
    }

}
