using System.Collections.Generic;
using System.Xml;

namespace ITJakub.Core.Database.Exist
{

    /// <summary>
    /// XML format that describes XPaths of TEI P5 standard.
    /// </summary>
    public class TeiP5Descriptor
    {

        public string GetTeiNameSpace()
        {
            return "http://www.tei-c.org/ns/1.0";
        }

        public string GetCollation()
        {
            return "http://exist-db.org/collation?lang=CS-cz";
        }

        public string GetCustomNamespace()
        {
            return "http://vokabular.ujc.cas.cz/schema/tei-nlp";
        }

        //public string GetTitleXPath()
        //{
        //    return "/TEI/teiHeader/fileDesc/titleStmt/title";
        //}

        //public string GetAuthorXPath()
        //{
        //    return "/TEI/teiHeader/fileDesc/titleStmt/author";
        //}

        //public string GetDatationFromXPath()
        //{
        //    return "/TEI/teiHeader/fileDesc/sourceDesc/msDesc/history/origin/origDate/@notBefore";
        //}

        //public string GetDatationToXPath()
        //{
        //    return "/TEI/teiHeader/fileDesc/sourceDesc/msDesc/history/origin/origDate/@notAfter"; ;
        //}

        public static string AuthorNodeName
        {
            get { return "author"; }
        }
        public static string TitleNodeName
        {
            get { return "title"; }
        }

        public static string CategoriesNodeName
        {
            get { return "catRef"; }
        }

        public static string ParagraphNodeName
        {
            get { return "p"; }
        }

        public static string WordTagName
        {
            get { return "w"; }
        }

        public static string SpaceTagName
        {
            get { return "c"; }
        }

        public static string IdAttributeName { get { return "n"; } }

        public static string CategoriesTargetAttributName
        {
            get { return "target"; }
        }

        public IEnumerable<KeyValuePair<string, string>> GetAllNamespaces()
        {
            var result = new List<KeyValuePair<string, string>>();
            result.Add(new KeyValuePair<string, string>("tei", GetTeiNameSpace()));
            result.Add(new KeyValuePair<string, string>("nlp", GetCustomNamespace()));
            return result;
        }
    }
}
