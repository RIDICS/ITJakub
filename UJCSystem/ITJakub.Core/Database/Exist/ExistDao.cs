using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using Jewelery;
using log4net;

namespace ITJakub.Core.Database.Exist
{
    public class ExistConnectionSettingsSkeleton
    {
        public string BaseUri { get; private set; }
        public string ViewsCollection { get; private set; }
        public string ResourcesCollection { get; private set; }

        public string DBUser { get; private set; }
        public string DBPassword { get; private set; }

        public ExistConnectionSettingsSkeleton(string baseUri, string viewsCollection, string resourcesCollection, string dbUser, string dbPassword)
        {
            BaseUri = baseUri;
            ViewsCollection = viewsCollection;
            ResourcesCollection = resourcesCollection;
            DBUser = dbUser;
            DBPassword = dbPassword;
        }
    }

    /// <summary>
    ///     Implementation of DAO for eXist DBMS.
    /// </summary>
    public class ExistDao
    {
        private readonly ExistConnectionSettingsSkeleton m_settings;
        private const int DBMaxResults = 99999;

        private const string Xsd1Name = "TEI_UJC_OVJ_Strict.xsd";
        private const string Xsd2Name = "xml.xsd";

        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ExistDao(ExistConnectionSettingsSkeleton connectionSettings)
        {
            m_settings = connectionSettings;
        }

        public string[] ReadAllNames()
        {
            const string wrapper = "my-result";
            string qry = "for $doc in collection(\"" + m_settings.ViewsCollection + "\") return <" + wrapper + ">{" +
                         "substring-after(document-uri($doc), \"" + m_settings.ViewsCollection + "/\")" +
                         "}</" + wrapper + "> ";

            byte[] reqBytes = ConstructExistQry(qry);
            byte[] respBytes = QueryDb("POST", "", reqBytes);
            string str = Encoding.UTF8.GetString(respBytes);
            return XmlTool.CutElementsText(str, wrapper);
        }

        public byte[] Read(string name)
        {
            return QueryDb("GET", m_settings.ViewsCollection + "/" + name, null);
        }

        public string QueryXml(string qry)
        {
            byte[] reqBytes = ConstructExistQry(qry);
            byte[] respBytes = QueryDb("POST", "", reqBytes);
            string str = Encoding.UTF8.GetString(respBytes);
            return XmlTool.RemoveRootElement(str);
        }

        public void Delete(string name)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> ReadXsds(DocumentKind docKind)
        {
            // TODO check document kind

            byte[] respBytes1 = QueryDb("GET", m_settings.ResourcesCollection + "/" + Xsd1Name, null);
            byte[] respBytes2 = QueryDb("GET", m_settings.ResourcesCollection + "/" + Xsd2Name, null);
            var outArr = new string[2];
            outArr[0] = Encoding.UTF8.GetString(respBytes1);
            outArr[1] = Encoding.UTF8.GetString(respBytes2);
            return outArr;
        }

        public void ReindexViewsCollection()
        {
            byte[] reqBytes = ConstructExistQry("xmldb:reindex(\"" + m_settings.ViewsCollection + "\")");
            QueryDb("POST", "", reqBytes);
        }

        private byte[] QueryDb(string method, string relativeUri, byte[] content)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            string qry = (content == null) ? "no-data" : Encoding.UTF8.GetString(content);
            Debug.WriteLine("Query: " + method + " " + m_settings.BaseUri + relativeUri + " { " + qry + " }");

            var req = (HttpWebRequest)WebRequest.Create(m_settings.BaseUri + relativeUri);

            // authentication header
            string auth = string.Format("{0}:{1}",m_settings.DBUser, m_settings.DBPassword);
            auth = Convert.ToBase64String(Encoding.UTF8.GetBytes(auth));
            req.Headers[HttpRequestHeader.Authorization] = "Basic " + auth;

            req.Method = method;
            req.ContentType = "application/xml";
            if (content != null && content.Length > 0)
            {
                using(Stream reqStream = req.GetRequestStream()){
                    reqStream.Write(content, 0, content.Length);
                    reqStream.Close();
                }
            }
            var resp = (HttpWebResponse) req.GetResponse();

            long ms = stopwatch.ElapsedMilliseconds;
            if (m_log.IsDebugEnabled)
                m_log.DebugFormat("HTTP request on DB's REST API took {0} ms", ms);
            stopwatch.Stop();

            Stream respStream = resp.GetResponseStream();
            byte[] respBytes = respStream.StreamToByteArray();
            if (respStream != null)
                respStream.Close();

            return respBytes;
        }

        private byte[] ConstructExistQry(string qry)
        {
            string qStr = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                          "<query xmlns=\"http://exist.sourceforge.net/NS/exist\" max=\"" + DBMaxResults + "\">" +
                          "<text><![CDATA[ " + qry + " ]]></text>" +
                          "<properties><property name=\"indent\" value=\"yes\"/></properties></query>";
            return Encoding.UTF8.GetBytes(qStr);
        }
    }
}