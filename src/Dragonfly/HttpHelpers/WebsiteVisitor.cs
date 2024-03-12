namespace Dragonfly.NetHelpers
{
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;

    public static class WebsiteVisitor
    {
        private const string ThisClassName = "Dragonfly.NetHelpers.WebsiteVisitor";

        public static string GetIPAddress()
        {
            string IPAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]; //From behind a proxy

            if (IPAddress == "" | IPAddress == null)
            {
                IPAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]; //normal
            }

            return IPAddress;
        }

        public static string AllServerVariables(bool AsHTML = true)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < HttpContext.Current.Request.ServerVariables.Count - 1; i++)
            {
                sb.AppendLine(HttpContext.Current.Request.ServerVariables[i].ToString());
                if (AsHTML) { sb.Append("<br/>"); }
            }

            return sb.ToString();
        }

        public static string GetUserAgent()
        {

            return HttpContext.Current.Request.UserAgent;
        }

        public static string GetReferingUrl()
        {
            var url = HttpContext.Current.Request.UrlReferrer;
            return url!= null ? url.AbsoluteUri : "";
        }

        public static bool VisitorIsMobile()
        {
            bool IsMobile = false;
            string UserAgent = "";
            UserAgent = HttpContext.Current.Request.UserAgent;
            //UserAgent ="Mozilla/5.0 (Linux; U; Android 4.0.3; ko-kr; LG-L160L Build/IML74K) AppleWebkit/534.30 (KHTML, like Gecko) Version/4.0 Mobile Safari/534.30";

            if (UserAgent != "")
            {
                Regex MobileRegEx =
                    new Regex("/Mobile|iP(hone|od|ad)|Android|BlackBerry|IEMobile|Kindle|NetFront|Silk-Accelerated|(hpw|web)OS|Fennec|Minimo|Opera M(obi|ini)|Blazer|Dolfin|Dolphin|Skyfire|Zune/");
                IsMobile = MobileRegEx.IsMatch(UserAgent);
            }

            return IsMobile;

        }
    }
}