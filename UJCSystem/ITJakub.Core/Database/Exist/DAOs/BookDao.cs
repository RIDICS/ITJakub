using System.Text;
using System.Xml;
using Castle.MicroKernel;

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
            builder.AppendLine(string.Format("let $words := collection(\"\")//tei:TEI[@n='{0}']", id));
            builder.AppendLine("for $hit in $words");
            builder.AppendLine("let $title := $hit/ancestor-or-self::tei:TEI/tei:teiHeader/tei:fileDesc/tei:titleStmt/tei:title");
            builder.AppendLine("return <title>{$title}</title>");

            return builder.ToString();
        }
    }
}