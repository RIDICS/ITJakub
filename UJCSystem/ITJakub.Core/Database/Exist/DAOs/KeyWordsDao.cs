﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Castle.MicroKernel;
using ITJakub.Contracts.Searching;
using ITJakub.Xml.XMLOperations;

namespace ITJakub.Core.Database.Exist.DAOs
{
    public class ExistWordsDao : ExistDaoBase
    {
        private const int KeyWordValue = 45;
        private XslTransformDirector m_xsltTransformer;

        public ExistWordsDao(IKernel container) : base(container)
        {
            m_xsltTransformer = container.Resolve<XslTransformDirector>();
        }

        

        public List<string> GetAllPossibleKeyWords(string key, List<string> booksIds = null)
        {
            var restriction = GetRestrictionByBookIds(booksIds);

            //string query = GetAllPossibleKeywordsQuery(key, restriction);
            string query = GetAllPossibleKeyWordsByLemmaAttributeQuery(key, restriction);

            string dbResult = ExistDao.QueryXml(query);
            var result = XmlTool.CutElementsText(dbResult, "word");
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
                    result.OriginalXml = hit.InnerXml;

                    results.Add(result);
                }


            XmlDocument xmlResult = new XmlDocument();
            xmlResult.LoadXml(dbResult);

            return results;
        }
        public List<SearchResultWithHtmlContext> GetHtmlContextByWord(string word, List<string> bookIds = null)
        {
            var restriction = GetRestrictionByBookIds(bookIds);
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

            return ConstructResultsWithHtmlContext(dbXmlResult, nManager);
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
                    result.Author = XmlTool.ParseTeiAuthor(hit.SelectSingleNode("//author"), TeiP5Descriptor.AuthorNodeName, nManager);
                    result.Title = XmlTool.ParseTeiTitle(hit.SelectSingleNode("//title"), TeiP5Descriptor.TitleNodeName, nManager);
                    result.Id = XmlTool.ParseId(hit.SelectSingleNode("//id"), TeiP5Descriptor.IdAttributeName);
                    result.Categories = XmlTool.ParseTeiCategoriesIds(hit.SelectSingleNode("//categories"), TeiP5Descriptor.CategoriesNodeName, TeiP5Descriptor.CategoriesTargetAttributName, nManager);
                    result.XmlContext = XmlTool.ParseXmlContext(hit.SelectSingleNode("context"));
                    results.Add(result);
                }


    

            return results;
        }

        private List<SearchResultWithHtmlContext> ConstructResultsWithHtmlContext(XmlDocument dbXmlResult, XmlNamespaceManager nManager)
        {
            var results = new List<SearchResultWithHtmlContext>();
            XmlNodeList hits = dbXmlResult.SelectNodes("//hit");
            if (hits != null)
                foreach (XmlNode hit in hits)
                {
                    SearchResultWithHtmlContext result = new SearchResultWithHtmlContext();
                    result.Author = XmlTool.ParseTeiAuthor(hit.SelectSingleNode("//author"), TeiP5Descriptor.AuthorNodeName, nManager);
                    result.Title = XmlTool.ParseTeiTitle(hit.SelectSingleNode("//title"), TeiP5Descriptor.TitleNodeName, nManager);
                    result.Id = XmlTool.ParseId(hit.SelectSingleNode("//id"), TeiP5Descriptor.IdAttributeName);
                    result.Categories = XmlTool.ParseTeiCategoriesIds(hit.SelectSingleNode("//categories"), TeiP5Descriptor.CategoriesNodeName, TeiP5Descriptor.CategoriesTargetAttributName, nManager);

                    string xmlContext = XmlTool.ParseXmlContext(hit.SelectSingleNode("context"));
                    result.HtmlContext = m_xsltTransformer.TransformResult(xmlContext);

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
            builder.AppendLine("let $words := collection(\"\")//tei:w[ft:query(., $query)]");
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
            builder.AppendLine("let $words := collection(\"\")//tei:w[ft:query(@nlp:lemma, $query)]");
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
            builder.AppendLine("let $words := collection(\"\")//tei:w[ft:query(., $query)]");
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
            builder.AppendLine("<words>{");
            builder.AppendLine("let $query :=");
            builder.AppendLine("<query>");
            builder.AppendLine("<bool>");
            builder.AppendLine(string.Format("<wildcard occur='must'>{0}</wildcard>", word));
            builder.AppendLine("</bool>");
            builder.AppendLine("</query>");
            builder.AppendLine("let $words := collection(\"\")//tei:w[ft:query(@nlp:lemma, $query)]");
            builder.AppendLine("for $hit in $words");
            builder.AppendLine("let $title := $hit/ancestor-or-self::tei:TEI/tei:teiHeader/tei:fileDesc/tei:titleStmt/tei:title");
            builder.AppendLine("let $id := $hit/ancestor-or-self::tei:TEI/@n");
            builder.AppendLine("let $author := $hit/ancestor-or-self::tei:TEI//tei:author");
            builder.AppendLine("let $categories := $hit/ancestor-or-self::tei:TEI/tei:teiHeader/tei:profileDesc/tei:textClass/tei:catRef");

            builder.AppendLine("let $context := if ($hit/ancestor::tei:p instance of element(tei:p)) then");
            builder.AppendLine("$hit/ancestor::tei:p");
            builder.AppendLine("else if($hit/ancestor::tei:entryFree instance of element(tei:entryFree)) then");
            builder.AppendLine("$hit/ancestor::tei:entryFree");
            builder.AppendLine("else");
            builder.AppendLine("$hit/..");

            builder.AppendLine(restrictions);

            builder.AppendLine("order by $hit");
            builder.AppendLine("return <hit>");
            builder.AppendLine("<title>{$title}</title>");
            builder.AppendLine("<id>{$id}</id>");
            builder.AppendLine("<author>{$author}</author>");
            builder.AppendLine("<categories>{$categories}</categories>");
            builder.AppendLine("<context>{$context}</context>");
            builder.AppendLine("</hit>");
            builder.AppendLine();
            builder.AppendLine("}");
            builder.AppendLine("</words>");


            return builder.ToString();
        }

    }
}
