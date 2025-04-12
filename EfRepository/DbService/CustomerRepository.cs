using EfRepository.Service;
using EfRepository.DbEntity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfRepository.DbService
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly JacoBankContext _JacoBankContext;
        private readonly PasswordHistoryService _passwordHistoryService;

        public CustomerRepository(JacoBankContext jacoBankContext)
        {
            _JacoBankContext = jacoBankContext;
            _passwordHistoryService = new PasswordHistoryService(jacoBankContext);
        }

        //檢查帳號是否重複
        public async Task<CustomerInfo> CheckAccount(string userName)
        {
            
            var customer = await _JacoBankContext.CustomerInfos.FirstOrDefaultAsync(c => c.Name == userName);

            return customer;

        }

        //檢查Email是否重複
        public async Task<CustomerInfo> CheckEmail(string email)
        {
            
            var customer = await _JacoBankContext.CustomerInfos.FirstOrDefaultAsync(c => c.Email == email);

            return customer;


        }

        //註冊
        public async Task<bool> CreateCustomerAsync(CustomerInfo customer)
        {
            await _JacoBankContext.CustomerInfos.AddAsync(customer);            
            await _passwordHistoryService.AddPasswordHistoryAsync((int)customer.CustomerId, customer.Password);
            await _JacoBankContext.SaveChangesAsync();

            return true;
        }

        //檢查Email是否重複
        public async Task<CustomerInfo> GetCustomerAsync(string email)
        {
            
            var customer = await _JacoBankContext.CustomerInfos.FirstOrDefaultAsync(c => c.Email == email);
            return customer;

        }

        //檢查使用者ID
        public async Task<CustomerInfo> GetCustomerAsync(int id)
        {
            var customer = await _JacoBankContext.CustomerInfos.FirstOrDefaultAsync(c => c.CustomerId == id);
            return customer;
        }

        //更新使用者資料
        public async Task<bool> UpdateCustomerAsync(CustomerInfo customer)
        {
            var existingCustomer = await _JacoBankContext.CustomerInfos.FindAsync(customer.CustomerId);
            if (existingCustomer != null)
            {
                existingCustomer.Name = customer.Name;
                existingCustomer.Email = customer.Email;
                existingCustomer.Phone = customer.Phone;
                existingCustomer.CreatedAt = DateTime.Now;
                await _JacoBankContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> UpdatePasswordAsync(CustomerInfo customer)
        {
            var existingCustomer = await _JacoBankContext.CustomerInfos.FindAsync(customer.CustomerId);
            if (existingCustomer == null) return false;

            var passwordService = new PasswordService();

            // 取得上一筆密碼歷史
            var lastHistory = await _JacoBankContext.PasswordHistories
                .Where(ph => ph.CustomerId == customer.CustomerId)
                .OrderByDescending(ph => ph.ChangedAt)
                .FirstOrDefaultAsync();

            if (lastHistory != null && passwordService.VerifyPassword(customer.Password, lastHistory.HashedPassword))
            {
                // 新密碼與舊密碼相同
                return false;
            }

            var hashedPassword = passwordService.HashPasswordWithBCrypt(customer.Password);

            // 更新主密碼
            existingCustomer.Password = hashedPassword;
            existingCustomer.CreatedAt = DateTime.Now;

            // 寫入密碼歷史
            var passwordHistory = new PasswordHistory
            {
                CustomerId = customer.CustomerId,
                HashedPassword = hashedPassword,
                ChangedAt = DateTime.Now
            };
            _JacoBankContext.PasswordHistories.Add(passwordHistory);

            await _JacoBankContext.SaveChangesAsync();
            return true;
        }

    }
}
