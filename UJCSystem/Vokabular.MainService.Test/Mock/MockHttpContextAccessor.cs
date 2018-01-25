using System;
using Microsoft.AspNetCore.Http;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.MainService.Core.Managers.Authentication;

namespace Vokabular.MainService.Test.Mock
{
    internal class MockHttpContextAccessor : IHttpContextAccessor
    {
        public HttpContext HttpContext { get; set; }
    }

    internal class MockCommunicationTokenProvider : ICommunicationTokenProvider
    {
        public string GetCommunicationToken()
        {
            return "communication-token-1";
        }
    }

    internal class MockCommunicationTokenGenerator : ICommunicationTokenGenerator
    {
        public string GetNewCommunicationToken(User dbUser)
        {
            return $"mock-communication-token-{Guid.NewGuid()}";
        }

        public bool ValidateTokenFormat(string token)
        {
            return !string.IsNullOrWhiteSpace(token);
        }
    }
}
