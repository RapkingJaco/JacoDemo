namespace JacoDemo.Models
{
    public class CustomerChagePwdRequestModel
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
