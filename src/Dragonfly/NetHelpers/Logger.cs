namespace Dragonfly.NetHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mail;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;


    public static class Logger
    {
        //TODO: Refactor with SOLID

        private const string ThisClassName = "Dragonfly.NetHelpers.Logger";

        private const string TracePrefix = "";

        private const string DefaultLogEmailAddress = "";

        /// <summary>
        /// Writes information to the Trace log. 
        /// </summary>
        /// <param name="LogMessage">Text to add to log</param>
        /// <param name="IsError">If TRUE, will be displayed in RED</param>
        public static void LogInfo(string LogMessage, bool IsError = false)
        {
            HttpContext cur = HttpContext.Current;
            if (cur != null)
            {
                if (IsError)
                {
                    cur.Trace.Warn(Logger.TracePrefix, LogMessage);
                }
                else
                {
                    cur.Trace.Write(Logger.TracePrefix, LogMessage);
                }
            }
        }

        public static void LogInfoToTextFile(string FilePath, string LogMessage)
        {
            HttpContext cur = HttpContext.Current;
            if (cur != null)
            {
                if (!String.IsNullOrEmpty(LogMessage))
                {
                    Files.WriteToTextFile(FilePath,
                                    "[" + Logger.TracePrefix + "] "
                                    + LogMessage
                        );
                }
            }
        }

        public static string SendLogEmail(string LogMessage, string FromEmailAddress, string SubjectLine, string LogEmailAddress = Logger.DefaultLogEmailAddress)
        {
            string Response = "";

            MailMessage mailObj = new MailMessage(
                FromEmailAddress, LogEmailAddress, SubjectLine, LogMessage);
            SmtpClient SMTPServer = new SmtpClient();

            try
            {
                SMTPServer.Send(mailObj);
                Response = "Success";
            }
            catch (Exception ex)
            {
                Response = ex.ToString();
            }

            return Response;
        }

        /// <summary>
        /// Writes information about exceptions to the Trace Log.
        /// </summary>
        /// <param name="FunctionName">Location where error is occurring ('namespace: method')</param>
        /// <param name="ErrorException">the generated exception (from a try/catch)</param>
        /// <param name="AdditionalLogMessage">Additional information to include in the trace (variable values, notes, etc)</param>
        public static void LogException(string FunctionName, Exception ErrorException, string AdditionalLogMessage = "", bool NotError = false)
        {
            HttpContext cur = HttpContext.Current;
            if (cur != null)
            {
                string strExtraMsg = "";
                if (AdditionalLogMessage != "")
                {
                    strExtraMsg = "{" + AdditionalLogMessage + "}";
                }

                if (ErrorException != null)
                {
                    string TraceMsg = FunctionName
                                      + ": (" + ErrorException.GetType().ToString() + ") "
                                      + ErrorException.Message
                                      + strExtraMsg
                                      + " [" + ErrorException.Source.ToString() + "] "
                        ;
                    if (ErrorException.InnerException != null)
                    {
                        TraceMsg += "/n INNER EXCEPTION:"
                                    + "/n (" + ErrorException.InnerException.GetType().ToString() + ") "
                                    + ErrorException.InnerException.Message
                                    + " [" + ErrorException.InnerException.Source.ToString() + "] "
                            ;
                    }
                    if (NotError)
                    {
                        cur.Trace.Write(Logger.TracePrefix, TraceMsg);
                    }
                    else
                    {
                        cur.Trace.Warn(Logger.TracePrefix, TraceMsg);
                    }

                }
                else
                {
                    if (NotError)
                    {
                        cur.Trace.Write(Logger.TracePrefix, FunctionName + ": " + strExtraMsg);
                    }
                    else
                    {
                        cur.Trace.Warn(Logger.TracePrefix, FunctionName + ": " + strExtraMsg);
                    }
                }
            }
        }

        public static void LogExceptionToTextFile(string FilePath, string FunctionName, Exception ErrorException, string AdditionalLogMessage = "")
        {
            HttpContext cur = HttpContext.Current;
            if (cur != null)
            {
                string strExtraMsg = "";
                if (AdditionalLogMessage != "")
                {
                    strExtraMsg = "{" + AdditionalLogMessage + "}";
                }

                if (ErrorException != null)
                {
                    Files.WriteToTextFile(FilePath,
                                    "[" + Logger.TracePrefix + "] "
                                    + FunctionName
                                    + ": (" + ErrorException.GetType().ToString() + ") "
                                    + ErrorException.Message
                                    + strExtraMsg
                                    + " [" + ErrorException.Source.ToString() + "] "
                        );
                }
                else
                {
                    Files.WriteToTextFile(Logger.TracePrefix, FunctionName + ": " + strExtraMsg);
                }
            }
        }

        public static string CreateErrorEmailMsg(Exception ErrorException, string FunctionName)
        {
            StringBuilder ErrorMessage = new StringBuilder();
            ErrorMessage.AppendLine("Error occurred in " + FunctionName + ".");
            ErrorMessage.AppendLine("The Error details are:");
            ErrorMessage.AppendLine("TYPE:" + ErrorException.GetType().ToString());
            ErrorMessage.AppendLine("MESSAGE:" + ErrorException.Message);
            ErrorMessage.AppendLine("SOURCE:" + ErrorException.Source);
            ErrorMessage.AppendLine("STACK TRACE:" + ErrorException.StackTrace);

            return ErrorMessage.ToString();
        }


    }
}
