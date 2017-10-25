using System;

namespace Jewelery
{
    public static class SettingKeys
    {
        public const string DefaultConnectionString = "DefaultConnectionString";
        public const string MainConnectionString = "MainConnectionString";
        public const string WebConnectionString = "WebConnectionString";
        public const string TestDbConnectionString = "TestDbConnectionString";
        public const string ExistDbUser = "ExistDbUser";
        public const string ExistDbPassword = "ExistDbPassword";
        public const string CardFilesUser = "CardFilesUser";
        public const string CardFilesPassword = "CardFilesPassword";

        public static string GetStringOrThrowArgumentException(this string value, string exceptionMessage)
        {
            if (value == null)
                throw new ArgumentException(exceptionMessage);
            return value;
        }
    }
}
