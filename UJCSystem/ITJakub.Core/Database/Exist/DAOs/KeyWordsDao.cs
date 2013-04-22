﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ITJakub.Contracts.Searching;

namespace ITJakub.Core.Database.Exist.DAOs
{
    public class KeyWordsDao
    {
        private readonly ExistDao m_existDao;
        private readonly TeiP5Descriptor m_descriptor;

        private const int KeyWordValue = 45;

        public KeyWordsDao(ExistDao existDao, TeiP5Descriptor descriptor)
        {
            m_existDao = existDao;
            m_descriptor = descriptor;
        }

        public List<string> GetAllPossibleKeyWords(string input)
        {
            string query = GetAllPossibleKeywordsQuery(input);

            string dbResult = m_existDao.QueryXml(query);
            var result = XmlTool.CutElementsText(dbResult, "word");

            return result.ToList();
        }

        public KwicResult[] GetKeyWordInContextByWord(string word)
        {
            string query = GetKeyWordInContextQuery(word);
            string dbResult = m_existDao.QueryXml(query);

            List<KwicResult> results = new List<KwicResult>();

            XmlDocument dbXmlResult = new XmlDocument();
            dbXmlResult.LoadXml(dbResult);

            XmlNamespaceManager nManager = new XmlNamespaceManager(dbXmlResult.NameTable);
            foreach (var allNamespace in m_descriptor.GetAllNamespaces())
            {
                nManager.AddNamespace(allNamespace.Key, allNamespace.Value);
            }

            XmlNodeList hits =  dbXmlResult.SelectNodes("//hit");
            if (hits != null)
                foreach (XmlNode hit in hits)
                {
                    KwicResult result = new KwicResult();
                    result.Author = XmlTool.ParseTeiAuthor(hit.SelectSingleNode("//author"), TeiP5Descriptor.AuthorNodeName, nManager);
                    result.Title = XmlTool.ParseTeiTitle(hit.SelectSingleNode("//title"), TeiP5Descriptor.TitleNodeName, nManager);
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
            builder.AppendLine("let $author := $hit/ancestor::tei:TEI//tei:author");
            builder.AppendLine(string.Format("let $kwic:= kwic:get-summary($expanded, ($expanded//exist:match), <config width=\"{0}\"/>)", KeyWordValue));
            builder.AppendLine("order by $hit");
            builder.AppendLine("return <hit>");
            builder.AppendLine("<title>{$title}</title>");
            builder.AppendLine("<author>{$author}</author>");
            builder.AppendLine("<kwic>{$kwic}</kwic>");
            builder.AppendLine("</hit>");
            builder.AppendLine();
            builder.AppendLine("}");
            builder.AppendLine("</words>");
            

            return builder.ToString();
        }

        private void AddNamespacesAndCollation(StringBuilder builder)
        {
            builder.AppendLine(string.Format("declare default collation \"{0}\";", m_descriptor.GetCollation()));
            foreach (KeyValuePair<string, string> allNamespace in m_descriptor.GetAllNamespaces())
            {
                builder.AppendLine(string.Format("declare namespace {0} = \"{1}\";", allNamespace.Key, allNamespace.Value));
            }
        }

        
    }
}
