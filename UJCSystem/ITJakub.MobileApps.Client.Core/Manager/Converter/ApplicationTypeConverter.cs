using System;
using ITJakub.MobileApps.Client.Shared.Enum;

namespace ITJakub.MobileApps.Client.Core.Manager.Converter
{
    public static class ApplicationTypeConverter
    {
        public static string ConvertToString(ApplicationType applicationType)
        {
            //TODO better conversion
            switch (applicationType)
            {
                case ApplicationType.Chat:
                    return "1";
                case ApplicationType.Crosswords:
                    return "3";
                case ApplicationType.Hangman:
                    return "2";
                default:
                    throw new ArgumentException("Unknown application.");
            }
        }
    }
}