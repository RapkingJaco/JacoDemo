using EfRepository.DbEntity;
using JacoBankAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace JacoBankAPI.Service
{
    //負責用戶轉帳功能的顯示與執行，前端呼叫 API 執行實際轉帳邏輯，並呈現結果
    public class ApiTransferService : IApiTransferService
    {
        private readonly JacoBankContext _jacoBankContext;
        private readonly ILogger<ApiTransferService> _logger;

        public ApiTransferService(JacoBankContext jacoBankContext, ILogger<ApiTransferService> logger)
        {
            _jacoBankContext = jacoBankContext;
            _logger = logger;
        }

        //根據傳入的 TransferRequestModel 驗證、扣款、入帳，並回傳 TransferResponseModel
        public async Task<TransferResponseModel> ExecuteAsync(TransferRequestModel transferRequest)
        {
            //找出轉出帳戶並驗證是否存在及是否為本人
            var sender = await _jacoBankContext.CustomerBankInfos.FindAsync(transferRequest.FromBankInfoId);
            if (sender == null)
            {
                return new TransferResponseModel
                {
                    IsSuccess = false,
                    Message = "帳戶不存在或不是本人"
                };
            }

            //檢查餘額是否足夠
            if (sender.Balance < transferRequest.Amount)
            {
                return new TransferResponseModel
                {
                    IsSuccess = false,
                    Message = "餘額不足"
                };
            }

            //根據銀行名稱與帳號查找收款帳戶
            var receiver = await _jacoBankContext.CustomerBankInfos.FirstOrDefaultAsync(x => x.BankName == transferRequest.ToBankName && x.AccountNumber == transferRequest.ToAccountNumber);

            if (receiver == null)
            {
                return new TransferResponseModel
                {
                    IsSuccess = false,
                    Message = "收款銀行或帳號不存在"
                };
            }

            //扣款與入帳
            sender.Balance -= transferRequest.Amount;
            receiver.Balance += transferRequest.Amount;

            //建立交易記錄（轉出/轉入）
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

            //儲存所有變更，完成交易
            await _jacoBankContext.SaveChangesAsync();
            return new TransferResponseModel { IsSuccess = true, Message = "轉帳成功" };


        }
    }

}
