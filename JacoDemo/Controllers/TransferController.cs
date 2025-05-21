using EfRepository.DbEntity;
using EfRepository.DbService;
using JacoBankAPI.Models;
using JacoDemo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace JacoDemo.Controllers
{
    public class TransferController : Controller
    {
        private readonly HttpClient _http;                    // 用來呼叫後端 Web API
        private readonly ICustomerBankRepository _customerBankRepository;
        
        public TransferController(IHttpClientFactory factory, ICustomerBankRepository customerBankRepository)
        {
            // 取得命名為 "JacoBankAPI" 的 HttpClient 實例
            _http = factory.CreateClient("JacoBankAPI");
            _customerBankRepository = customerBankRepository;
        }

        //顯示轉帳頁面
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var accountList = await _customerBankRepository.GetAccountsByCustomerIdAsync(int.Parse(userId));

            //將查詢結果存到 ViewBag，以便在 View 中用於下拉選單
            ViewBag.FromAccounts = accountList;

            return View(new TransferViewModel());
        }

        //執行轉帳請求
        [HttpPost]
        public async Task<IActionResult> Transfer(TransferViewModel transferViewModel)
        {
            //從 ViewModel 與 Claims 組成 API 所需的 TransferRequestModel
            var req = new TransferRequestModel
            {
                SenderCustomerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
                FromBankInfoId = transferViewModel.FromBankInfoId,
                ToBankName = transferViewModel.ToBankName,
                ToAccountNumber = transferViewModel.ToAccountNumber,
                Amount = transferViewModel.Amount,
                Note = transferViewModel.Note
            };

            //重新查詢帳戶資訊以保持下拉選單(如果有轉帳失敗)
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var accountList = await _customerBankRepository.GetAccountsByCustomerIdAsync(int.Parse(userId));
            ViewBag.FromAccounts = accountList;

            var res = await _http.PostAsJsonAsync("/api/payment/transfer", req);

            //若 API 回傳失敗，讀取錯誤訊息並回到 View 顯示
            if (!res.IsSuccessStatusCode)
            {
                var result = await res.Content.ReadFromJsonAsync<TransferResponseModel>();
                ModelState.AddModelError("", result?.Message ?? "轉帳失敗");
                return View("Index", transferViewModel);
            }

            //成功轉帳
            TempData["SuccessMessage"] = "轉帳成功";
            return RedirectToAction("Index");
        }
    }
}
