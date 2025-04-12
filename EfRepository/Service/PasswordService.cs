using BCrypt.Net;

namespace EfRepository.Service
{
    public class PasswordService
    {
        //密碼雜湊
        public string HashPasswordWithBCrypt(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
