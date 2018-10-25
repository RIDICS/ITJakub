using System;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.ForumSite.Core.Helpers
{
    public class MessageGenerator
    {
        public string GetMessage(ProjectDetailContract project, short bookType, string hostUrl)
        {
            string authors = "";
            if (project.Authors != null)
            {
                foreach (var author in project.Authors)
                {
                    authors += author.FirstName + " " + author.LastName + Environment.NewLine;
                }
            }

            return $@"{project.Name}
[url={VokabularUrlHelper.GetBookUrl(project.Id, bookType, hostUrl)}]Odkaz na knihu ve Vokabuláři webovém[/url]
{(project.Authors == null ? "Autor: <Nezadáno>" : (project.Authors.Count == 1 ? "Autor:" : "Autoři:"))} {authors}
Počet stran: {(project.PageCount == null ? "<Nezadáno>" : project.PageCount.ToString())}";
        }
    }
}
