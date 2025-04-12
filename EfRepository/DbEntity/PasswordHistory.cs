using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfRepository.DbEntity
{
    public class PasswordHistory
    {
        public int Id { get; set; }

        public int? CustomerId { get; set; }

        public string HashedPassword { get; set; } = string.Empty;

        public DateTime ChangedAt { get; set; }
    }
}
