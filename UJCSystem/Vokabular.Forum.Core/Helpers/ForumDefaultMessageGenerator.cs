using System.Text;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.ForumSite.Core.Helpers
{
    /// <summary>
    /// Provides default messages which will be posted to the forum thread. This messages are hardcoded (in one language).
    /// </summary>
    public class ForumDefaultMessageGenerator
    {
        private readonly VokabularUrlHelper m_vokabularUrlHelper;

        public ForumDefaultMessageGenerator(VokabularUrlHelper vokabularUrlHelper)
        {
            m_vokabularUrlHelper = vokabularUrlHelper;
        }

        public string GetCreateMessage(ProjectDetailContract project, short bookTypeForReader, string hostUrl)
        {
            var authorsBuilder = new StringBuilder();
            if (project.Authors != null)
            {
                foreach (var author in project.Authors)
                {
                    authorsBuilder
                        .Append(author.FirstName)
                        .Append(' ')
                        .Append(author.LastName)
                        .AppendLine();
                }
            }

            var bookUrl = m_vokabularUrlHelper.GetBookUrl(project.Id, bookTypeForReader, hostUrl);
            var pageCount = project.PageCount == null ? "<Nezadáno>" : project.PageCount.ToString();
            var authorsLabel = project.Authors == null || project.Authors.Count == 1 ? "Autor:" : "Autoři:";
            var authors = authorsBuilder.ToString();

            if (authors.Length == 0)
            {
                authors = "<Nezadáno>";
            }

            return $@"{project.Name}
[url={bookUrl}]Odkaz na knihu ve Vokabuláři webovém[/url]
{authorsLabel}: {authors}
Počet stran: {pageCount}";
        }

        public string GetUpdateMessage(ProjectDetailContract project, short bookType, string hostUrl)
        {
            var bookUrl = m_vokabularUrlHelper.GetBookUrl(project.Id, bookType, hostUrl);

            return $@"Nová publikace: {project.Name}
[url={bookUrl}]Odkaz na knihu ve Vokabuláři webovém[/url]";
        }

    }
}