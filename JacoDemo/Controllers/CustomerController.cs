using EfRepository.DbEntity;
using EfRepository.DbService;
using EfRepository.Service;
using JacoDemo.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.RegularExpressions;


namespace JacoDemo.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerRepository _customerRepository;

        //與EfRepository溝通
        public CustomerController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        //顯示使用者資料
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index()
        {
            //取得使用者ID、Name
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = User.Identity.Name;

            //取得使用者資料
            var customer = await _customerRepository.GetCustomerAsync(int.Parse(userId));

            var customerViewModel = new CustomerRegisterViewModel();
            customerViewModel.CustomerId = customer.CustomerId;
            customerViewModel.UserName = customer.Name;
            customerViewModel.Email = customer.Email;
            customerViewModel.Phone = customer.Phone;

            //回傳View
            return View(customerViewModel);

        }

        //更新使用者資料
        [HttpPost]
        [Authorize]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> CustomerInfoUpdate(CustomerRegisterViewModel customerRegisterViewModel)
        {
            //取得使用者ID、Name
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cusInfo = new CustomerInfo();
            cusInfo.CustomerId = int.Parse(userId);
            cusInfo.Name = customerRegisterViewModel.UserName;
            cusInfo.Email = customerRegisterViewModel.Email;
            cusInfo.Phone = customerRegisterViewModel.Phone;

            await _customerRepository.UpdateCustomerAsync(cusInfo);

            //儲存成功，紀錄目前使用者
            Claim[] claims = new Claim[]
            {
                new Claim(ClaimTypes.Name, cusInfo.Name),
                new Claim(ClaimTypes.NameIdentifier, cusInfo.CustomerId.ToString())
            };

            //儲存登入憑證
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

            //顯示儲存成功
            TempData["SuccessSaveMessage"] = "儲存成功！";
            return View("Index", customerRegisterViewModel);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        //變更密碼
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePassword(CustomerChagePwdRequestModel customerChagePwdRequestModel)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int customerId = int.Parse(userId);

            var customer = await _customerRepository.GetCustomerAsync(customerId); // 加一個方法來取得資料
            if (customer == null)
            {
                return NotFound();
            }

            var passwordService = new PasswordService();

            // 驗證舊密碼是否正確
            bool isOldPasswordCorrect = passwordService.VerifyPassword(customerChagePwdRequestModel.OldPassword, customer.Password);
            if (!isOldPasswordCorrect)
            {
                ModelState.AddModelError("OldPassword", "舊密碼不正確");
                return View("ChangePassword", customerChagePwdRequestModel);
            }

            // 驗證新密碼與確認密碼是否一致
            if (customerChagePwdRequestModel.NewPassword != customerChagePwdRequestModel.ConfirmPassword)
            {
                ModelState.AddModelError("NewPassword", "新密碼與確認密碼不一致");
                return View("ChangePassword", customerChagePwdRequestModel);
            }

            // 驗證新密碼格式
            var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_])[A-Za-z\d\W_]{8,20}$");
            if (!regex.IsMatch(customerChagePwdRequestModel.NewPassword))
            {
                ModelState.AddModelError("NewPassword", "密碼需為 8-20 字，包含大小寫字母、數字及特殊符號");
                return View("ChangePassword", customerChagePwdRequestModel);
            }

            // 比對是否與舊密碼相同（防止用同一組）
            if (passwordService.VerifyPassword(customerChagePwdRequestModel.NewPassword, customer.Password))
            {
                ModelState.AddModelError("NewPassword", "新密碼不能與舊密碼相同");
                return View("ChangePassword", customerChagePwdRequestModel);
            }

            // 密碼通過驗證後，進行加密並更新
            var hashedPassword = passwordService.HashPasswordWithBCrypt(customerChagePwdRequestModel.NewPassword);

            var cusInfo = new CustomerInfo();
            cusInfo.CustomerId = customerId;
            cusInfo.Password = hashedPassword;


            await _customerRepository.UpdatePasswordAsync(cusInfo);

            TempData["PasswordChangeMessage"] = "密碼已變更，請重新登入。";
            return RedirectToAction("Logout", "Login");
        }


    }
}
