namespace Dragonfly.NetHelpers
{
    using System;
    using System.Net.Mail;
    using System.Text;
    using System.Web;

    public class Email
    {
        private const string ThisClassName = "Dragonfly.NetHelpers.Email";

        public static string Send(string FromEmailAddress, string ToEmailAddress, string SubjectLine, string EmailMessage, bool IsHTML = false)
        {
            string Response = "";
            
            try
		    {
			    using (MailMessage mail = new MailMessage())
			    {
				    mail.From = new MailAddress(FromEmailAddress, FromEmailAddress);
				    //mail.ReplyTo = new MailAddress(FromEmailAddress, FromEmailAddress);

				    mail.To.Add(ToEmailAddress);
				    mail.Subject = SubjectLine;
                    mail.Body = EmailMessage;
                    //mail.Body = "<div style=\"font: 11px verdana, arial\">";
                    //mail.Body += Server.HtmlEncode(message).Replace("\n", "<br />") + "<br /><br />";
                    //mail.Body += "<hr /><br />";
                    //mail.Body += "<h3>Author information</h3>";
                    //mail.Body += "<div style=\"font-size:10px;line-height:16px\">";
                    //mail.Body += "<strong>Name:</strong> " + Server.HtmlEncode(name) + "<br />";
                    //mail.Body += "<strong>E-mail:</strong> " + Server.HtmlEncode(email) + "<br />";

                    if (IsHTML)
                    { mail.IsBodyHtml = true; }

                    mail.BodyEncoding = Encoding.UTF8;

                    SmtpClient Smtp = new SmtpClient();
                    Smtp.Send(mail);

			    }

			    Response = "Success";
		    }
		    catch (Exception ex)
		    {
			     if (ex.InnerException != null)
                    {
                        Response = ex.InnerException.Message;
                    }
                    else
                    {
                        Response = ex.Message;
                    }
			    }
		   
            //try 
            //{	
            //    MailMessage mailObj = new MailMessage(FromEmailAddress, ToEmailAddress, SubjectLine, EmailMessage);
            //    SmtpClient SMTPServer = new SmtpClient();
                   
            //    SMTPServer.Send(mailObj);
            //    Response = "Success";
            //}
            //catch (Exception ex)
            //{
            //    Response = Info.CreateErrorEmailMsg(ex, "Email.Send()");
            //}
            
            return Response;
        }

        /// <summary>
        /// Sends a MailMessage object using the SMTP settings.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// Error message, if any.
        /// </returns>
        private static string SendMailMessage(MailMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            StringBuilder errorMsg = new StringBuilder();

            try
            {
                message.BodyEncoding = Encoding.UTF8;

                SmtpClient Smtp = new SmtpClient();
                    Smtp.Send(message);

                //var smtp = new SmtpClient(BlogSettings.Instance.SmtpServer);

                //// don't send credentials if a server doesn't require it,
                //// linux smtp servers don't like that 
                //if (!string.IsNullOrEmpty(BlogSettings.Instance.SmtpUserName))
                //{
                //    smtp.Credentials = new NetworkCredential(
                //        BlogSettings.Instance.SmtpUserName, BlogSettings.Instance.SmtpPassword);
                //}

                //smtp.Port = BlogSettings.Instance.SmtpServerPort;
                //smtp.EnableSsl = BlogSettings.Instance.EnableSsl;
                //smtp.Send(message);

            }
            catch (Exception ex)
            {
                errorMsg.Append("Error sending email in SendMailMessage: ");
                Exception current = ex;

                while (current != null)
                {
                    if (errorMsg.Length > 0) { errorMsg.Append(" "); }
                    errorMsg.Append(current.Message);
                    current = current.InnerException;
                }

                //TODO: Update using new code pattern:
                //var functionName = string.Format("{0}.GetMySQLDataSet", ThisClassName);
                //var msg = string.Format("");
                //Info.LogException("Email.SendMailMessage", ex, errorMsg.ToString());
            }
            finally
            {
                // Remove the pointer to the message object so the GC can close the thread.
                message.Dispose();
            }

            return errorMsg.ToString();
        }

        /// <summary>
        /// Data structure for storing mail related items
        /// </summary>
        public class MailVariables
        {
            public MailVariables() { this.IsReady = false; }

            public string BodyContent { get; set; }
            public string Subject { get; set; }
            public string To { get; set; }
            public string ToName { get; set; }
            public string From { get; set; }
            public string FromName { get; set; }
            public string ReplyTo { get; set; }
            public bool EnableSsl { get; set; }
            public bool IsReady { get; set; }
            public bool IsHtml { get; set; }

        }

        ///// <summary>
        ///// Gets mail variables from EmailMessage document type
        ///// </summary>
        ///// <param name="nodeId">
        ///// The node id of the EmailMessage
        ///// </param>
        ///// <returns>
        ///// The <see cref="MailVariables"/>.
        ///// </returns>
        //public static MailVariables GetMailVariables(int NodeId, out string Result)
        //{
        //    var umbContentService = ApplicationContext.Current.Services.ContentService;
        //    var emf = umbContentService.GetById(nodeId);

