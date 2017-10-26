using System.IO;
using System.Management.Automation;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml;
using HtmlAgilityPack;

namespace Sorlov.PowerShell.Cmdlets
{
    [Cmdlet(VerbsData.Import,"HTML")]
    public class ImportHTML: PSCmdlet
    {
        private string url;
        private string requestBody;
        private string userAgent;

        [Parameter(Position = 0, Mandatory = true, ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty()]
        public string Path
        {
            get { return url; }
            set { url = value; }
        }

        [Parameter(Position = 1)]
        [ValidateNotNullOrEmpty()]
        public string RequestBody
        {
            get { return requestBody; }
            set { requestBody = value; }
        }

        [Parameter(Position = 2)]
        [ValidateNotNullOrEmpty()]
        public string UserAgent
        {
            get { return userAgent; }
            set { userAgent = value; }
        }

        

        protected override void BeginProcessing()
        {
            HtmlDocument doc = new HtmlDocument();
            doc.OptionOutputAsXml = true;
            doc.OptionFixNestedTags = true;
            HtmlNode.ElementsFlags.Remove("script");

            if (url.StartsWith("http", true, Thread.CurrentThread.CurrentUICulture))
            {

                if (string.IsNullOrWhiteSpace(userAgent)) userAgent = string.Format("Mozilla/5.0 (Windows NT; Windows NT 6.1; en-US) Sorlov.PowerShell/{0}", Assembly.GetExecutingAssembly().GetName().Version.ToString());
                HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);

                request.UserAgent = userAgent;
                if (!string.IsNullOrWhiteSpace(requestBody))
                {
                    request.ContentType = "application/x-www-form-urlencoded";
                    Stream dataStream = request.GetRequestStream();
                    byte[] data = System.Text.Encoding.UTF8.GetBytes(requestBody);
                    dataStream.Write(data, 0, data.Length);
                    dataStream.Close();
                    request.Method = "POST";
                }

                HttpWebResponse response = (HttpWebResponse) request.GetResponse();

                using (Stream responseStream = response.GetResponseStream())
                {
                    doc.Load(responseStream);
                }
            }
            else
            {
                ProviderInfo providerInfo;
                string resolvedPath = this.GetResolvedProviderPathFromPSPath(url, out providerInfo)[0];
                doc.Load(resolvedPath);
            }

            using (MemoryStream resultStream = new MemoryStream())
            {
                doc.Save(resultStream, Encoding.UTF8);
                resultStream.Seek(0, SeekOrigin.Begin);

                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(resultStream);
                WriteObject(xmlDocument);
            }

        }
    }
}
