namespace Dragonfly.NetHelpers
{
    using Microsoft.AspNetCore.Html;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using HtmlAgilityPack;

    public static class Html
    {
        #region Testing HtmlString Values

        /// <summary>
        /// Checks for content 
        /// </summary>
        /// <param name="HtmlString">String to test</param>
        /// <param name="EmptyParagraphsIsNull">Should a string made up of only empty &lt;p&gt; tags be considered null?</param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this HtmlString HtmlString, bool EmptyParagraphsIsNull)
        {
            if (HtmlString == null)
            {
                return true;
            }

            var testString = HtmlString.ToString();
            if (string.IsNullOrWhiteSpace(testString))
            {
                return true;
            }

            if (EmptyParagraphsIsNull)
            {
                testString = testString.RemoveAllParagraphTags(false).Trim();
                return string.IsNullOrWhiteSpace(testString);
            }

            //If we get here
            return false;
        }

        #endregion

        #region Altering HtmlString Values

        /// <summary>
        /// Takes a List of IHtmlStrings and concatenates them together using a provided delimiter
        /// </summary>
        /// <param name="List">List of IHtmlStrings</param>
        /// <param name="Delimiter">string to separate HTML Strings</param>
        /// <returns>IHtmlString</returns>
        public static HtmlString ToConcatenatedHtmlString(this List<HtmlString> List, string Delimiter)
        {
            var returnSB = new StringBuilder();

            foreach (var item in List)
            {
                returnSB.Append(item.ToString());

                if (item != List.Last())
                {
                    returnSB.Append(Delimiter);
                }
            }

            return new HtmlString(returnSB.ToString());
        }

        /// <summary>
        /// Replaces plain-text line breaks with &lt;br/&gt; tags
        /// </summary>
        /// <param name="StringToFix">Original string</param>
        /// <returns></returns>
        public static HtmlString ReplaceLineBreaksForWeb(this string StringToFix)
        {
            return new HtmlString(StringToFix.Replace("\r\n", "<br />").Replace("\n", "<br />"));
        }

        /// <summary>
        /// Strips out &lt;p&gt; and &lt;/p&gt; tags if they were used as a wrapper
        /// for other HTML content.
        /// </summary>
        /// <param name="Text">The HTML text.</param>
        /// <param name="ConvertEmptyParagraphsToBreaks"></param>
        public static HtmlString RemoveParagraphWrapperTags(this HtmlString Text, bool ConvertEmptyParagraphsToBreaks = false)
        {
            var str = Text.ToString();
            var fixedText = str.RemoveParagraphWrapperTags(ConvertEmptyParagraphsToBreaks);
            return new HtmlString(fixedText);
        }

        /// <summary>
        /// Remove all &lt;p&gt; tags
        /// </summary>
        /// <param name="Html"></param>
        /// <param name="RetainBreaks">Replaces the paragraph tag with two &lt;br&gt; tags</param>
        /// <returns></returns>
        public static HtmlString RemoveAllParagraphTags(this HtmlString Html, bool RetainBreaks)
        {
            var result = Html.ToString();

            if (RetainBreaks)
            {
                result = result.Replace("\r\n<p>", "<br/><br/>");
                result = result.Replace("</p><p>", "<br/><br/>");
            }
            result = result.Replace("<p>", "");
            result = result.Replace("</p>", " ");

            return new HtmlString(result);
        }


        /// <summary>
        /// Removes surrounding &lt;p&gt; tags
        /// </summary>
        /// <param name="HtmlToFix"></param>
        /// <returns></returns>
        public static HtmlString RemoveOuterParagrahTags(this HtmlString HtmlToFix)
        {
            string result = HtmlToFix.ToString().RemoveOuterParagrahTags();
            return new HtmlString(result);
        }


        /// <summary>
        /// Removes all Tags from IHtmlString
        /// </summary>
        /// <param name="Input">Original IHtmlString</param>
        /// <returns></returns>
        public static string StripHtml(this HtmlString Input)
        {
            var inputS = Input.ToString();
            return inputS.StripHtml();
        }

        /// <summary>
        /// Truncates a string to a given length, can add a ellipsis at the end (...).
        /// Method checks for open HTML tags, and makes sure to close them.
        /// </summary>
        public static HtmlString Truncate(this HtmlString Text, int Length, bool AddEllipsis, bool TreatTagsAsContent)
        {
            string html = Text.ToString();
            return html.TruncateHtml(Length, AddEllipsis, TreatTagsAsContent);

        }

