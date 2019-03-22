using System;
using Vokabular.OaiPmhImportManager.Model;

namespace Vokabular.OaiPmhImportManager
{
    public class OaiPmhException : Exception
    {
        public OaiPmhException(string message, OAIPMHerrorcodeType errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }

        public OaiPmhException(string message) : base(message)
        {
        }

        public OAIPMHerrorcodeType ErrorCode { get; }
    }
}
