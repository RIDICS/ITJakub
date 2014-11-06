using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Castle.MicroKernel;
using ITJakub.Shared.Contracts.Searching;
using ITJakub.Xml.XMLOperations;

namespace ITJakub.Core.Database.Exist.DAOs
{
    public class ExistWordsDao : ExistDaoBase
    {
        private const int KeyWordValue = 45;
        private readonly XslTransformDirector m_xsltTransformer;

        public ExistWordsDao(IKernel container) : base(container)
        {
            m_xsltTransformer = container.Resolve<XslTransformDirector>();
        }


        public List<string> GetAllPossibleKeyWords(string key, List<string> booksIds = null)
        {
            var restriction = GetRestrictionByBookIds(booksIds);
            string query = GetAllPossibleKeyWordsByLemmaAttributeQuery(key, restriction);
            string dbResult = ExistDao.QueryXml(query);
            var result = XmlTool.CutElementsText(dbResult, "word");
            return result.ToList();
        }

        public List<string> GetAllPossibleIds(string key, List<string> bookIds = null)
        {
            var restrictions = GetRestrictionByBookIds(bookIds);
            string query = GetAllPossibleIdsByLemmaAttributeQuery(key, restrictions);
            string dbResult = ExistDao.QueryXml(query);
            var result = XmlTool.CutElementsText(dbResult, "id");
            return result.ToList();
        }

        private static string GetRestrictionByBookIds(List<string> booksIds)
        {
            string restriction = string.Empty;
            if (booksIds != null && booksIds.Count > 0)
            {
                string idCollection = string.Join(",", booksIds.Select(id => string.Format("\"{0}\"", id)));

                restriction = string.Format("where $id = ({0})", idCollection);
            }
            return restriction;
        }

        public List<SearchResultWithKwicContext> GetKeyWordInContextByWord(string word, List<string> booksIds = null)
        {
            var restriction = GetRestrictionByBookIds(booksIds);

            string query = GetKeyWordInContextQuery(word, restriction);
            string dbResult = ExistDao.QueryXml(query);

            List<SearchResultWithKwicContext> results = new List<SearchResultWithKwicContext>();

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
                    SearchResultWithKwicContext result = new SearchResultWithKwicContext();
                    result.Author = XmlTool.ParseTeiAuthor(hit.SelectSingleNode("//author"), TeiP5Descriptor.AuthorNodeName, nManager);
                    result.Title = XmlTool.ParseTeiTitle(hit.SelectSingleNode("//title"), TeiP5Descriptor.TitleNodeName, nManager);
                    result.Id = XmlTool.ParseId(hit.SelectSingleNode("//id"), TeiP5Descriptor.IdAttributeName);
                    result.Categories = XmlTool.ParseTeiCategoriesIds(hit.SelectSingleNode("//categories"), TeiP5Descriptor.CategoriesNodeName, TeiP5Descriptor.CategoriesTargetAttributName, nManager);
                    result.Kwic = XmlTool.ParseKwicStructure(hit.SelectSingleNode("kwic"), TeiP5Descriptor.ParagraphNodeName, nManager);

                    results.Add(result);
                }

            XmlDocument xmlResult = new XmlDocument();
            xmlResult.LoadXml(dbResult);

            return results;
        }

        public List<SearchResultWithHtmlContext> GetHtmlContextByWord(string word, List<string> bookIds = null)
        {
            var restriction = GetRestrictionByBookIds(bookIds);
            string query = GetXmlContextForWordQueryV2(word, restriction);
            string dbResult = ExistDao.QueryXml(query);

            XmlDocument dbXmlResult = new XmlDocument();
            dbXmlResult.LoadXml(dbResult);

            XmlNamespaceManager nManager = new XmlNamespaceManager(dbXmlResult.NameTable);
            foreach (var allNamespace in Descriptor.GetAllNamespaces())
            {
                nManager.AddNamespace(allNamespace.Key, allNamespace.Value);
            }

            return ConstructResultsWithHtmlContext(dbXmlResult, nManager, word);
        }