        #endregion

        #region If
        /// <summary>
        /// If the test is true, the string valueIfTrue will be returned, otherwise the valueIfFalse will be returned.
        /// </summary>
        public static HtmlString If(bool Test, string ValueIfTrue, string ValueIfFalse)
        {
            return Test ? new HtmlString(ValueIfTrue) : new HtmlString(ValueIfFalse);
        }

        /// <summary>
        /// If the test is true, the string valueIfTrue will be returned, otherwise the valueIfFalse will be returned.
        /// </summary>
        public static HtmlString If(bool Test, HtmlString ValueIfTrue, HtmlString ValueIfFalse)
        {
            return Test ? ValueIfTrue : ValueIfFalse;
        }

        /// <summary>
        /// If the test is true, the string valueIfTrue will be returned, otherwise an empty string will be returned.
        /// </summary>
        public static HtmlString If(bool Test, string ValueIfTrue)
        {
            return Test ? new HtmlString(ValueIfTrue) : new HtmlString(string.Empty);
        }

        /// <summary>
        /// If the test is true, the string valueIfTrue will be returned, otherwise an empty string will be returned.
        /// </summary>
        public static HtmlString If(bool Test, HtmlString ValueIfTrue)
        {
            return Test ? ValueIfTrue : new HtmlString(string.Empty);
        }
        #endregion

        #region String-based

        /// <summary>
        /// Encodes the string
        /// </summary>
        /// <param name="OriginalString"></param>
        /// <returns></returns>
        public static string HtmlEncode(this string OriginalString)
        {
            return System.Web.HttpUtility.HtmlEncode(OriginalString);
        }

        /// <summary>
        /// Decodes the string
        /// </summary>
        /// <param name="EncodedString"></param>
        /// <returns></returns>
        public static string HtmlDecode(this string EncodedString)
        {
            return System.Web.HttpUtility.HtmlDecode(EncodedString);
        }

        /// <summary>
        /// Removes all Tags from string
        /// </summary>
        /// <param name="Input">Original string</param>
        /// <returns></returns>
        public static string StripHtml(this string Input)
        {
            return Regex.Replace(Input, "<.*?>", String.Empty);
        }

        /// <summary>
        /// Strips out &lt;p&gt; and &lt;/p&gt; tags if they were used as a wrapper
        /// for other HTML content.
        /// </summary>
        /// <param name="Text">The HTML text.</param>
        /// <param name="ConvertEmptyParagraphsToBreaks"></param>
        public static string RemoveParagraphWrapperTags(this string Text, bool ConvertEmptyParagraphsToBreaks = false)
        {
            if (string.IsNullOrEmpty(Text))
            {
                return Text;
            }

            string trimmedText = Text.Trim();

            if (ConvertEmptyParagraphsToBreaks)
            {
                //trimmedText = trimmedText.Replace("<p>", "<P>");
                //trimmedText = trimmedText.Replace("</p>", "</P>");

                trimmedText = trimmedText.RemoveDoubleSpaces();

                trimmedText = trimmedText.Replace("<p></p>", "<br/>");
                trimmedText = trimmedText.Replace("<P></P>", "<br/>");
                trimmedText = trimmedText.Replace("<p> </p>", "<br/>");
                trimmedText = trimmedText.Replace("<P> </P>", "<br/>");
                trimmedText = trimmedText.Replace("<p>&nbsp;</p>", "<br/>");
                trimmedText = trimmedText.Replace("<P>&nbsp;</P>", "<br/>");
            }

            string upperText = trimmedText.ToUpper();
            int paragraphIndex = upperText.IndexOf("<P>");

            if (paragraphIndex == -1 ||
                paragraphIndex != upperText.LastIndexOf("<P>") ||
                upperText.Substring(upperText.Length - 4, 4) != "</P>")
            {
                // Paragraph not used as a wrapper element
                return Text;
            }

            // Remove paragraph wrapper tags
            return trimmedText.Substring(3, trimmedText.Length - 7);
        }

        /// <summary>
        /// Removes surrounding &lt;p&gt; tags
        /// </summary>
        /// <param name="HtmlToFix"></param>
        /// <returns></returns>
        public static string RemoveOuterParagrahTags(this string HtmlToFix)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(HtmlToFix);
            string result = doc.DocumentNode.FirstChild.InnerHtml;

            return result;
        }

