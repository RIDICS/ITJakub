using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using ITJakub.Contracts.Searching;

namespace ITJakub.Core.Database.Exist.DAOs
{
    public abstract class ExistDaoBase
    {
        protected readonly TeiP5Descriptor Descriptor;
        protected readonly ExistDao ExistDao;

        protected ExistDaoBase(ExistDao existDao, TeiP5Descriptor descriptor)
        {
            ExistDao = existDao;
            Descriptor = descriptor;
        }

        protected void AddNamespacesAndCollation(StringBuilder builder)
        {
            builder.AppendLine(string.Format("declare default collation \"{0}\";", Descriptor.GetCollation()));
            foreach (KeyValuePair<string, string> allNamespace in Descriptor.GetAllNamespaces())
            {
                builder.AppendLine(string.Format("declare namespace {0} = \"{1}\";", allNamespace.Key, allNamespace.Value));
            }
        }
    }

    public class ExistWordsDao : ExistDaoBase
    {
        private const int KeyWordValue = 45;

        public ExistWordsDao(ExistDao existDao, TeiP5Descriptor descriptor) : base(existDao, descriptor)
        {
        }

        public List<string> GetAllPossibleKeyWords(string input)
        {
            string query = GetAllPossibleKeywordsQuery(input);

            string dbResult = ExistDao.QueryXml(query);
            var result = XmlTool.CutElementsText(dbResult, "word");

            return result.ToList();
        }

        public SearchResult[] GetKeyWordInContextByWord(string word)
        {
            string query = GetKeyWordInContextQuery(word);
            string dbResult = ExistDao.QueryXml(query);

            List<SearchResult> results = new List<SearchResult>();

            XmlDocument dbXmlResult = new XmlDocument();
            dbXmlResult.LoadXml(dbResult);

            XmlNamespaceManager nManager = new XmlNamespaceManager(dbXmlResult.NameTable);
            foreach (var allNamespace in Descriptor.GetAllNamespaces())
            {
                nManager.AddNamespace(allNamespace.Key, allNamespace.Value);
            }

            XmlNodeList hits = dbXmlResult.SelectNodes("//hit");
            if (hits != null)
                foreach (XmlNode hit in hits)
                {
                    SearchResult result = new SearchResult();
                    result.Author = XmlTool.ParseTeiAuthor(hit.SelectSingleNode("//author"), TeiP5Descriptor.AuthorNodeName, nManager);
                    result.Title = XmlTool.ParseTeiTitle(hit.SelectSingleNode("//title"), TeiP5Descriptor.TitleNodeName, nManager);
                    result.Id = XmlTool.ParseId(hit.SelectSingleNode("//id"), TeiP5Descriptor.IdAttributeName);
                    result.Categories = XmlTool.ParseTeiCategoriesIds(hit.SelectSingleNode("//categories"), TeiP5Descriptor.CategoriesNodeName, TeiP5Descriptor.CategoriesTargetAttributName, nManager);
                    result.Kwic = XmlTool.ParseKwicStructure(hit.SelectSingleNode("kwic"), TeiP5Descriptor.ParagraphNodeName, nManager);
                    result.OriginalXml = hit.InnerXml;

                    results.Add(result);
                }


            XmlDocument xmlResult = new XmlDocument();
            xmlResult.LoadXml(dbResult);

            return results.ToArray();
        }

        private string GetAllPossibleKeywordsQuery(string input)
        {
            StringBuilder builder = new StringBuilder();

            AddNamespacesAndCollation(builder);
            builder.AppendLine("<words> {");
            builder.AppendLine("let $query :=");
            builder.AppendLine("<query>");
            builder.AppendLine("<bool>");
            builder.AppendLine(string.Format("<wildcard occur='must'>{0}</wildcard>", input));
            builder.AppendLine("</bool>");
            builder.AppendLine("</query>");
            builder.AppendLine("let $words := collection(\"\")//tei:w[ft:query(., $query)]");
            builder.AppendLine("for $word in distinct-values($words)");
            builder.AppendLine("order by $word");
            builder.AppendLine("return <word>{$word}</word>");
            builder.AppendLine("} </words>");

            return builder.ToString();
        }

        private string GetKeyWordInContextQuery(string word)
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
            builder.AppendLine("let $words := collection(\"\")//tei:w[ft:query(., $query)]");
            builder.AppendLine("for $hit in $words");
            builder.AppendLine("let $expanded := kwic:expand($hit/..)");
            builder.AppendLine("let $title := $hit/ancestor::tei:TEI/tei:teiHeader/tei:fileDesc/tei:titleStmt/tei:title");
            builder.AppendLine("let $id := $hit/ancestor::tei:TEI/tei:teiHeader/tei:fileDesc/@n");
            builder.AppendLine("let $author := $hit/ancestor::tei:TEI//tei:author");
            builder.AppendLine("let $categories := $hit/ancestor::tei:TEI/tei:teiHeader/tei:profileDesc/tei:textClass/tei:catRef");
            builder.AppendLine(string.Format("let $kwic:= kwic:get-summary($expanded, ($expanded//exist:match), <config width=\"{0}\"/>)", KeyWordValue));
            builder.AppendLine("order by $hit");
            builder.AppendLine("return <hit>");
            builder.AppendLine("<title>{$title}</title>");
            builder.AppendLine("<id>{$id}</id>");
            builder.AppendLine("<author>{$author}</author>");
            builder.AppendLine("<categories>{$categories}</categories>");
            builder.AppendLine("<kwic>{$kwic}</kwic>");
            builder.AppendLine("</hit>");
            builder.AppendLine();
            builder.AppendLine("}");
            builder.AppendLine("</words>");
            

            return builder.ToString();
        }
    }

    public class BookDao : ExistDaoBase
    {
        public BookDao(ExistDao existDao, TeiP5Descriptor descriptor) : base(existDao, descriptor)
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
            builder.AppendLine(string.Format("let $words := collection(\"\")//tei:fileDesc[@n='{0}']", id));
            builder.AppendLine("for $hit in $words");
            builder.AppendLine("let $title := $hit/ancestor::tei:TEI/tei:teiHeader/tei:fileDesc/tei:titleStmt/tei:title");
            builder.AppendLine("return <title>{$title}</title>");

            return builder.ToString();
        }
    }
}
