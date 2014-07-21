using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITJakub.MobileApps.Core
{
    public static class EnterCodeGenerator
    {
        private const string EnabledChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        private const int DefaultCodeLength = 10;


        public static string GenerateCode()
        {
           var random = new Random();
           return new string(Enumerable.Repeat(EnabledChars, DefaultCodeLength).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string GenerateCode(int length)
        {
            var random = new Random();
            return new string(Enumerable.Repeat(EnabledChars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
