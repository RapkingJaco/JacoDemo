using EfRepository;

namespace JacoDemo.Models
{
    public class BankTransInfoViewModel
    {
        // 查詢條件
        public string? AccountNumber { get; set; }
        public string? TransType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? ReceiverBankName { get; set; }
        public string? ReceiverAccount { get; set; }

        public bool IsCrossBank { get; set; }

        // 查詢結果列表
        public List<BankTransInfo> ResultList { get; set; } = new();
    }
}
