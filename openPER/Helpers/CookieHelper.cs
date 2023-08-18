using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace openPER.Helpers
{
    public class CookieHelper
    {
        public static string DecryptString(string cipherText, string keyString)
        {
            var fullCipher = Convert.FromBase64String(cipherText);

            var key = Encoding.UTF8.GetBytes(keyString);

            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                var result = aesAlg.DecryptEcb(fullCipher, PaddingMode.PKCS7);
                return Encoding.UTF8.GetString(result);
            }
        }
    }
}