        public List<SearchResultWithXmlContext> GetXmlContextByWord(string word, List<string> booksIds = null)
        {
            var restriction = GetRestrictionByBookIds(booksIds);
            string query = GetXmlContextForWordQuery(word, restriction);
            string dbResult = ExistDao.QueryXml(query);

            List<SearchResultWithXmlContext> results = new List<SearchResultWithXmlContext>();

            XmlDocument dbXmlResult = new XmlDocument();
            dbXmlResult.LoadXml(dbResult);

            XmlNamespaceManager nManager = new XmlNamespaceManager(dbXmlResult.NameTable);
            foreach (var allNamespace in Descriptor.GetAllNamespaces())
            {
                nManager.AddNamespace(allNamespace.Key, allNamespace.Value);
            }

            return ConstructResultsWithXmlContext(dbXmlResult, nManager);
        }

        private List<SearchResultWithXmlContext> ConstructResultsWithXmlContext(XmlDocument dbXmlResult, XmlNamespaceManager nManager)
        {
            var results = new List<SearchResultWithXmlContext>();
            XmlNodeList hits = dbXmlResult.SelectNodes("//hit");
            if (hits != null)
                foreach (XmlNode hit in hits)
                {
                    SearchResultWithXmlContext result = new SearchResultWithXmlContext();
                    result.Author = XmlTool.ParseTeiAuthor(hit.SelectSingleNode("author"), TeiP5Descriptor.AuthorNodeName, nManager);
                    result.Title = XmlTool.ParseTeiTitle(hit.SelectSingleNode("title"), TeiP5Descriptor.TitleNodeName, nManager);
                    result.Id = XmlTool.ParseId(hit.SelectSingleNode("id"), TeiP5Descriptor.IdAttributeName);
                    result.Categories = XmlTool.ParseTeiCategoriesIds(hit.SelectSingleNode("categories"), TeiP5Descriptor.CategoriesNodeName, TeiP5Descriptor.CategoriesTargetAttributName, nManager);
                    result.XmlContext = XmlTool.ParseXmlContext(hit.SelectSingleNode("context"));
                    results.Add(result);
                }

            return results;
        }

        private List<SearchResultWithHtmlContext> ConstructResultsWithHtmlContext(XmlDocument dbXmlResult, XmlNamespaceManager nManager, string searchedTerm)
        {
            List<SearchResultWithHtmlContext> results = new List<SearchResultWithHtmlContext>();
            XmlNodeList hits = dbXmlResult.SelectNodes("//hit");
            if (hits != null)
                foreach (XmlNode hit in hits)
                {
                    SearchResultWithHtmlContext result = new SearchResultWithHtmlContext();
                    result.Author = XmlTool.ParseTeiAuthor(hit.SelectSingleNode("authors"), TeiP5Descriptor.AuthorNodeName, nManager);
                    result.Title = XmlTool.ParseTeiTitle(hit.SelectSingleNode("title"), TeiP5Descriptor.TitleNodeName, nManager);
                    result.Id = XmlTool.ParseId(hit.SelectSingleNode("id"), TeiP5Descriptor.IdAttributeName);
                    result.Categories = XmlTool.ParseTeiCategoriesIds(hit.SelectSingleNode("categories"), TeiP5Descriptor.CategoriesNodeName, TeiP5Descriptor.CategoriesTargetAttributName, nManager);

                    //string xmlContext = XmlTool.ParseXmlContext(hit.SelectSingleNode("context"));
                    // result.HtmlContext = m_xsltTransformer.TransformResult(xmlContext, searchedTerm);//todo parse only hit.SelectSingleNode
                    result.HtmlContext = m_xsltTransformer.TransformResult(hit.OuterXml, searchedTerm); //todo parse only hit.SelectSingleNode

                    results.Add(result);
                }

            return results;
        }

        private string GetAllPossibleKeywordsQuery(string input, string restrictions)
        {
            StringBuilder builder = new StringBuilder();

            AddNamespacesAndCollation(builder);
            builder.AppendLine("<words> {");

            builder.AppendLine("for $distinctVal in");
            builder.AppendLine("distinct-values(<undistinctedValues> {");

            builder.AppendLine("let $query :=");
            builder.AppendLine("<query>");
            builder.AppendLine("<bool>");
            builder.AppendLine(string.Format("<wildcard occur='must'>{0}</wildcard>", input));
            builder.AppendLine("</bool>");
            builder.AppendLine("</query>");

            builder.AppendLine(string.Format("let $collections:= string(\"{0}\")", Descriptor.GetDataLocation));
            builder.AppendLine("let $words := collection($collections)//tei:w[ft:query(., $query)]");
            builder.AppendLine("for $word in $words");

            builder.AppendLine("let $id := $word/ancestor-or-self::tei:TEI/@n");
            builder.AppendLine(restrictions);

            builder.AppendLine("order by $word");
            builder.AppendLine("return <word>{$word}</word>");
            builder.AppendLine("} </undistinctedValues>//tei:w)");

            builder.AppendLine("return <word>{$distinctVal}</word>");
            builder.AppendLine("} </words>");

            return builder.ToString();
        }

