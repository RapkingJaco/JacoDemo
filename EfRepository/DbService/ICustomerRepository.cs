using EfRepository.DbEntity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfRepository.DbService
{
    public interface ICustomerRepository
    {
        Task<CustomerInfo> CheckAccount(string userName);
        Task<CustomerInfo> CheckEmail(string email);
        Task<CustomerInfo> GetCustomerAsync(int id);
        Task<bool> CreateCustomerAsync(CustomerInfo customer);
        Task<bool> UpdateCustomerAsync(CustomerInfo customer);
        Task<bool> UpdatePasswordAsync(CustomerInfo customer);
    }
}
