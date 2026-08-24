
using Abstractions;
using System.Security.Cryptography;
using System.Text;

namespace Services
{
    public class PlainTextPasswordService : IPasswordService
    {
        //Logica de hash
        public string Hash(string plainTextPassword)
        {
            if (string.IsNullOrEmpty(plainTextPassword))
                return string.Empty;

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(plainTextPassword));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                    builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }
    }
}