        private string GetAllPossibleKeyWordsByLemmaAttributeQuery(string input, string restrictions)
        {
            StringBuilder builder = new StringBuilder();

            AddNamespacesAndCollation(builder);
            builder.AppendLine("<words> {");

            builder.AppendLine("for $distinctVal in");
            builder.AppendLine("distinct-values(<undistinctedValues> {");

            builder.AppendLine("let $query :=");
            builder.AppendLine("<query>");
            builder.AppendLine("<bool>");
            builder.AppendLine(string.Format("<wildcard occur='must'>{0}</wildcard>", input));
            builder.AppendLine("</bool>");
            builder.AppendLine("</query>");

            builder.AppendLine(string.Format("let $collections:= string(\"{0}\")", Descriptor.GetDataLocation));
            builder.AppendLine("let $words := collection($collections)/tei:TEI/tei:text/tei:body//tei:w[ft:query(@nlp:lemma, $query)]");
            builder.AppendLine("for $word in $words");

            builder.AppendLine("let $id := $word/ancestor-or-self::tei:TEI/@n");
            builder.AppendLine(restrictions);

            builder.AppendLine("order by $word");
            builder.AppendLine("return <word>{$word}</word>");
            builder.AppendLine("} </undistinctedValues>//tei:w/@nlp:lemma)");

            builder.AppendLine("return <word>{$distinctVal}</word>");
            builder.AppendLine("} </words>");

            return builder.ToString();
        }


        public string GetAllPossibleIdsByLemmaAttributeQuery(string input, string restrictions)
        {
            StringBuilder builder = new StringBuilder();
            AddNamespacesAndCollation(builder);
            builder.AppendLine("<ids> {");
            builder.AppendLine("for $distinctVal in distinct-values(<undistinctedValues> {");
            builder.AppendLine("let $query :=");
            builder.AppendLine("<query>");
            builder.AppendLine("<bool>");
            builder.AppendLine(string.Format("<wildcard occur='must'>{0}</wildcard>", input));
            builder.AppendLine("</bool>");
            builder.AppendLine("</query>");

            builder.AppendLine(string.Format("let $collections:= string(\"{0}\")", Descriptor.GetDataLocation));
            builder.AppendLine("let $words := collection($collections)/tei:TEI/tei:text/tei:body//tei:w[ft:query(@nlp:lemma, $query)]");
            builder.AppendLine("for $word in $words");
            builder.AppendLine("let $id := $word/ancestor-or-self::tei:TEI/@n");
            builder.AppendLine(restrictions);
            builder.AppendLine("return <id>{$id}</id>");
            builder.AppendLine("} ");
            builder.AppendLine("</undistinctedValues>//@n)");
            builder.AppendLine("return <id>{$distinctVal}</id>");
            builder.AppendLine("} </ids>");


            return builder.ToString();
        }


        private string GetKeyWordInContextQuery(string word, string restrictions)
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
            builder.AppendLine("let $words := collection($collections)//tei:w[ft:query(., $query)]");
            builder.AppendLine("for $hit in $words");
            builder.AppendLine("let $expanded := kwic:expand($hit/..)");
            builder.AppendLine("let $title := $hit/ancestor-or-self::tei:TEI/tei:teiHeader/tei:fileDesc/tei:titleStmt/tei:title");
            builder.AppendLine("let $id := $hit/ancestor-or-self::tei:TEI/@n");
            builder.AppendLine("let $author := $hit/ancestor-or-self::tei:TEI//tei:author");
            builder.AppendLine("let $categories := $hit/ancestor-or-self::tei:TEI/tei:teiHeader/tei:profileDesc/tei:textClass/tei:catRef");
            builder.AppendLine(string.Format("let $kwic:= kwic:get-summary($expanded, ($expanded//exist:match), <config width=\"{0}\"/>)", KeyWordValue));

