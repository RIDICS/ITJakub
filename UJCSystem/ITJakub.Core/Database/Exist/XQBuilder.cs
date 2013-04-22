//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace ITJakub.Core.Database.Exist
//{
//    /// <summary>
//    ///     XQuery builder which is used for constructing XQuery queries to XML database.
//    /// </summary>
//    public class XQBuilder
//    {
//        private const string DocVariable = "$doc";

//        // documents search
//        private const string RootDocumentsElement = "documents";
//        private const string DocumentElement = "document";

//        // metadata
//        private const string RootMetadataElement = "metadata";
//        private const string MetadataTitleElement = "title";
//        private const string MetadataAuthorElement = "author";
//        private const string MetadataTypeElement = "type";
//        private const string MetadataViewsElement = "views";
//        private const string MetadataViewElement = "view";
//        private const string MetadataPagesElement = "pages";
//        private const string MetadataPageElement = "page";
//        private const string MetadataPageNumberElement = "number";
//        private const string MetadataPageImageElement = "image";
//        private const string MetadataWordElement = "w";

//        private readonly IFilenamesResolver m_filenames;
//        private readonly TeiP5Descriptor m_formatDesc;
//        private readonly List<string> m_whereClauses = new List<string>();
//        private string m_collectionName;
//        private int m_id = -1; // not set
//        private bool m_isMetadataRequest;

//        public XQBuilder(TeiP5Descriptor xpath, IFilenamesResolver filenames)
//        {
//            m_formatDesc = xpath;
//            m_filenames = filenames;
//        }

//        /// <summary>
//        ///     Adds where clause to the builder.
//        /// </summary>
//        /// <param name="where">where clause to be added</param>
//        private void AddWhere(string where)
//        {
//            if (where != null && !string.Empty.Equals(where))
//            {
//                m_whereClauses.Add(where);
//            }
//        }

//        /// <summary>
//        ///     Builds final XQuery query.
//        /// </summary>
//        /// <returns>built XQuery</returns>
//        public string Build()
//        {
//            // metadata
//            if (m_isMetadataRequest)
//            {
//                if (m_id == -1)
//                {
//                    throw new Exception("If requesting metadata, ID must be set!");
//                }
//                return BuildMetadataRetrieval();
//            }

//                // /documents
//            return BuildDocumentsSearch();
//        }

//        private string BuildDocumentsSearch()
//        {
//            var sb = new StringBuilder();
//            sb.Append("declare default element namespace \"" + m_formatDesc.GetTeiNameSpace() + "\";");

//            AppendTag(sb, RootDocumentsElement, false);
//            sb.Append(" { "); // BRACE1
//            sb.Append("for $wrapper in distinct-values(");

//            sb.Append("for " + DocVariable + " in collection(\"" + m_collectionName + "\") ");
//            AppendWhereClauses(sb);
//            sb.Append("return ");
//            sb.Append("substring-before(substring-after(document-uri(")
//              .Append(DocVariable)
//              .Append("), \"")
//              .Append(m_collectionName)
//              .Append("/\"), \"")
//              .Append(m_filenames.GetSeparator())
//              .Append("\")");

//            sb.Append(") return "); // distinct-values
//            sb.Append("<").Append(DocumentElement).Append(" id=\"{$wrapper}\" />");

//            sb.Append(" } "); // BRACE1
//            AppendTag(sb, RootDocumentsElement, true);

//            return sb.ToString();
//        }

//        private string BuildMetadataRetrieval()
//        {
//            var sb = new StringBuilder();
//            sb.Append("declare default element namespace \"" + m_formatDesc.GetTeiNameSpace() + "\";");

//            string name = m_filenames.ConstructXmlName(m_id, DocumentView.Typo); // there must be always TYPO version
//            sb.Append("for " + DocVariable + " in doc(\"" + m_collectionName + "/" + name + "\") ");
//            sb.Append("return ");
//            AppendTag(sb, RootMetadataElement, false);

//            // title
//            AppendTag(sb, MetadataTitleElement, false);
//            sb.Append(" { ").Append(DocVariable).Append(m_formatDesc.GetTitleXPath()).Append("/text() } ");
//            AppendTag(sb, MetadataTitleElement, true);

