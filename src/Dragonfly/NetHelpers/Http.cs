namespace Dragonfly.NetHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;

    public static class Http
    {
        /// <summary>
        /// Method to simplify calling an external url.  Any results from the url will be saved as a string that can
        /// either be xml, html, json, etc etc.  It won't handle calling files directly, I think, not tried it,
        /// not had a need.  It works great with string based result though.
        /// </summary>
        /// <param name="url">The url you want to call</param>
        /// <param name="method">How you want to call it: "GET" or "POST"</param>
        /// <returns></returns>
        public static string CallUrl(string Url, string Method, string UserAgentString = "C# Application (compatible; MSIE 6.0; Windows NT 5.1)")
        {
            //From http://you.arenot.me/2010/09/28/facebooks-graph-api-and-asp-net/
            string UserAgent = UserAgentString;
            int _timeout = 300000;

            HttpWebRequest req = null;
            HttpWebResponse res = null;

            // Initialise the web request
            req = (HttpWebRequest)WebRequest.Create(Url);
            req.Method = Method.Length > 0 ? Method : "POST";

            req.UserAgent = UserAgent;

            // if (Proxy != null) req.Proxy = Proxy;
            req.Timeout = _timeout;
            req.KeepAlive = false;

            // This is needed in the Compact Framework
            // See for more details: http://msdn2.microsoft.com/en-us/library/1afx2b0f.aspx
            if (Method != "GET")
                req.GetRequestStream().Close();

            string responseString = String.Empty;

            try
            {
                // Get response from the internet
                res = (HttpWebResponse)req.GetResponse();
                using (StreamReader sr = new StreamReader(res.GetResponseStream()))
                {
                    responseString = sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                //Info.LogException("Functions.CallUrl", ex);
            }

            return responseString;
        }

        /// <summary>
        /// Just an overload of above.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string CallUrl(string Url)
        {
            return CallUrl(Url, "GET");
        }

        /// <summary>
        /// Takes a StringBuilder object and converts it to a file for instant download
        /// </summary>
        /// <param name="StringData">String Builder object to download</param>
        /// <param name="OutputFileName">Filename for downloaded file (default = "Export.csv")</param>
        /// <param name="MediaType">MIME Type for file (default = "text/csv")</param>
        /// <returns></returns>
        public static HttpResponseMessage StringBuilderToFile(StringBuilder StringData, string OutputFileName = "Export.csv", string MediaType = "text/csv")
        {
            //TODO: Need to figure out why » is returning as Â (likely an issue with unicode in general...?)

            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(StringData.ToString());
            writer.Flush();
            stream.Position = 0;

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue(MediaType);
            result.Content.Headers.ContentDisposition =
                new ContentDispositionHeaderValue("attachment") { FileName = OutputFileName };
            return result;
        }
    }
}
