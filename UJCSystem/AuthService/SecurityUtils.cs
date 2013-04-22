using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;

namespace AuthService
{
    public class SecurityUtils
    {
        public static string HashPassword(string salt, string password)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(salt + "~-" + password);
            HashAlgorithm algorithm = new SHA256CryptoServiceProvider();
            byte[] hashedBytes = algorithm.ComputeHash(inputBytes);
            return BitConverter.ToString(hashedBytes);
        }

    }
}