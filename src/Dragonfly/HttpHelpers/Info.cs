namespace Dragonfly.NetHelpers
{
    using System;
    using System.Configuration;
    using System.Net;
    using System.Web;

    public enum ServerEnvironment
    {
        Dev,
        Staging,
        Live,
        Unknown
    }

    public class Info
    {
        private const string ThisClassName = "Dragonfly.NetHelpers.Info";
        
        public static string SiteDomainName = HttpContext.Current.Request.ServerVariables["HTTP_HOST"].ToString();

        /// <summary>
        /// Retrieves an app key from the web.config
        /// </summary>
        /// <param name="AppKeyName"></param>
        /// <returns></returns>
        public static string WebConfigValue(string AppKeyName, bool FailSilently = true)
        {
            string KeyValue = "";

            if (AppKeyName != null)
            {
                try
                {
                    KeyValue = ConfigurationManager.AppSettings[AppKeyName].ToString();
                }
                catch (Exception Ex)
                {
                    if (!FailSilently)
                    {
                        var functionName = string.Format("{0}.WebConfigValue", ThisClassName);
                        var msg = string.Format("{0} [AppKeyName='{1}']", functionName, AppKeyName);
                        Ex.Data.Add("DragonflyException",msg);
                        throw Ex;
                    }
                }
            }
            return KeyValue;
        }

        /// <summary>
        /// Checks a web.config value for common boolean strings and returns true or false
        /// </summary>
        /// <param name="AppKeyName">AppKey from Web.config to lookup</param>
        /// <param name="FailSilently">If the Key is not found and FailSilently is true, FALSE will be returned, otherwise an exception will be raised. </param>
        /// <returns></returns>
        public static bool WebConfigValueAsBool(string AppKeyName, bool FailSilently = true)
        {
            var stringKey = WebConfigValue(AppKeyName, FailSilently);

            if (stringKey == "")
            {
                return false;
            }
            else
            {
                switch (stringKey.ToLower())
                {
                    case "true":
                        return true;
                    case "false":
                        return false;
                    case "1":
                        return true;
                    case "0":
                        return false;
                    default:
                        return false;
                }
            }

        }

        public static string GetConnectionStringFromConfig(string ConnectionStringName)
        {
            string RetrievedConnString = "";

            System.Configuration.Configuration rootWebConfig =
                System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(null);

            System.Configuration.ConnectionStringSettings connString;

            if (rootWebConfig.ConnectionStrings.ConnectionStrings.Count > 0)
            {
                connString =
                    rootWebConfig.ConnectionStrings.ConnectionStrings[ConnectionStringName];

                if (connString != null)
                    RetrievedConnString = connString.ConnectionString;
                else
                    RetrievedConnString = "No connection string available for " + ConnectionStringName;
            }

            return RetrievedConnString;

        }

        /// <summary>
        /// Gets the domain name for the current page
        /// </summary>
        /// <param name="IncludeProtocol">Include "http://" at the beginning?</param>
        /// <returns></returns>
        public static string ThisSiteDomain(bool IncludeProtocol = true)
        {
            string DomainText = HttpContext.Current.Request.ServerVariables["HTTP_HOST"].ToString();

            if (IncludeProtocol)
            {
                return "http://" + DomainText;
            }
            else
            {
                return DomainText;
            }
        }

        public static string GetServerIpAddress()
        {
            string strHostName = Dns.GetHostName();
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];

            return ipAddress.ToString();
        }

        public static ServerEnvironment GetCurrentEnvironment(bool ExplicitOnly = false)
        {
            ServerEnvironment ThisEnvironment = ServerEnvironment.Unknown;

            //TODO: Refactor/cleanup
            string DevKeywords = ",dev,development,local";
            string StageKeywords = ",stag,staged,staging,";
            string LiveKeywords = ",live,prod,production,";

            //#1 : Check web.config
            string WebConfigEnv = Info.WebConfigValue("CurrentEnvironment").ToLower();

            if (DevKeywords.Contains(String.Concat(",", WebConfigEnv, ",")))
            {
                ThisEnvironment = ServerEnvironment.Dev;
            }
            else if (StageKeywords.Contains(String.Concat(",", WebConfigEnv, ",")))
            {
                ThisEnvironment = ServerEnvironment.Staging;
            }
            else if (LiveKeywords.Contains(String.Concat(",", WebConfigEnv, ",")))
            {
                ThisEnvironment = ServerEnvironment.Live;
            }
            else
            {
                //var functionName = string.Format("{0}.GetCurrentEnvironment", ThisClassName);
                //var msg = string.Format("{0} There is no explicit Web.Config value for 'CurrentEnvironment'", functionName);
                //Ex.Data.Add("DragonflyException", msg);
                //throw Ex;
            }


            if (ThisEnvironment == ServerEnvironment.Unknown)
            {
                //#2 : TODO: check hostname
            }

            if (ThisEnvironment == ServerEnvironment.Unknown)
            {
                //#4 : Assume live (unless "ExplicitOnly = true)
                if (!ExplicitOnly)
                {
                    ThisEnvironment = ServerEnvironment.Live;
                }
            }

            return ThisEnvironment;
        }

        public static bool IsLiveEnvironment(bool ExplicitOnly = false)
        {
            ServerEnvironment ThisEnvironment = GetCurrentEnvironment(ExplicitOnly);

            if (ThisEnvironment == ServerEnvironment.Live)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}