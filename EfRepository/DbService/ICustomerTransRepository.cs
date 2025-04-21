using EfRepository.DbEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfRepository.DbService
{
    public interface ICustomerTransRepository
    {
        Task AddTransactionAsync(CustomerTransInfo customerTransInfo);
    }
}
