using System.IO;
using System.Net;
using System.Text;

namespace Ujc.Naki.AdvUploader
{
    internal class Program
    {
        public string UploadFile(string URL, byte[] fileData)
        {
            string response;
            HttpWebRequest webReq = null;
            HttpWebResponse webRes = null;
            StreamReader streamResponseReader = null;
            Stream requestStream = null;
            try
            {
                webReq = (HttpWebRequest) WebRequest.Create(URL);
                webReq.Method = "POST";
                webReq.Accept = "*/*";
                webReq.Timeout = 50000;
                webReq.KeepAlive = false;
                webReq.AllowAutoRedirect = false;
                webReq.AllowWriteStreamBuffering = true;
                webReq.ContentType = "binary/octet-stream";
                webReq.ContentLength = fileData.Length;


                requestStream = webReq.GetRequestStream();
                requestStream.Write(fileData, 0, fileData.Length);

                requestStream.Close();

                webRes = (HttpWebResponse) webReq.GetResponse();
                streamResponseReader = new StreamReader(webRes.GetResponseStream(), Encoding.UTF8);
                response = streamResponseReader.ReadToEnd();
            }
            catch
            {
                throw;
            }
            finally
            {
                if (webReq != null)
                {
                    webReq.Abort();
                    webReq = null;
                }
                if (webRes != null)
                {
                    webRes.Close();
                    webRes = null;
                }
                if (streamResponseReader != null)
                {
                    streamResponseReader.Close();
                    streamResponseReader = null;
                }
                if (requestStream != null)
                {
                    requestStream = null;
                }
            }


            return response;
        }

        private static void Main(string[] args)
        {
        }
    }
}