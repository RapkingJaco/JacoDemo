using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfRepository
{
    public class BankTransInfo
    {
        // CustomerBankInfo 的欄位
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } 
        public int BankInfoId { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }

        // CustomerTransInfo 的欄位
        public int TransId { get; set; }
        public DateTime? TransDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string TransType { get; set; }
        public decimal Amount { get; set; }
        public decimal BalanceAfter { get; set; }
        public string Note { get; set; }
        public string? ReceiverBankName { get; set; }
        public string? ReceiverAccount { get; set; }
        public bool IsCrossBank { get; set; }
    }
}
