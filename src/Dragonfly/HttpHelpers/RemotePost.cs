namespace Dragonfly.NetHelpers
{
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using System;
    using System.Collections.Specialized;
    using System.Net;
    using System.Text;
    using System.Web;
    //using System.Web.UI;

    // from assembly System.Web.Extensions

    public class RemotePost
    {
        private const string ThisClassName = "Dragonfly.NetHelpers.RemotePost";

        //enum PostDataType
        //{ Json
        //}

        //enum PostMethod
        //{
        //    Get,
        //    Post
        //}

        #region ---Private Variables---

        //public string PostDataType;

        private NameValueCollection Inputs = new NameValueCollection();
        private NameValueCollection InputsUnEncoded = new NameValueCollection();
        private string DefaultPostId = "";

        #endregion

        #region  ---Editable Public Properties---

        /// <summary>
        /// URL to Post To (String)
        /// </summary>
        public string Url = "";

        /// <summary>
        /// Method - "get" or "post" (String)
        /// </summary>
        public string Method = "post";

        /// <summary>
        /// Optional String for ID purposes
        /// </summary>
        public string FormName = "form1";

        public bool PreventFailedExpectationError = false;

        public CookieContainer CookieContainer;

        public bool DebugLoggingMode = false;
        public string DebugLogFilePath = "/iris-log.txt";
        public string PostId = "";

        #endregion

        #region  ---Non-editable Public Properties/Functions---

        public string FullPostingUrl(bool EncodeSpacesAs20 = false, bool DontEncodeFormData = false)
        {
            string FullPostingUrl = String.Concat(this.Url, this.GetQueryStringFormattedData(EncodeSpacesAs20, DontEncodeFormData));

            return FullPostingUrl;
        }

        /// <summary>
        /// Show all "Added" data
        /// </summary>
        /// <returns>A NameValueCollection of all the added Key/Value pairs URLencoded</returns>
        //public NameValueCollection DataCollection()
        //{
        //    return DataCollection(false);
        //}

        /// <summary>
        /// Show all "Added" data
        /// </summary>
        /// <param name="ReturnUnencodedData">Don't urlencode the data</param>
        /// <returns>A NameValueCollection of all the added Key/Value pairs</returns>
        public NameValueCollection DataCollection(bool ReturnUnencodedData = false)
        {
            if (ReturnUnencodedData)
            { return this.InputsUnEncoded; }
            else
            { return this.Inputs; }
        }

        ///// <summary>
        ///// Converts all "Added" data to a single string of query-string-ready params in the format "key1=value1&amp;key2=value2"
        ///// </summary>
        ///// <returns>string</returns>
        //public string GetQueryStringFormattedData()
        //{
        //    return GetQueryStringFormattedData(false, false);
        //}

        /// <summary>
        /// Converts all "Added" data to a single string of query-string-ready params in the format "key1=value1&amp;key2=value2"
        /// </summary>
        /// <param name="EncodeSpacesAs20">Encode spaces as '%20' instead of '+' (%2b)</param>
        /// <param name="DontEncodeFormData">Do not URL encode the data</param>
        /// <returns>string of query-string-ready params in the format "key1=value1&amp;key2=value2"</returns>
        public string GetQueryStringFormattedData(bool EncodeSpacesAs20 = false, bool DontEncodeFormData = false)
        {
            string QSData = "";

            if (DontEncodeFormData)
            {
                for (int i = 0; i < this.InputsUnEncoded.Keys.Count; i++)
                {
                    QSData += string.Format("{0}={1}&", this.InputsUnEncoded.Keys[i], this.InputsUnEncoded[this.InputsUnEncoded.Keys[i]]);
                }
            }
            else
            {
                for (int i = 0; i < this.Inputs.Keys.Count; i++)
                {
                    QSData += string.Format("{0}={1}&", this.Inputs.Keys[i], this.Inputs[this.Inputs.Keys[i]]);
                }
            }

            if (EncodeSpacesAs20)
            {
                QSData = QSData.Replace("%2b", "%20");
                QSData = QSData.Replace("%2540", "%40");
            }

            return QSData.TrimEnd('&');
        }

        public override string ToString()
        {
            StringBuilder DataText = new StringBuilder();

            for (int i = 0; i < this.Inputs.Keys.Count; i++)
            {
                DataText.AppendLine(string.Format("{0} = {1}", this.Inputs.Keys[i], HttpUtility.UrlDecode(this.Inputs[this.Inputs.Keys[i]])));
            }

            return DataText.ToString();

            //return base.ToString();
        }

        #endregion

        #region ---Instantiation---

        public RemotePost()
        {
            this.DefaultPostId = Guid.NewGuid().ToString();
        }

        #endregion

        #region ---Methods---

        /// <summary>
        /// Add a Name/Value pair to the post
        /// </summary>
        /// <param name="name">Key Name</param>
        /// <param name="value">Value</param>
        public void Add(string Name, string Value)
        {
            this.Inputs.Add(Name, HttpUtility.UrlEncode(Value));
            this.InputsUnEncoded.Add(Name, Value);
        }

        /// <summary>
        /// Update a Name/Value pair
        /// </summary>
        /// <param name="name">Key Name</param>
        /// <param name="value">Value</param>
        public void Edit(string Name, string NewValue)
        {
            this.Inputs.Remove(Name);
            this.Inputs.Add(Name, HttpUtility.UrlEncode(NewValue));
            this.InputsUnEncoded.Add(Name, NewValue);
        }

        ///// <summary>
        ///// Response from POST
        ///// </summary>
        ///// <returns></returns>
        //public string PostReturnResponse()
        //{
        //    return PostReturnResponse(false, false, false);
        //}


        /// <summary>
        /// POSTs to a URL and returns the response string
        /// </summary>
        /// <param name="UseFullPostingUrl">Use the full url, which includes the encoded POST data. default is to use the base URL</param>
        /// <param name="EncodeSpacesAs20">Fixes space-encoding to use %20 rather than %2b</param>
        /// <returns>Response from POSTed-to server</returns>
        public string PostReturnResponse(bool UseFullPostingUrl = false, bool EncodeSpacesAs20 = false, bool DontEncodeFormData = false)
        {
            //if (FixError417)
            //{
            //    System.Net.ServicePointManager.Expect100Continue = false;
            //}
            string ResponseString = "";
            string PostingUrl = this.Url;

            if (UseFullPostingUrl)
            {
                PostingUrl = this.FullPostingUrl(EncodeSpacesAs20);
            }

            this.LogPostInfo("RemotePost.PostReturnResponse PARAMS : "
                        + "[UseFullPostingUrl=" + UseFullPostingUrl
                        + "] [EncodeSpacesAs20=" + EncodeSpacesAs20
                        + "] [DontEncodeFormData" + DontEncodeFormData
                        + "]");



            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(PostingUrl);

            if (this.PreventFailedExpectationError == true)
            { req.ServicePoint.Expect100Continue = false; }

            //Add these, as we're doing a POST
            req.ContentType = "application/x-www-form-urlencoded";
            req.Method = "POST";
            this.LogPostInfo("RemotePost.PostReturnResponse : request.RequestUri=" + req.RequestUri);


            try
            {
                if (this.CookieContainer != null)
                {
                    this.LogPostInfo("RemotePost.PostReturnResponse : CookieContainer.Count=" + this.CookieContainer.Count);
                    req.CookieContainer = this.CookieContainer;
                    this.LogPostInfo("RemotePost.PostReturnResponse : req-num of Cookies = " + req.CookieContainer.Count);
                }
                else
                {
                    this.LogPostInfo("RemotePost.PostReturnResponse : CookieContainer IS NULL");
                }
            }
            catch (Exception ex)
            {
                this.LogPostInfo("RemotePost.PostReturnResponse : Error w. Cookie Container: " + ex.Message, true);
            }

            //We need to count how many bytes we're sending. 
            //Post'ed Faked Forms should be name=value&
            string AllData = this.GetQueryStringFormattedData(EncodeSpacesAs20, DontEncodeFormData);
            this.LogPostInfo("RemotePost.PostReturnResponse : QueryStringFormattedData=" + AllData);

            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(AllData);
            this.LogPostInfo("RemotePost.PostReturnResponse : bytes=" + bytes.ToString());
            req.ContentLength = bytes.Length;
            this.LogPostInfo("RemotePost.PostReturnResponse : req.ContentLength=" + req.ContentLength);
            System.IO.Stream os = req.GetRequestStream();
            os.Write(bytes, 0, bytes.Length); //Push it out there
            os.Close();

            try
            {
                System.Net.WebResponse resp = req.GetResponse();
                if (resp == null) return null;
                System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
                ResponseString = sr.ReadToEnd().Trim();
            }
            catch (Exception RespEx)
            {
                //TODO: Update using new code pattern:
                //var functionName = string.Format("{0}.GetMySQLDataSet", ThisClassName);
                //var msg = string.Format("");

                this.LogPostInfo("RemotePost.PostReturnResponse : Error during GetResponse:", true);
                //Info.LogException("RemotePost.PostReturnResponse", RespEx);
                ResponseString = "Error - " + RespEx.Message;
            }

            this.LogPostInfo("RemotePost.PostReturnResponse : ResponseString=" + ResponseString);

            return ResponseString;
        }

        public object PostReturnResponseAsObject(bool UseFullPostingUrl = false, bool EncodeSpacesAs20 = false)
        {
            string PostingUrl = this.Url;

            if (UseFullPostingUrl)
            {
                PostingUrl = this.FullPostingUrl(EncodeSpacesAs20);
            }

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(PostingUrl);

            if (this.PreventFailedExpectationError == true)
            { req.ServicePoint.Expect100Continue = false; }

            //Add these, as we're doing a POST
            req.ContentType = "application/x-www-form-urlencoded";
            req.Method = "POST";
            //TODO: Update using new code pattern:
            //var functionName = string.Format("{0}.GetMySQLDataSet", ThisClassName);
            //var msg = string.Format("");
            //Info.LogInfo("RemotePost.PostReturnResponseAsObject : request.RequestUri=" + req.RequestUri);

            if (this.CookieContainer != null)
            {
                req.CookieContainer = this.CookieContainer;
            }

            //We need to count how many bytes we're sending. 
            //Post'ed Faked Forms should be name=value&
            string test = this.GetQueryStringFormattedData(EncodeSpacesAs20);
            //TODO: Update using new code pattern:
            //var functionName = string.Format("{0}.GetMySQLDataSet", ThisClassName);
            //var msg = string.Format("");
            //Info.LogInfo("RemotePost.PostReturnResponseAsObject : QueryStringFormattedData=" + test);
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(this.GetQueryStringFormattedData(EncodeSpacesAs20));
            req.ContentLength = bytes.Length;
            System.IO.Stream os = req.GetRequestStream();
            os.Write(bytes, 0, bytes.Length); //Push it out there
            os.Close();
            System.Net.WebResponse resp = req.GetResponse();
            if (resp == null) return null;
            System.IO.StreamReader sr =
                  new System.IO.StreamReader(resp.GetResponseStream());
            return sr.ReadToEnd().Trim();
        }

        /// <summary>
        /// Do the POST and redirect to a different page
        /// </summary>
        /// <param name="Page">Current Page</param>
        /// <param name="DestinationUrl">Redirect to this URL</param>
        public void PostWithRedirect(Page Page, string DestinationUrl)
        {
            //Prepare the Posting form
            string strForm = this.PreparePOSTForm(DestinationUrl);

            //Add a literal control the specified page holding the Post Form, this is to submit the Posting form with the request.
            Page.Controls.Add(new LiteralControl(strForm));
        }



        #endregion

        #region ---Private Functions/Methods---

        private String PreparePOSTForm(string url)
        {
            //Set a name for the form
            string formID = "PostForm";

            //Build the form using the specified data to be posted.
            StringBuilder strForm = new StringBuilder();
            strForm.Append("<form id=\"" + formID + "\" name=\"" + formID + "\" action=\"" + url +
                           "\" method=\"POST\">");

            for (int i = 0; i < this.Inputs.Keys.Count; i++)
            {
                strForm.Append("<input type=\"hidden\" name=\"" + this.Inputs.Keys[i] + "\" value=\"" +
                               this.Inputs[this.Inputs.Keys[i]] + "\">");
            }

            strForm.Append("</form>");

            //Build the JavaScript which will do the Posting operation.
            StringBuilder strScript = new StringBuilder();
            strScript.Append("<script language='javascript'>");
            strScript.Append("var v" + formID + " = document." + formID + ";");
            strScript.Append("v" + formID + ".submit();");
            strScript.Append("</script>");

            //Return the form and the script concatenated. (The order is important, Form then JavaScript)
            return strForm.ToString() + strScript.ToString();
        }

        private void LogPostInfo(string InfoMsg, bool IsError = false)
        {
            string LogMsg = "";
            string PostIdString = "";

            if (this.PostId != "")
            {
                PostIdString = this.PostId;
            }
            else
            {
                PostIdString = this.DefaultPostId;
            }

            LogMsg = "{" + PostIdString + "} " + InfoMsg;

            //Info.LogInfo(LogMsg, IsError);
            if (this.DebugLoggingMode)
            {
                //Info.LogInfoToTextFile(this.DebugLogFilePath, LogMsg);
            }
        }

        #endregion

        #region ---Old/Temp Code---

        //public string PostReturnJSONResponse(string OAuthToken)
        //{
        //    byte[] postBytes = Encoding.ASCII.GetBytes(GetQueryStringFormattedData());
        //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
        //    request.ProtocolVersion = HttpVersion.Version11;
        //    request.Method = "POST";
        //    request.Accept = "application/json";
        //    request.ContentType = "application/json";
        //    request.Headers["Authorization"] = "OAuth oauth_token=" + OAuthToken;
        //    request.ContentLength = postBytes.Length;

        //    Stream requestStream = request.GetRequestStream();

        //    requestStream.Write(postBytes, 0, postBytes.Length);
        //    requestStream.Close();

        //    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

        //    StreamReader sr = new StreamReader(response.GetResponseStream());

        //    return sr.ReadToEnd();
        //}

        //public string PostReturnResponse(bool UseFullPostingUrl = false, bool EncodeSpacesAs20 = false)
        //{
        //    return PostReturnResponse(UseFullPostingUrl, EncodeSpacesAs20, false);
        //}

        //public void ContextPost()
        //{
        //    HttpContext.Current.Response.Clear();
        //    HttpContext.Current.Response.Write("");
        //    HttpContext.Current.Response.Write(string.Format("", FormName));
        //    HttpContext.Current.Response.Write(string.Format("", FormName, Method, Url));

        //    for (int i = 0; i < Inputs.Keys.Count; i++)
        //    {
        //        HttpContext.Current.Response.Write(string.Format("", Inputs.Keys[i],
        //                                                                    Inputs[Inputs.Keys[i]]));
        //    }

        //    HttpContext.Current.Response.Write("");
        //    HttpContext.Current.Response.Write("");
        //    HttpContext.Current.Response.End();
        //}

        #endregion

    }


}