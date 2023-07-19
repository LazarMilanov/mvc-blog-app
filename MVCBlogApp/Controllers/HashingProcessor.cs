using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace MVCBlogApp.Controllers
{
    public class HashingProcessor
    {

        public static string CreateSalt()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] salt = new byte[16];
            rng.GetBytes(salt);
            rng.Dispose();
            return Convert.ToBase64String(salt);
        }

        public static string GenerateHash(string input, string salt)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input + salt);
            SHA256Managed sHA256ManagedString = new SHA256Managed();
            byte[] hash = sHA256ManagedString.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public static bool AreEqual(string plainTextInput, string hashedInput, string salt)
        {
            string newHashedPin = GenerateHash(plainTextInput, salt);
            return newHashedPin.Equals(hashedInput);
        }

        public static string RandomTokenHex(int size = 16) {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] randomText = new byte[size];
            rng.GetBytes(randomText);
            string randomString = Convert.ToBase64String(randomText);

            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            randomString = rgx.Replace(randomString, "");

            rng.Dispose();

            return randomString;
            
        }
    }
}