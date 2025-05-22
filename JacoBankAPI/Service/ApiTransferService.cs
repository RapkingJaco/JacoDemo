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
            //取出轉出方最新餘額
            var lastSenderTrans = await _jacoBankContext.CustomerTransInfos
                .Where(t => t.BankInfoId == sender.BankInfoId)
                .OrderByDescending(t => t.TransDate)
                .FirstOrDefaultAsync();
            decimal senderCurrentBalance = lastSenderTrans?.BalanceAfter ?? 0;

            //檢查餘額
            if (senderCurrentBalance < transferRequest.Amount)
                return new TransferResponseModel { IsSuccess = false, Message = "餘額不足" };

            //取出收款方最新餘額
            var lastReceiverTrans = await _jacoBankContext.CustomerTransInfos
                .Where(t => t.BankInfoId == receiver.BankInfoId)
                .OrderByDescending(t => t.TransDate)
                .FirstOrDefaultAsync();
            decimal receiverCurrentBalance = lastReceiverTrans?.BalanceAfter ?? 0;

            //建交易紀錄（設定 BalanceAfter）
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
                ReceiverAccount = transferRequest.ToAccountNumber,
                BalanceAfter = senderCurrentBalance - transferRequest.Amount
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
                ReceiverAccount = sender.AccountNumber,
                BalanceAfter = receiverCurrentBalance + transferRequest.Amount
            });

            //同步更新CustomerBankInfo.Balance
            sender.Balance = senderCurrentBalance - transferRequest.Amount;
            receiver.Balance = receiverCurrentBalance + transferRequest.Amount;

            //儲存
            await _jacoBankContext.SaveChangesAsync();         
            return new TransferResponseModel { IsSuccess = true, Message = "轉帳成功" };
        }
    }

}
