using EfRepository.DbEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfRepository.Service
{
    public class PasswordHistoryService
    {
        private readonly JacoBankContext _jacoBankContext;

        public PasswordHistoryService(JacoBankContext jacoBankContext)
        {
            _jacoBankContext = jacoBankContext;
        }

        public async Task AddPasswordHistoryAsync(int customerId, string hashedPassword)
        {
            var history = new PasswordHistory
            {
                CustomerId = customerId,
                HashedPassword = hashedPassword,
                ChangedAt = DateTime.Now
            };

            _jacoBankContext.PasswordHistories.Add(history);
            await _jacoBankContext.SaveChangesAsync();
        }
    }
}
