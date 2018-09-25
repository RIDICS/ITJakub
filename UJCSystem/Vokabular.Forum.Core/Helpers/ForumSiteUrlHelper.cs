using Microsoft.Extensions.Options;
using Vokabular.ForumSite.Core.Options;

namespace Vokabular.ForumSite.Core.Helpers
{
    public class ForumSiteUrlHelper
    {
        private readonly IOptions<ForumOption> m_forumOptions;
        public const string ForumSiteBaseUrl = "http://localhost:50165"; //TODO load from config file???
        private const string TopicsUrlPart = "/topics";

        public ForumSiteUrlHelper(IOptions<ForumOption> forumOptions)
        {
            m_forumOptions = forumOptions;
        }

        public string GetTopicsUrl(int forumId)
        {
            return m_forumOptions.Value.ForumBaseUrl + TopicsUrlPart + "/" + forumId + "-";
        }
    }
}