using JacoBankAPI.Models;
using JacoBankAPI.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JacoBankAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IApiTransferService _iapiTransferService;

        public PaymentController(IApiTransferService iapiTransferService)
        {
            _iapiTransferService = iapiTransferService;
        }
        [HttpPost("transfer")]
        public async Task<ActionResult<TransferResponseModel>> Transfer([FromBody] TransferRequestModel transferRequestModel)
        {
            var res = await _iapiTransferService.ExecuteAsync(transferRequestModel);
            return StatusCode(res.IsSuccess ? 200 : 400, res);
        }
    }    
}