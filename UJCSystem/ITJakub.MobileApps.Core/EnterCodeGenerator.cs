using System;
using System.Linq;

namespace ITJakub.MobileApps.Core
{
    public class EnterCodeGenerator
    {
        private readonly int m_codeLength;
        private readonly string m_enabledChars;
        private const string DefaultEnabledChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        private const int DefaultCodeLength = 10;

        public EnterCodeGenerator(int codeLength=DefaultCodeLength, string enabledChars=DefaultEnabledChars)
        {
            m_codeLength = codeLength;
            m_enabledChars = enabledChars;
        }

        public string GenerateCode()
        {
            return GenerateCode(m_codeLength);
        }

        private string GenerateCode(int length)
        {
            var random = new Random();
            return new string(Enumerable.Repeat(m_enabledChars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}