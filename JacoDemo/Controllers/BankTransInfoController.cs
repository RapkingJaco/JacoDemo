using EfRepository;
using EfRepository.DbService;
using JacoDemo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace JacoDemo.Controllers
{
    public class BankTransInfoController : Controller
    {
        private readonly IBankInfoRepository _bankInfoRepository;

        public BankTransInfoController(IBankInfoRepository bankInfoRepository)
        {
            _bankInfoRepository = bankInfoRepository;
        }

        //讀取當前登入使用者的所有交易記錄
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index()
        {
            //從 Claims 提取已登入使用者的唯一識別 ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //組裝查詢條件物件，只包含使用者 ID，表示查詢該使用者所有交易
            var query = new BankTransInfo
            {
                CustomerId = int.Parse(userId) // 不帶其他條件，查全部
            };

            var resultList = await _bankInfoRepository.GetTransListAsync(query);

            //將結果封裝到 ViewModel，以便在 View 中顯示
            var bankTransInfoView = new BankTransInfoViewModel
            {
                ResultList = resultList
            };

            return View("Index", bankTransInfoView);
        }

        //接收使用者輸入的篩選條件，並回傳符合條件的交易列表
        [HttpPost]
        [Authorize]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> QueryTransList(BankTransInfoViewModel bankTransInfoViewModel)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //根據前端 ViewModel 中的條件，建立查詢物件
            var query = new BankTransInfo
            {
                CustomerId = int.Parse(userId),
                AccountNumber = bankTransInfoViewModel.AccountNumber,  // 可空白表示不篩選帳號
                TransType = bankTransInfoViewModel.TransType,          // 存款/提款/跨行
                StartDate = bankTransInfoViewModel.StartDate,          // 有指定則篩選起始日
                EndDate = bankTransInfoViewModel.EndDate,              // 有指定則篩選結束日
                IsCrossBank = bankTransInfoViewModel.IsCrossBank       // 是否只看跨行交易
            };

            //執行動態查詢，取得符合條件的結果
            bankTransInfoViewModel.ResultList = await _bankInfoRepository.GetTransListAsync(query);

            return View("Index", bankTransInfoViewModel);
        }

    }
}
