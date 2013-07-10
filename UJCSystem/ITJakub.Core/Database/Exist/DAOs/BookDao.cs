using System.Collections.Generic;
using System.Text;
using System.Xml;
using Castle.MicroKernel;
using ITJakub.Contracts.Categories;
using ITJakub.Contracts.Searching;

namespace ITJakub.Core.Database.Exist.DAOs
{
    public class BookDao : ExistDaoBase
    {
        public BookDao(IKernel container) : base(container)
        {
        }

        public string GetTitleByBookId(string id)
        {
            string query = GetTitleQuery(id);
            string dbResult = ExistDao.QueryXml(query);

            XmlDocument dbXmlResult = new XmlDocument();
            dbXmlResult.LoadXml(dbResult);

            XmlNamespaceManager nManager = new XmlNamespaceManager(dbXmlResult.NameTable);
            foreach (var allNamespace in Descriptor.GetAllNamespaces())
            {
                nManager.AddNamespace(allNamespace.Key, allNamespace.Value);
            }

            var hit = dbXmlResult.SelectSingleNode("//title");
            string result = XmlTool.ParseTeiTitle(hit, TeiP5Descriptor.TitleNodeName, nManager);
            return result;
        }

        private string GetTitleQuery(string id)
        {
            StringBuilder builder = new StringBuilder();
            AddNamespacesAndCollation(builder);
            builder.AppendLine(string.Format("let $collections:= string(\"{0}\")", Descriptor.GetDataLocation));

            builder.AppendLine(string.Format("let $words := collection($collections)//tei:TEI[@n='{0}']", id));
            builder.AppendLine("for $hit in $words");
            builder.AppendLine("let $title := $hit/ancestor-or-self::tei:TEI/tei:teiHeader/tei:fileDesc/tei:titleStmt/tei:title");
            builder.AppendLine("return <title>{$title}</title>");

            return builder.ToString();
        }

        public IEnumerable<SearchResult> GetAllBooksContainsTerm(string searchTerm)
        {
            string query = GetBooksContainsTermQuery(searchTerm);
            string dbResult = ExistDao.QueryXml(query);

            XmlDocument dbXmlResult = new XmlDocument();
            dbXmlResult.LoadXml(dbResult);

            XmlNamespaceManager nManager = new XmlNamespaceManager(dbXmlResult.NameTable);
            foreach (var allNamespace in Descriptor.GetAllNamespaces())
            {
                nManager.AddNamespace(allNamespace.Key, allNamespace.Value);
            }

            List<SearchResult> results = new List<SearchResult>();
            XmlNodeList hits = dbXmlResult.SelectNodes("//hit");
            if (hits != null)
                foreach (XmlNode hit in hits)
                {
                    SearchResult result = new SearchResult();
                    result.Author = XmlTool.ParseTeiAuthor(hit.SelectSingleNode("//authors"), TeiP5Descriptor.AuthorNodeName, nManager);
                    result.Title = XmlTool.ParseTeiTitle(hit.SelectSingleNode("//title"), TeiP5Descriptor.TitleNodeName, nManager);
                    result.Id = XmlTool.ParseId(hit.SelectSingleNode("//id"), TeiP5Descriptor.IdAttributeName);

                    results.Add(result);
                }

            return results;
        }

