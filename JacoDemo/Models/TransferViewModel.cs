namespace JacoDemo.Models
{
    public class TransferViewModel
    {
        public int FromBankInfoId { get; set; }
        public string ToBankName { get; set; }
        public string ToAccountNumber { get; set; }
        public decimal Amount { get; set; }
        public string? Note { get; set; }
    }
}
