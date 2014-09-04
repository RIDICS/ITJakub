using System;
using System.Security.Cryptography;
using System.Text;

namespace ITJakub.MobileApps.Core.Authentication.Image
{
    public class GravatarImageUrlProvider
    {
        private readonly string m_baseUrl;

        public GravatarImageUrlProvider(string baseUrl)
        {
            m_baseUrl = baseUrl;
        }

        public string GetImageUrl(string email)
        {
            return string.Format(m_baseUrl,ComputeHash(email.Trim().ToLower()).ToLower());
        }

        private static string ComputeHash(string input)
        {
            var md5 = new MD5CryptoServiceProvider();
            var inputArray = Encoding.ASCII.GetBytes(input);
            var hashedArray = md5.ComputeHash(inputArray);
            md5.Clear();
            return BitConverter.ToString(hashedArray).Replace("-", "");
        }
    }
}