//            // author
//            AppendTag(sb, MetadataAuthorElement, false);
//            sb.Append(" { ").Append(DocVariable).Append(m_formatDesc.GetAuthorXPath()).Append("/text() } ");
//            AppendTag(sb, MetadataAuthorElement, true);

//            // type
//            AppendTag(sb, MetadataTypeElement, false);
//            sb.Append("TODO - is within TEI P5 document?");
//            AppendTag(sb, MetadataTypeElement, true);

//            // views
//            AppendTag(sb, MetadataViewsElement, false);
//            string[] views = m_filenames.GetViewsFromID(m_id);
//            foreach (string text in views)
//            {
//                AppendTag(sb, MetadataViewElement, false);
//                sb.Append(text.ToLower());
//                AppendTag(sb, MetadataViewElement, true);
//            }
//            AppendTag(sb, MetadataViewsElement, true);

//            // pages
//            AppendTag(sb, MetadataPagesElement, false);
//            sb.Append(" { for $page in ").Append(DocVariable).Append("//pb return ");
//            AppendTag(sb, MetadataPageElement, false);
//            AppendTag(sb, MetadataPageNumberElement, false);
//            sb.Append("{ fn:string($page/@n) }");
//            AppendTag(sb, MetadataPageNumberElement, true);
//            AppendTag(sb, MetadataPageImageElement, false);
//            sb.Append("{ util:binary-doc-available(fn:replace('");
//            sb.Append(m_collectionName).Append("/");
//            string replaceString = "PGPGPGPG";
//            sb.Append(m_filenames.ConstructImgPageNameTemplate(m_id, DocumentView.Imgpage, replaceString));
//            // 0 as page number for replace           
//            sb.Append("','").Append(replaceString).Append("',fn:string($page/@n)");
//            sb.Append(")) }");
//            AppendTag(sb, MetadataPageImageElement, true);
//            AppendTag(sb, MetadataPageElement, true);
//            sb.Append(" } ");
//            AppendTag(sb, MetadataPagesElement, true);


//            AppendTag(sb, RootMetadataElement, true);
//            return sb.ToString();
//        }

//        private void AppendWhereClauses(StringBuilder sb)
//        {
//            for (int i = 0; i < m_whereClauses.Count; i++)
//            {
//                if (i == 0)
//                {
//                    sb.Append("where ");
//                }
//                else
//                {
//                    sb.Append("and ");
//                }
//                sb.Append(m_whereClauses[i]).Append(" ");
//            }
//        }

//        private void AppendTag(StringBuilder sb, string name, bool isClosing)
//        {
//            sb.Append("<");
//            if (isClosing)
//            {
//                sb.Append("/");
//            }
//            sb.Append(name).Append(">");
//        }

//        public void SetCollectionName(string collection)
//        {
//            m_collectionName = collection;
//        }

//        public void SetForMetadataRetrieval(int id)
//        {
//            m_isMetadataRequest = true;
//            m_id = id;
//        }

//        public void AddSearch(string searchText)
//        {
//            AddWhere(string.Format("{0} contains text \"{1}\"", DocVariable, searchText));
//        }

//        public void AddDatationFromWhere(int yearFrom)
//        {
//            AddWhere(string.Format("{0}{1} >= {2}", DocVariable, m_formatDesc.GetDatationFromXPath(), yearFrom));
//        }

//        public void AddDatationToWhere(int yearTo)
//        {
//            AddWhere(string.Format("{0}{1} <= {2}", DocVariable, m_formatDesc.GetDatationToXPath(), yearTo));
//        }

//        public void AddTitleWhere(string title)
//        {
//            AddWhere(string.Format("matches({0}{1},\"{2}\")", DocVariable, m_formatDesc.GetTitleXPath(), title));
//        }

//        public void AddAuthorWhere(string author)
//        {
//            AddWhere(string.Format("matches({0}{1},\"{2}\")", DocVariable, m_formatDesc.GetAuthorXPath(), author));
//        }

//        public void AddKindWhere(DocumentKind kind)
//        {
//            throw new NotImplementedException();
//        }

//        public void AddGenreWhere(DocumentGenre genre)
//        {
//            throw new NotImplementedException();
//        }

//        public void AddOriginalWhere(DocumentOriginal original)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}