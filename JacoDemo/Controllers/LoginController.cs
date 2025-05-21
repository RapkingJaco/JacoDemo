using EfRepository.DbEntity;
using EfRepository.DbService;
using EfRepository.Service;
using JacoDemo.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace JacoDemo.Controllers
{
    public class LoginController : Controller
    {
        private readonly ICustomerRepository _customerRepository;

        //與EfRepository溝通
        public LoginController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        // GET: /<controller>/
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        //顯示註冊畫面
        [HttpGet]
        public IActionResult CustomerRegister()
        {
            return View();
        }

        //註冊按鈕送出
        [HttpPost]
        public async Task<IActionResult> Register(CustomerRegisterViewModel customerRegisterViewModel)
        {
            var customerAccount = await _customerRepository.CheckAccount(customerRegisterViewModel.UserName);
            var customerEmail = await _customerRepository.CheckEmail(customerRegisterViewModel.Email);
            if (customerRegisterViewModel.Password != customerRegisterViewModel.ConfirmPassword)
            {
                ModelState.AddModelError("Password", "密碼不一致");
                return View("CustomerRegister", customerRegisterViewModel);
            }

            //檢查帳號是否重複
            if (customerAccount != null)
            {
                ModelState.AddModelError("UserName", "此使用者名稱已存在");
                return View("CustomerRegister", customerRegisterViewModel);
            }

            //檢查Email是否重複
            if (customerEmail != null)
            {
                ModelState.AddModelError("Email", "此電子郵件已存在");
                return View("CustomerRegister", customerRegisterViewModel);
            }

            string password = customerRegisterViewModel.Password;

            // 至少 8 碼，包含大小寫、數字和特殊符號
            var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_])[A-Za-z\d\W_]{8,20}$");

            if (!regex.IsMatch(password))
            {
                ModelState.AddModelError("Password", "密碼需為 8-20 字，包含大小寫字母、數字及特殊符號");
                return View("CustomerRegister", customerRegisterViewModel);
            }
            else
            {
                var passwordService = new PasswordService();
                var hashedPassword = passwordService.HashPasswordWithBCrypt(customerRegisterViewModel.Password);

                var customer = new CustomerInfo();
                                
                customer.Name = customerRegisterViewModel.UserName;
                customer.Password = hashedPassword;
                customer.Phone = customerRegisterViewModel.Phone;
                customer.Email = customerRegisterViewModel.Email;
                customer.CreatedAt = DateTime.Now;

                //建立客戶資料和歷史密碼
                await _customerRepository.CreateCustomerAsync(customer);

                // 註冊成功，使用 TempData 傳遞訊息
                TempData["SuccessMessage"] = "註冊成功，請登入！";
            }
            return RedirectToAction("CustomerRegister", "Login");
        }

        [HttpPost]
        //因有填寫表單，需加上[AutoValidateAntiforgeryToken]，以防 CSRF 攻擊確保資料正確性
        [AutoValidateAntiforgeryToken]
        //登入驗證、檢查帳號密碼
        public async Task<IActionResult> VerifyUser(LoginRequestModel loginRequestModel, string returnUrl)
        {            
            var customerLogin = await _customerRepository.CheckEmail(loginRequestModel.Account);
            if (customerLogin == null)
            {
                ModelState.AddModelError("Account", "電子郵件不存在");
                return View("Index", loginRequestModel);
            }
            //密碼驗證
            var passwordService = new PasswordService();
            if (!passwordService.VerifyPassword(loginRequestModel.Password, customerLogin.Password))
            {
                ModelState.AddModelError("Password", "密碼錯誤");
                return View("Index", loginRequestModel);
            }

            //Login 成功，紀錄目前使用者
            Claim[] claims = new Claim[]
            {
                new Claim(ClaimTypes.Name, customerLogin.Name),
                new Claim(ClaimTypes.NameIdentifier, customerLogin.CustomerId.ToString())          
            };

            //儲存登入憑證
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

            //若有接收轉想網址參數、則基於資安考量，不允許轉向外部網址
            //Url.IsLocalUrl(xxxx); //true：本地網址，false：外部網址
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        //登出
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }
    }
}
