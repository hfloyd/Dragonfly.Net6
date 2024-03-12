namespace Dragonfly.NetHelpers
{
    using System;
    using System.IO;
    using System.Net;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Web;

    /// <summary>
    /// Helpers related to Cookies
    /// </summary>
    public class Cookies
    {
        private const string ThisClassName = "Dragonfly.NetHelpers.Cookies";

        /// <summary>
        /// Attempts to set a cookie, then checks to see if the cookie is present.
        /// </summary>
        /// <param name="AddResultToUrl">If TRUE, adds 'CookieCheck=true' to query string</param>
        /// <param name="SkipIfCookiePresent">If the test has already been run (the test cookie exists), don't redo the test.</param>
        /// <returns>Boolean</returns>
        //TODO: Fix & Re-enable
        //public static bool IsBrowserCookieDisabled(bool AddResultToUrl = false, bool SkipIfCookiePresent = true)
        //{
        //    var CurrRequest = HttpContext.Current.Request;
        //    var CurrResponse = HttpContext.Current.Response;

        //    bool CookiesDisabled = true;
        //    string currentUrl = CurrRequest.RawUrl;
        //    bool DoCookieTest = true;


        //    //Check for previously set cookie
        //    if (CurrRequest.Cookies["SupportCookies"] != null)
        //    {
        //        DoCookieTest = false;
        //        CookiesDisabled = false;

        //        //TODO: Update using new code pattern:
        //        //var functionName = string.Format("{0}.GetMySQLDataSet", ThisClassName);
        //        //var msg = string.Format("");
        //        //Info.LogInfo("Cookies: IsBrowserCookieDisabled: [SupportCookies]" + CurrRequest.Cookies["SupportCookies"].Value.ToString());
        //    }
        //    else //Previous cookie not found
        //    {
        //        if (CurrRequest.QueryString["CookieCheck"] == "true") //Test already run, must have failed
        //        {
        //            CookiesDisabled = true;
        //            DoCookieTest = false;
        //            //TODO: Update using new code pattern:
        //            //var functionName = string.Format("{0}.GetMySQLDataSet", ThisClassName);
        //            //var msg = string.Format("");
        //            //Info.LogInfo("Cookies: IsBrowserCookieDisabled: [CookieCheck] IS TRUE");
        //        }
        //        else //Test not run yet
        //        {
        //            DoCookieTest = true;
        //            //TODO: Update using new code pattern:
        //            //var functionName = string.Format("{0}.GetMySQLDataSet", ThisClassName);
        //            //var msg = string.Format("");
        //            //Info.LogInfo("Cookies: IsBrowserCookieDisabled: [SupportCookies] AND [CookieCheck] ARE NULL");
        //        }
        //    }

        //    if (DoCookieTest)
        //    {
        //        //TODO: Update using new code pattern:
        //        //var functionName = string.Format("{0}.GetMySQLDataSet", ThisClassName);
        //        //var msg = string.Format("");
        //        //Info.LogInfo("Cookies: IsBrowserCookieDisabled: START Cookie Test");
        //        try
        //        {
        //            HttpCookie c = new HttpCookie("SupportCookies", "true");
        //            CurrResponse.Cookies.Add(c);

        //            if (AddResultToUrl)
        //            {
        //            if (currentUrl.IndexOf("?") > 0)
        //                currentUrl = currentUrl + "&CookieCheck=true";
        //            else
        //                currentUrl = currentUrl + "?CookieCheck=true";
        //            CurrResponse.Redirect(currentUrl);
        //            }

        //            //Refresh page to test
        //            //
        //        }
        //        catch
        //        { } //something didn't work
        //        //TODO: Update using new code pattern:
        //        //var functionName = string.Format("{0}.GetMySQLDataSet", ThisClassName);
        //        //var msg = string.Format("");
        //        //Info.LogInfo("Cookies: IsBrowserCookieDisabled: END Cookie Test");
        //    }

        //    //TODO: Update using new code pattern:
        //    //var functionName = string.Format("{0}.GetMySQLDataSet", ThisClassName);
        //    //var msg = string.Format("");
        //    //Info.LogInfo("Cookies: IsBrowserCookieDisabled: [SupportCookies]" + CurrRequest.Cookies["SupportCookies"]);

        //    return CookiesDisabled;
        //}

        /// <summary>
        /// Gets the value of a subkey for a multi-key cookie
        /// </summary>
        /// <param name="CookieName">Name of parent cookie</param>
        /// <param name="SubKey">Name of desired subkey</param>
        /// <returns></returns>
        //TODO: Fix & Re-enable
        public static string GetCookieSubKey(string CookieName, string SubKey)
        {
            string CookieValue = string.Empty;

            if (HttpContext.Current.Request.Cookies[CookieName] != null)
            {
                //DEBUG INFO
                //TODO: Update using new code pattern:
                //var functionName = string.Format("{0}.GetMySQLDataSet", ThisClassName);
                //var msg = string.Format("");
                //Info.LogInfo("GetCookieSubKey=Main Cookie: " + HttpContext.Current.Request.Cookies[CookieName].Value);

                if (HttpContext.Current.Request.Cookies[CookieName].HasKeys == true)
                {
                    if (HttpContext.Current.Request.Cookies[CookieName][SubKey] != null)
                    { CookieValue = HttpContext.Current.Request.Cookies[CookieName][SubKey]; }
                }

                return CookieValue;
            }
            else
                return string.Empty;
        }

        /// <summary>
        /// Sets a cookie value
        /// </summary>
        /// <param name="CookieName">Name for Cookie</param>
        /// <param name="CookieValue">Value for Cookie</param>
        /// <param name="ExpirationDays">Days until expiration. 0 = "end of session"</param>
        /// <param name="OverwriteExisting">If a cookie with the same name already exists, should it be overwritten?</param>
        /// <param name="StripHTML">StripHTML</param>
        /// <param name="AlsoSaveToSession">Should the same data be saved to the Session state also (to support disabled cookies)?</param>
        /// <returns></returns>
        public static bool SetCookie(string CookieName, string CookieValue, int ExpirationDays = 0, bool OverwriteExisting = true, 
                                     bool StripHTML = true, bool AlsoSaveToSession = false)
        {
            var CurrRequest = HttpContext.Current.Request;
            var CurrResponse = HttpContext.Current.Response;
            bool boolSuccess = false;
            string sCleanedCookieVal = "";

            if (CookieValue != null & StripHTML == true)
            {
                sCleanedCookieVal = CookieValue; //TODO: Fix umbraco.library.StripHtml(CookieValue); 
            }
            else
            {
                sCleanedCookieVal = CookieValue;
            }

            if (CookieName != null & sCleanedCookieVal != null)
            {
                //Check for previously set cookie
                if (
                    (CurrRequest.Cookies[CookieName] != null & OverwriteExisting == true)
                    || (CurrRequest.Cookies[CookieName] == null)
                    )
                {
                    //Set cookie with options specified
                    try
                    {
                        HttpCookie c = new HttpCookie(CookieName, sCleanedCookieVal);
                        if (ExpirationDays > 0)
                        { c.Expires = DateTime.Today.AddDays(ExpirationDays); }

                        CurrResponse.Cookies.Add(c);

                        boolSuccess = true;
                        var msg = string.Format("{0}.SetCookie: {1}={2} SET", ThisClassName, CookieName, sCleanedCookieVal.ToString());
                        //Info.LogInfo(msg);
                    }
                    catch (Exception ex)
                    {   //something didn't work
                        boolSuccess = false;

                        var functionName = string.Format("{0}.SetCookie", ThisClassName);
                        var msg = string.Format("{0} {1}={2} NOT SET",functionName, CookieName.ToString(), sCleanedCookieVal.ToString());
                        ex.Data.Add("DragonflyException", msg);
                        throw ex;

                    }
                }
                else
                {
                    //else Previous cookie should not be overwritten
                    boolSuccess = false;
                }

                if (AlsoSaveToSession)
                {
                    HttpContext.Current.Session[CookieName] = sCleanedCookieVal;
                    var functionName = string.Format("{0}.SetCookie", ThisClassName);
                    var msg = string.Format("{0}: [{1}] Saved to Session State", functionName, CookieName);
                    //Info.LogInfo(msg);
                }
            }

            return boolSuccess;
        }

        /// <summary>
        /// Get a value from a cookie.
        /// </summary>
        /// <param name="CookieName">Name for Cookie</param>
        /// <param name="CheckSessionIfMissing">If the cookie is not found, should the session state be checked for the same key?</param>
        /// <returns>String of cookie/session data, empty string if no cookie/session is found.</returns>
        public static string GetCookie(string CookieName, bool CheckSessionIfMissing = false)
        {
            string CookieReturnValue = "";

            var CurrRequest = HttpContext.Current.Request;
            if (CurrRequest.Cookies[CookieName] != null)
            {
                //TODO: Update using new code pattern:
                //var functionName = string.Format("{0}.GetMySQLDataSet", ThisClassName);
                //var msg = string.Format("");
                //Info.LogInfo("Cookies.GetCookie[" + CookieName + "] = " + CurrRequest.Cookies[CookieName].Value.ToString());
                CookieReturnValue = CurrRequest.Cookies[CookieName].Value.ToString();
            }
            else
            {
                //TODO: Update using new code pattern:
                //var functionName = string.Format("{0}.GetMySQLDataSet", ThisClassName);
                //var msg = string.Format("");
                //Info.LogInfo("Cookies.GetCookie[" + CookieName + "] = NULL");
                CookieReturnValue = "";
            }

            if (CookieReturnValue == "" & CheckSessionIfMissing)
            {
                if (HttpContext.Current.Session[CookieName] != null)
                {
                    CookieReturnValue = HttpContext.Current.Session[CookieName].ToString();
                    //TODO: Update using new code pattern:
                    //var functionName = string.Format("{0}.GetMySQLDataSet", ThisClassName);
                    //var msg = string.Format("");
                    //Info.LogInfo("Cookies.GetCookie[" + CookieName + "] Retrieved from Session State");
                }
                
            }

            return CookieReturnValue;
        }


        /// <summary>
        /// Persists a CookieContainer to File storage
        /// </summary>
        /// <param name="FilePathName">Path &amp; filename for cookie storage</param>
        /// <param name="CookieJar">CookieContainer to Serialize</param>
        public static void WriteCookiesToDisk(string FilePathName, CookieContainer CookieJar)
        {
            using (Stream FStream = File.Create(FilePathName))
            {
                try
                {
                    BinaryFormatter BFormatter = new BinaryFormatter();
                    BFormatter.Serialize(FStream, CookieJar);
                    var functionName = string.Format("{0}.WriteCookiesToDisk", ThisClassName);
                    var msg = string.Format("{0} - Complete ({1})", functionName, FilePathName);
                    //Info.LogInfo(msg);
                }
                catch (Exception ex)
                {
                    var functionName = string.Format("{0}.WriteCookiesToDisk", ThisClassName);
                    var msg = string.Format("{0} FilePathName=({1})", functionName, FilePathName);
                    ex.Data.Add("DragonflyException", msg);
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Deserializes a file back into a CookieContainer
        /// </summary>
        /// <param name="FilePathName"></param>
        /// <returns></returns>
        public static CookieContainer ReadCookiesFromDisk(string FilePathName)
        {
            try
            {
                using (Stream FStream = File.Open(FilePathName, FileMode.Open))
                {
                    BinaryFormatter BFormatter = new BinaryFormatter();

                    var functionName = string.Format("{0}.ReadCookiesFromDisk", ThisClassName);
                    var msg = string.Format("{0} - Complete ({1})", functionName, FilePathName);
                    //Info.LogInfo(msg);

                    return (CookieContainer) BFormatter.Deserialize(FStream);
                }
            }
            catch (System.IO.FileNotFoundException exFile)
            {
                var functionName = string.Format("{0}.ReadCookiesFromDisk", ThisClassName);
                var msg = string.Format("FilePathName=({0}) - HANDLED BY CODE", FilePathName);
                //Info.LogException(functionName, FileEx, msg);
                return null;
            }
            catch (Exception ex)
            {
                var functionName = string.Format("{0}.ReadCookiesFromDisk", ThisClassName);
                var msg = string.Format("{0} FilePathName=({1})", functionName, FilePathName);
                ex.Data.Add("DragonflyException", msg);
                //throw ex;
                return new CookieContainer();
            }
        }

    }
}