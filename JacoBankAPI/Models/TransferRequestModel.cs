namespace JacoBankAPI.Models
{
    public class TransferRequestModel
    {
        public int SenderCustomerId { get; set; }
        public int FromBankInfoId { get; set; }
        public string ToBankName { get; set; }
        public string ToAccountNumber { get; set; }
        public decimal Amount { get; set; }
        public string? Note { get; set; }
    }
}
