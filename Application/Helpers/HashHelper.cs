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
    }
}
