using System;

namespace Vokabular.CommunicationService.OAIPMH
{
    public class OaiPmhException : Exception
    {
        public OAIPMHerrorcodeType ErrorCode { get; }

        public OaiPmhException(string message, OAIPMHerrorcodeType errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}