        //    MailVariables mailvars = new MailVariables();
        //    try
        //    {
        //        mailvars.From = emf.GetValue<string>("from");
        //        mailvars.FromName = emf.GetValue<string>("fromName");
        //        mailvars.To = emf.GetValue<string>("to");
        //        mailvars.ToName = emf.GetValue<string>("toName");
        //        mailvars.Subject = emf.GetValue<string>("subject");
        //        mailvars.BodyContent = emf.GetValue<string>("content");
        //        mailvars.IsReady = true;

        //        SendResult = "Sent";
        //    }
        //    catch (Exception ex)
        //    {
        //        var Msg = string.Format("Error creating or MailVariables. Exception: {0}", ex.Message);
        //        LogHelper.Error(typeof(Email), Msg, new Exception(Msg));
        //    }
        //    return mailvars;
        //}

        /// <summary>
        /// Attempts to send an email with mail variable package passed-in
        /// </summary>
        /// <param name="package">
        /// The The <see cref="MailVariables"/> package.
        /// </param>
        /// <returns>
        /// <see cref="bool"/> indicating successful send.
        /// </returns>
        public static bool TrySendMail(MailVariables MailPackage, out string SendResult)
        {
            try
            {
                var msg = new System.Net.Mail.MailMessage();
                msg.From = new System.Net.Mail.MailAddress(MailPackage.From, HttpUtility.HtmlEncode(MailPackage.FromName));
                msg.Subject = MailPackage.Subject;
                msg.Body = MailPackage.BodyContent;
                msg.IsBodyHtml = MailPackage.IsHtml;

                msg.To.Add(new System.Net.Mail.MailAddress(HttpUtility.HtmlEncode(MailPackage.To), HttpUtility.HtmlEncode(MailPackage.ToName)));

                var smtp = new System.Net.Mail.SmtpClient { EnableSsl = MailPackage.EnableSsl };
                smtp.Send(msg);

                SendResult = "Sent";
                return true;
            }
            catch (Exception ex)
            {
                SendResult = string.Concat("TrySendMail: ", string.Format("Error creating or sending email, exception: {0}", ex.Message));
                //LogHelper.Error(typeof(Email), Msg, new Exception(Msg));
            }

            return false;
        }

        ///// <summary>
        ///// Given the set of replacement values and a list of email fields, construct and send the required emails.
        ///// </summary>
        ///// <param name="emailValues">The replacement values</param>
        ///// <param name="formAliases">The node property aliases, relevant to the current node.</param>
        //public static void ProcessForms(this UmbracoHelper umbraco, Dictionary<string, string> emailValues, IEnumerable<EmailFields> emailFieldsList, EmailType? emailType, bool addFiles = false)
        //{
        //    var streams = new Dictionary<string, MemoryStream>();

        //    if (addFiles)
        //    {
        //        var files = HttpContext.Current.Request.Files;
        //        foreach (string fileKey in files)
        //        {
        //            var file = files[fileKey];

        //            //Only add the file if one has been selected.
        //            if (file.ContentLength > 0)
        //            {
        //                file.InputStream.Position = 0;
        //                var memoryStream = new MemoryStream();
        //                file.InputStream.CopyTo(memoryStream);
        //                streams.Add(file.FileName, memoryStream);
        //            }
        //        }
        //    }

        //    foreach (var emailFields in emailFieldsList)
        //    {
        //        if (emailFields.Send
        //            && !string.IsNullOrWhiteSpace(emailFields.SenderName)
        //            && !string.IsNullOrWhiteSpace(emailFields.SenderEmail)
        //            && !string.IsNullOrWhiteSpace(emailFields.ReceiverEmail)
        //            && !string.IsNullOrWhiteSpace(emailFields.Subject)
        //            )
        //        {
        //            var attachments = new Dictionary<string, MemoryStream>();
        //            foreach (var stream in streams)
        //            {
        //                var memoryStream = new MemoryStream();
        //                stream.Value.Position = 0;
        //                stream.Value.CopyTo(memoryStream);
        //                memoryStream.Position = 0;
        //                attachments.Add(stream.Key, memoryStream);
        //            }

        //            ReplacePlaceholders(emailFields, emailValues);
        //            emailFields.Body = AddImgAbsolutePath(emailFields.Body);
        //            Umbraco.SendEmail(
        //                emailFields.SenderEmail,
        //                emailFields.SenderName,
        //                emailFields.ReceiverEmail,
        //                emailFields.Subject,
        //                emailFields.Body,
        //                emailFields.CcEmail,
        //                emailFields.BccEmail,
        //                emailType: emailType,
        //                addFiles: addFiles,
        //                attachments: attachments
        //                );
        //        }
        //    }
        //}
    }
}