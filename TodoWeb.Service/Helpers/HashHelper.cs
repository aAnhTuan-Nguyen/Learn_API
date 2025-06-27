using BCrypt.Net;
using System.Text;

namespace TodoWeb.Application.Helpers
{
    public static class HashHelper
    {
        public static string HashBcrypt(string passwrod)
        {
            return BCrypt.Net.BCrypt.HashPassword(passwrod);
        }

        public static bool BCryptVerify(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

        public static string GenerateRandomString(int length)
        {
            StringBuilder s = new StringBuilder();
            var random = new Random();
            for (int i = 0; i < length; i++)
            {
                s.Append((char)random.Next(32, 127));
            }
            return s.ToString();
        }

        public static string HashSha256(string input)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                return Convert.ToBase64String(bytes);
            }
            //byte[] bytes = System.Security.Cryptography.SHA256.HashData(Encoding.UTF8.GetBytes(input));
            //string hash =  Convert.ToBase64String(bytes);
            //return hash;
        }
    }
}
