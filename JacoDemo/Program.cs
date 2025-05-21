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

//���UDI�A��
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IBankInfoRepository, BankInfoRepository>();
builder.Services.AddScoped<ICustomerBankRepository, CustomerBankRepository>();
builder.Services.AddScoped<ICustomerTransRepository, CustomerTransRepository>();

builder.Services.AddHttpClient("JacoBankAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:7035");
});

//���UCookie base �n�J����
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/VerifyUser";
        options.LogoutPath = "/Login/Logout";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);

        // true; ���i�O���ɶ�(���b�ާ@�t�δN�|�i��)
        options.SlidingExpiration = true;
    });

//�ϥθ�Ʈw
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

//����������
app.UseAuthentication();
//�A���v
app.UseAuthorization();

//app.UseMiddleware<ExceptionHandleMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
