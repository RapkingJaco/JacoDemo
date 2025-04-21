using JacoBankAPI.Models;

namespace JacoBankAPI.Service
{
    public interface IApiTransferService
    {
        Task<TransferResponseModel> ExecuteAsync(TransferRequestModel transferRequest);
    }
}
