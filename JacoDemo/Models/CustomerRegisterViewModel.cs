namespace JacoDemo.Models
{
    public class CustomerRegisterViewModel
    {
        public int? CustomerId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
}