        private string GetBooksContainsTermQuery(string word)
        {
            StringBuilder builder = new StringBuilder();
            AddNamespacesAndCollation(builder);


            builder.AppendLine("import module namespace kwic=\"http://exist-db.org/xquery/kwic\";");

            builder.AppendLine("<words>{");
            builder.AppendLine("let $query :=");
            builder.AppendLine("<query>");
            builder.AppendLine("<bool>");
            builder.AppendLine(string.Format("<wildcard occur='must'>{0}</wildcard>", word));
            builder.AppendLine("</bool>");
            builder.AppendLine("</query>");

            builder.AppendLine(string.Format("let $collections:= string(\"{0}\")", Descriptor.GetDataLocation));

            builder.AppendLine("let $words := collection($collections)/tei:TEI/tei:text/tei:body//tei:w[ft:query(@nlp:lemma, $query)]");
            builder.AppendLine("for $hit in $words");
            builder.AppendLine("let $title := $hit/ancestor-or-self::tei:TEI/tei:teiHeader/tei:fileDesc/tei:titleStmt/tei:title");
            builder.AppendLine("let $id := $hit/ancestor-or-self::tei:TEI/@n");
            builder.AppendLine("let $author := $hit/ancestor-or-self::tei:TEI//tei:author");
            builder.AppendLine("let $categories := $hit/ancestor-or-self::tei:TEI/tei:teiHeader/tei:profileDesc/tei:textClass/tei:catRef");

            builder.AppendLine("order by $hit");
            builder.AppendLine("return <hit>");
            builder.AppendLine("<id>{$id}</id>");
            builder.AppendLine("<title>{$title}</title>");
            builder.AppendLine("<authors>{$author}</authors>");
            builder.AppendLine("<categories>{$categories}</categories>");
            builder.AppendLine("</hit>");
            builder.AppendLine();
            builder.AppendLine("}");
            builder.AppendLine("</words>");


            return builder.ToString();
        }

        public IEnumerable<SearchResult> GetAllBooksByAuthorFirstLetter(string letter)
        {
            string query = GetBooksByAuthorFirstLetterQuery(letter);
            string dbResult = ExistDao.QueryXml(query);

            XmlDocument dbXmlResult = new XmlDocument();
            dbXmlResult.LoadXml(dbResult);

            XmlNamespaceManager nManager = new XmlNamespaceManager(dbXmlResult.NameTable);
            foreach (var allNamespace in Descriptor.GetAllNamespaces())
            {
                nManager.AddNamespace(allNamespace.Key, allNamespace.Value);
            }

            List<SearchResult> results = new List<SearchResult>();
            XmlNodeList hits = dbXmlResult.SelectNodes("//hit");
            if (hits != null)
                foreach (XmlNode hit in hits)
                {
                    SearchResult result = new SearchResult();
                    result.Author = XmlTool.ParseTeiAuthor(hit.SelectSingleNode("//authors"), TeiP5Descriptor.AuthorNodeName, nManager);
                    result.Title = XmlTool.ParseTeiTitle(hit.SelectSingleNode("//title"), TeiP5Descriptor.TitleNodeName, nManager);
                    result.Id = XmlTool.ParseId(hit.SelectSingleNode("//id"), TeiP5Descriptor.IdAttributeName);

                    results.Add(result);
                }


            return results;
        }

        public string GetBooksByAuthorFirstLetterQuery(string letter)
        {
            StringBuilder builder = new StringBuilder();
            AddNamespacesAndCollation(builder);

            builder.AppendLine("<books>{");

            builder.AppendLine(string.Format("let $letter := \"{0}\"", letter));
            builder.AppendLine(string.Format("let $collections:= string(\"{0}\")", Descriptor.GetDataLocation));
            builder.AppendLine("let $hits := collection($collections)/tei:TEI");
            builder.AppendLine("for $hit in $hits");

            builder.AppendLine("let $titleWord := $hit/ancestor-or-self::tei:TEI/tei:teiHeader/tei:fileDesc/tei:titleStmt/tei:title[1]/tei:w[1]");
            builder.AppendLine("let $title := $hit/ancestor-or-self::tei:TEI/tei:teiHeader/tei:fileDesc/tei:titleStmt/tei:title");
            builder.AppendLine("let $id := $hit/ancestor-or-self::tei:TEI/@n");
            builder.AppendLine("let $author := $hit/ancestor-or-self::tei:TEI//tei:author");

            builder.AppendLine("where fn:starts-with(fn:upper-case($titleWord),fn:upper-case($letter))");

            builder.AppendLine("order by $hit");
            builder.AppendLine("return <hit>");
            builder.AppendLine("<id>{$id}</id>");
            builder.AppendLine("<title>{$title}</title>");
            builder.AppendLine("<authors>{$author}</authors>");
            builder.AppendLine("</hit>");

            builder.AppendLine("}</books>");

            return builder.ToString();
        }
    }
}