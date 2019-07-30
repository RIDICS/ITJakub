using System;

namespace Vokabular.MainService.Core.Errors
{
    public class ForumException : Exception
    {
        public ForumException()
        {
        }

        public ForumException(string message) : base(message)
        {
        }

        public ForumException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
