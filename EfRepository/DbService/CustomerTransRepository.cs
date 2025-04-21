using EfRepository.DbEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfRepository.DbService
{
    public class CustomerTransRepository : ICustomerTransRepository
    {
        private readonly JacoBankContext _jacoBankContext;

        public CustomerTransRepository(JacoBankContext jacoBankContext)
        {
            _jacoBankContext = jacoBankContext;
        }
        public async Task AddTransactionAsync(CustomerTransInfo customerTransInfo)
        {
            await _jacoBankContext.CustomerTransInfos.AddAsync(customerTransInfo);
        }
    }
}