        /// <summary>
        /// Remove all &lt;p&gt; tags
        /// </summary>
        /// <param name="Html"></param>
        /// <param name="RetainBreaks">Replaces the paragraph tag with two &lt;br&gt; tags</param>
        /// <returns></returns>
        public static string RemoveAllParagraphTags(this string Html, bool RetainBreaks)
        {
            var result = new HtmlString(Html).RemoveAllParagraphTags(RetainBreaks);
            return result.ToString();
        }


        /// <summary>
        /// Add an absolute path to all the img tags in the html of a passed-in string.
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string AddImgAbsolutePath(this string HtmlString, Uri CurrentRequestUri)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(HtmlString);

            var uri = CurrentRequestUri;//new Uri(HttpContext.Current.Request.Url.AbsoluteUri);
            var domainUrl = string.Format("{0}://{1}", uri.Scheme, uri.Authority);

            if (doc.DocumentNode.SelectNodes("//img[@src]") != null)
            {
                foreach (HtmlNode img in doc.DocumentNode.SelectNodes("//img[@src]"))
                {
                    HtmlAttribute att = img.Attributes["src"];
                    if (att.Value.StartsWith("/"))
                    {
                        att.Value = domainUrl + att.Value;
                    }
                }
            }

            return doc.DocumentNode.InnerHtml;
        }

        /// <summary>
        /// Searches for Urls in a string and replaces them with full a href tags
        /// </summary>
        /// <param name="HtmlString">String to search and replace in</param>
        /// <param name="Target">Target for links (default = '_blank' (new window))</param>
        /// <returns></returns>
        public static string EnableHtmlLinks(this string HtmlString, string Target = "_blank")
        {
            var fixedHtml = HtmlString;

            var urlRegEx = @"(?:(?:https?|ftp|file):\/\/|www\.|ftp\.)(?:\([-A-Z0-9+&@#\/%=~_|$?!:,.]*\)|[-A-Z0-9+&@#\/%=~_|$?!:,.])*(?:\([-A-Z0-9+&@#\/%=~_|$?!:,.]*\)|[A-Z0-9+&@#\/%=~_|$])";
            //*var urlRegEx = @"(?:(?:https?|ftp):\/\/)?[\w/\-?=%.]+\.[\w/\-?=%.]+";
            //var urlRegEx = @"/(http|https|ftp|ftps)\:\/\/[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(\/\S*)?";
            //var urlRegEx = @"/(ftp:\/\/|www\.|https?:\/\/){1}[a-zA-Z0-9u00a1-\uffff0-]{2,}\.[a-zA-Z0-9u00a1-\uffff0-]{2,}(\S*)";
            //var urlRegEx = @"/[-a-zA-Z0-9@:%_\+.~#?&//=]{2,256}\.[a-z]{2,4}\b(\/[-a-zA-Z0-9@:%_\+.~#?&//=]*)?";
            //var urlRegEx = @"/([--:\w?@%&+~#=]*\.[a-z]{2,4}\/{0,2})((?:[?&](?:\w+)=(?:\w+))+|[--:\w?@%&+~#=]+)?/g";

            var matches = Regex.Matches(HtmlString, urlRegEx, RegexOptions.IgnoreCase);

            if (matches.Count > 0)
            {
                foreach (var match in matches)
                {
                    var matchUrl = match.ToString();
                    var linkHtml = $"<a href=\"{matchUrl}\" target=\"{Target}\">{matchUrl}</a>";
                    fixedHtml = fixedHtml.Replace(matchUrl, linkHtml);
                }

            }

            return fixedHtml;
        }

        /// <summary>
        /// Truncates a string to a given length, can add a ellipsis at the end (...).
        /// Method checks for open HTML tags, and makes sure to close them.
        /// </summary>
        public static HtmlString TruncateHtml(this string HtmlText, int Length, bool AddEllipsis, bool TreatTagsAsContent)
        {
            const string hellip = "&hellip;";
            string html = HtmlText;

            using (var outputms = new MemoryStream())
            {
                bool lengthReached = false;

                using (var outputtw = new StreamWriter(outputms))
                {
                    using (var ms = new MemoryStream())
                    {
                        using (var tw = new StreamWriter(ms))
                        {
                            tw.Write(html);
                            tw.Flush();
                            ms.Position = 0;
                            var tagStack = new Stack<string>();

                            using (TextReader tr = new StreamReader(ms))
                            {
                                bool isInsideElement = false,
                                    insideTagSpaceEncountered = false,
                                    isTagClose = false;

                                int ic = 0,
                                    //currentLength = 0,
                                    currentTextLength = 0;

                                string currentTag = string.Empty,
                                    tagContents = string.Empty;

                                while ((ic = tr.Read()) != -1)
                                {
                                    bool write = true;

                                    switch ((char)ic)
                                    {
                                        case '<':
                                            if (!lengthReached)
                                            {
                                                isInsideElement = true;
                                            }

                                            insideTagSpaceEncountered = false;
                                            currentTag = string.Empty;
                                            tagContents = string.Empty;
                                            isTagClose = false;
                                            if (tr.Peek() == (int)'/')
                                            {
                                                isTagClose = true;
                                            }
                                            break;

                                        case '>':
                                            isInsideElement = false;

                                            if (isTagClose && tagStack.Count > 0)
                                            {
                                                string thisTag = tagStack.Pop();
                                                outputtw.Write("</" + thisTag + ">");
                                                if (TreatTagsAsContent)
                                                {
                                                    currentTextLength++;
                                                }
                                            }
                                            if (!isTagClose && currentTag.Length > 0)
                                            {
                                                if (!lengthReached)
                                                {
                                                    tagStack.Push(currentTag);
                                                    outputtw.Write("<" + currentTag);
                                                    if (TreatTagsAsContent)
                                                    {
                                                        currentTextLength++;
                                                    }
                                                    if (!string.IsNullOrEmpty(tagContents))
                                                    {
                                                        if (tagContents.EndsWith("/"))
                                                        {
                                                            // No end tag e.g. <br />.
                                                            tagStack.Pop();
                                                        }

                                                        outputtw.Write(tagContents);
                                                        write = true;
                                                        insideTagSpaceEncountered = false;
                                                    }
                                                    outputtw.Write(">");
                                                }
                                            }
                                            // Continue to next iteration of the text reader.
                                            continue;

                                        default:
                                            if (isInsideElement)
                                            {
                                                if (ic == (int)' ')
                                                {
                                                    if (!insideTagSpaceEncountered)
                                                    {
                                                        insideTagSpaceEncountered = true;
                                                    }
                                                }

                                                if (!insideTagSpaceEncountered)
                                                {
                                                    currentTag += (char)ic;
                                                }
                                            }
                                            break;
                                    }

                                    if (isInsideElement || insideTagSpaceEncountered)
                                    {
                                        write = false;
                                        if (insideTagSpaceEncountered)
                                        {
                                            tagContents += (char)ic;
                                        }
                                    }

                                    if (!isInsideElement || TreatTagsAsContent)
                                    {
                                        currentTextLength++;
                                    }

                                    if (currentTextLength <= Length || (lengthReached && isInsideElement))
                                    {
                                        if (write)
                                        {
                                            var charToWrite = (char)ic;
                                            outputtw.Write(charToWrite);
                                            //currentLength++;
                                        }
                                    }

                                    if (!lengthReached)
                                    {
                                        if (currentTextLength == Length)
                                        {
                                            // if the last character added was the first of a two character unicode pair, add the second character
                                            if (char.IsHighSurrogate((char)ic))
                                            {
                                                var lowSurrogate = tr.Read();
                                                outputtw.Write((char)lowSurrogate);
                                            }

                                        }
                                        // only add elipsis if current length greater than original length
                                        if (currentTextLength > Length)
                                        {
                                            if (AddEllipsis)
                                            {
                                                outputtw.Write(hellip);
                                            }
                                            lengthReached = true;
                                        }
                                    }

                                }

                            }
                        }
                    }
                    outputtw.Flush();
                    outputms.Position = 0;
                    using (TextReader outputtr = new StreamReader(outputms))
                    {
                        string result = string.Empty;

                        string firstTrim = outputtr.ReadToEnd().Replace("  ", " ").Trim();

                        // Check to see if there is an empty char between the hellip and the output string
                        // if there is, remove it
                        if (AddEllipsis && lengthReached && string.IsNullOrWhiteSpace(firstTrim) == false)
                        {
                            result = firstTrim[firstTrim.Length - hellip.Length - 1] == ' ' ? firstTrim.Remove(firstTrim.Length - hellip.Length - 1, 1) : firstTrim;
                        }
                        else
                        {
                            result = firstTrim;
                        }

                        return new HtmlString(result);
                    }
                }
            }
        }


        #endregion
    }

}
