using System.Text;
using Vokabular.RestClient.Extensions;

namespace Vokabular.RestClient
{
    public class UrlQueryBuilder : BuilderBase<string>
    {
        private readonly StringBuilder m_stringBuilder;
        private bool m_containsParameters;

        private UrlQueryBuilder(string url)
        {
            m_stringBuilder = new StringBuilder(url);
            m_containsParameters = url.Contains("?");
        }

        public static UrlQueryBuilder Create(string url)
        {
            return new UrlQueryBuilder(url);
        }

        protected override void AppendParameter(string name, string value)
        {
            m_stringBuilder
                .Append(m_containsParameters ? '&' : '?')
                .Append(name)
                .Append("=")
                .Append(value.EncodeQueryString());

            m_containsParameters = true;
        }

        public override string ToResult()
        {
            return m_stringBuilder.ToString();
        }
    }
}
