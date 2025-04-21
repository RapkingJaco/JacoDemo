using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfRepository.DbService
{
    public interface IBankInfoRepository
    {
        Task<List<BankTransInfo>> GetTransListAsync(BankTransInfo bankTransInfo);
    }
}