            builder.AppendLine(restrictions);

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

        private string GetXmlContextForWordQuery(string word, string restrictions)
        {
            StringBuilder builder = new StringBuilder();
            AddNamespacesAndCollation(builder);

            builder.AppendLine("import module namespace kwic=\"http://exist-db.org/xquery/kwic\";");

            builder.AppendLine("declare function local:getAdequateContext($hit as node()) {");
            builder.AppendLine("let $context := ");
            builder.AppendLine("if($hit instance of element(tei:body)) then $hit");
            builder.AppendLine("else if ($hit/.. instance of element(tei:div)) then $hit/..");
            builder.AppendLine("else if ($hit/.. instance of element(tei:p)) then $hit/..");
            builder.AppendLine("else if($hit/.. instance of element(tei:entryFree)) then $hit/..");
            builder.AppendLine("else if ($hit/.. instance of element(tei:l)) then $hit/..     ");
            builder.AppendLine("else local:getAdequateContext($hit/..)");
            builder.AppendLine("return $context");

            builder.AppendLine("};");

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

            builder.AppendLine("let $context := local:getAdequateContext($hit)");

            builder.AppendLine(restrictions);

            builder.AppendLine("order by $hit");
            builder.AppendLine("return <hit>");
            builder.AppendLine("<id>{$id}</id>");
            builder.AppendLine("<title>{$title}</title>");
            builder.AppendLine("<authors>{$author}</authors>");
            builder.AppendLine("<categories>{$categories}</categories>");
            builder.AppendLine("<context>{$context}</context>");
            builder.AppendLine("</hit>");
            builder.AppendLine();
            builder.AppendLine("}");
            builder.AppendLine("</words>");

            return builder.ToString();
        }

        private string GetXmlContextForWordQueryV2(string word, string restrictions)
        {
            StringBuilder builder = new StringBuilder();
            AddNamespacesAndCollation(builder);
            builder.AppendLine("declare copy-namespaces no-preserve, inherit;");


            builder.AppendLine("<words xmlns:tei=\"http://www.tei-c.org/ns/1.0\" xmlns:nlp=\"http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0\">");

            builder.AppendLine("{");
            builder.AppendLine("let $query :=");
            builder.AppendLine("<query>");
            builder.AppendLine("<bool>");
            builder.AppendLine(string.Format("<wildcard occur='must'>{0}</wildcard>", word));
            builder.AppendLine("</bool>");
            builder.AppendLine("</query>");

            builder.AppendLine(string.Format("let $collections:= string(\"{0}\")", Descriptor.GetDataLocation));

            builder.AppendLine("let $segDictionaries := collection($collections)//tei:TEI//tei:entryFree[*//tei:w[ft:query(@nlp:lemma, $query)]]");
            builder.AppendLine("let $segTexts := collection($collections)//tei:TEI//(tei:p | tei:l | tei:title | tei:item | tei:head)[tei:w[ft:query(@nlp:lemma, $query)]]");
            builder.AppendLine("let $segments := ($segDictionaries, $segTexts)");

            builder.AppendLine("for $segment in $segments  ");
            builder.AppendLine("let $id := $segment/ancestor::tei:TEI/@n");
            builder.AppendLine("let $title := $segment/ancestor::tei:TEI/tei:teiHeader/tei:fileDesc/tei:titleStmt/tei:title");
            builder.AppendLine("let $author := $segment/ancestor::tei:TEI/tei:teiHeader/tei:fileDesc/tei:titleStmt/tei:author");
            builder.AppendLine("let $categories := $segment/ancestor::tei:TEI/tei:teiHeader/tei:profileDesc/tei:textClass/tei:catRef");
            builder.AppendLine(restrictions);

            builder.AppendLine("    return <hit>");
            builder.AppendLine("<id>{$id}</id>");
            builder.AppendLine("<title>{$title}</title>");
            builder.AppendLine("<authors>{$author}</authors>");
            builder.AppendLine("<categories>{$categories}</categories>");
            builder.AppendLine("<context>{$segment}</context>");
            builder.AppendLine("</hit>");
            builder.AppendLine("}");
            builder.AppendLine("</words>");


            return builder.ToString();
        }
    }
}