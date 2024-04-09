using System.Text;

namespace SalesHelper.Services.EncryptionServices
{
    public class EncryptionDecryptionService
    {

        public static string key = "myscrectkey";
        public string Encrypt(string simplePassword)
        {
            if (string.IsNullOrEmpty(simplePassword)) return string.Empty;
            simplePassword += key;
            var passwordBytes = Encoding.UTF8.GetBytes(simplePassword);
            return Convert.ToBase64String(passwordBytes);
        }
        public string Decrypt(string encodedPassword)
        {
            if (string.IsNullOrEmpty(encodedPassword)) return string.Empty;
            var passwordBytes = Convert.FromBase64String(encodedPassword);
            var simplePassword = Encoding.UTF8.GetString(passwordBytes);
            simplePassword = simplePassword.Substring(0, simplePassword.Length - key.Length);
            return simplePassword;
        }
    }
}
