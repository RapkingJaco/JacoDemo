using EfRepository.DbEntity;
using EfRepository.DbService;
using JacoBankAPI.Service;
using JacoDemo.Middleware;
using JacoDemo.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//註冊DI服務
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IBankInfoRepository, BankInfoRepository>();
builder.Services.AddScoped<ICustomerBankRepository, CustomerBankRepository>();
builder.Services.AddScoped<ICustomerTransRepository, CustomerTransRepository>();

builder.Services.AddHttpClient("JacoBankAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:7035");
});

//註冊Cookie base 登入機制
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/VerifyUser";
        options.LogoutPath = "/Login/Logout";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);

        // true; 延展逾期時間(有在操作系統就會展延)
        options.SlidingExpiration = true;
    });

//使用資料庫
builder.Services.AddDbContext<JacoBankContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("JacoBankDb")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//先身分驗證
app.UseAuthentication();
//再授權
app.UseAuthorization();

//app.UseMiddleware<ExceptionHandleMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
