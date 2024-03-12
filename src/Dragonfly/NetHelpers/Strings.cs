namespace Dragonfly.NetHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using HtmlAgilityPack;
    using Microsoft.AspNetCore.Html;

    /// <summary>
    /// Helpers dealing with text strings
    /// </summary>
    [Serializable]
    public static class Strings
    {
        #region Testing String Values

        /// <summary>
        /// Check whether the string is an exact match to a provided RegEx
        /// </summary>
        /// <param name="RegularExpression">Expression to test against</param>
        /// <param name="StringToTest">Data to test</param>
        /// <param name="CaseSensitive">Does casing matter?</param>
        /// <returns></returns>
        public static bool RegExMatchesExactly(string RegularExpression, string StringToTest, bool CaseSensitive = false)
        {
            bool bolResult;
            Match match;

            if (CaseSensitive)
            { match = Regex.Match(StringToTest, RegularExpression); }
            else
            { match = Regex.Match(StringToTest, RegularExpression, RegexOptions.IgnoreCase); }

            if (match.ToString() == StringToTest)
            { bolResult = true; }
            else
            { bolResult = false; }


            return bolResult;
        }


        /// <summary>
        /// Tests whether a string contains any of the separate values specified in  a delimited string list
        /// </summary>
        /// <param name="StringToTest">The string which might contain one or more of the various values</param>
        /// <param name="DelimitedListOfTestValues">All the values to test</param>
        /// <param name="DelimChar">Character used as the delimiter</param>
        /// <returns>TRUE if any of the values appears in the string. FALSE if NONE of the values appear in the string</returns>
        public static bool ContainsValueFromList(this string StringToTest, string DelimitedListOfTestValues, char DelimChar, bool CaseSensitive = false)
        {
            bool ValueIsInString = false;
            int TotalMatches = 0;

            string StringToTestFin = "";
            string DelimitedListOfTestValuesFin = "";

            if (!CaseSensitive)
            {
                StringToTestFin = StringToTest.ToLower();
                DelimitedListOfTestValuesFin = DelimitedListOfTestValues.ToLower();
            }
            else
            {
                StringToTestFin = StringToTest;
                DelimitedListOfTestValuesFin = DelimitedListOfTestValues;
            }

            List<string> ValuesList = DelimitedListOfTestValuesFin.Split(DelimChar).ToList();

            for (int i = 0; i < ValuesList.Count; i++)
            {
                string ThisValue = ValuesList[i].ToString();

                if (StringToTestFin.Contains(ThisValue))
                {
                    TotalMatches++;
                }
            }

            if (TotalMatches > 0)
            { ValueIsInString = true; }

            return ValueIsInString;

        }

        /// <summary>
        /// Tests whether two provided Url strings are on the same domain (host). 
        /// </summary>
        /// <param name="Url1"></param>
        /// <param name="Url2"></param>
        /// <param name="RelativeUrlAssumedDomain">Provide the "assumed" domain for any relative urls.</param>
        /// <returns></returns>
        public static bool UrlsAreOnSameDomain(string Url1, string Url2, string RelativeUrlAssumedDomain)
        {
            if (Url1.StartsWith("/") | Url1.StartsWith("~"))
            {
                Url1 = RelativeUrlAssumedDomain + Url1;
            }

            if (Url2.StartsWith("/") | Url2.StartsWith("~"))
            {
                Url2 = RelativeUrlAssumedDomain + Url2;
            }

            Uri Uri1 = new Uri(Url1);
            Uri Uri2 = new Uri(Url2);

            // There are overloads for the constructor too
            //Uri uri3 = new Uri(url3, UriKind.Absolute);

            if (Uri1.Host == Uri2.Host)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Can the provided string be converted to a decimal value?
        /// </summary>
        /// <param name="StringToTest"></param>
        /// <returns></returns>
        public static bool IsNumeric(this string StringToTest)
        {
            try
            {
                decimal testDec = Decimal.Parse(StringToTest);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        /// <summary>
        /// Test whether a string can be converted to a real date
        /// </summary>
        /// <param name="DateStringToTest"></param>
        /// <param name="DateFormat"></param>
        /// <returns></returns>
        public static bool IsValidDate(this string DateStringToTest, string DateFormat)
        {
            bool isValid = false;
            try
            {
                var dateTest = DateTime.ParseExact(DateStringToTest, DateFormat, null);

                if (dateTest != null)
                {
                    isValid = true;
                }
            }
            catch (Exception exNonValidDate)
            {
                isValid = false;
            }

            return isValid;
        }


        /// <summary>
        /// Is the provided string a properly formatted email address?
        /// </summary>
        /// <param name="EmailToTest"></param>
        /// <returns></returns>
        public static bool IsValidEmailAddress(this string EmailToTest)
        {
            bool result;
            Regex rgx = new Regex(@"([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})");

            result = rgx.IsMatch(EmailToTest) ? true : false;

            return result;
        }

        /// <summary>
        /// Test whether a string can be rendered successfully in an HTML document
        /// </summary>
        /// <param name="TextToTest">HTML string to test</param>
        /// <param name="ValidationMsg">Returns a message about any errors</param>
        /// <param name="ErrorsList">Returns a list of errors</param>
        /// <returns>TRUE if valid, FALSE if not</returns>
        public static bool IsValidHtml(this string TextToTest, out string ValidationMsg, out IEnumerable<HtmlParseError> ErrorsList)
        {
            if (!string.IsNullOrWhiteSpace(TextToTest))
            {
                //Validate HTML
                HtmlDocument doc = new HtmlDocument();

                doc.LoadHtml(TextToTest);

                if (doc.ParseErrors.Any())
                {
                    //Invalid HTML
                    ValidationMsg = "Invalid HTML. Errors:" + string.Join("; ", doc.ParseErrors);
                    ErrorsList = doc.ParseErrors;
                    return false;
                }
                else
                {
                    ValidationMsg = "Valid HTML";
                    ErrorsList = new List<HtmlParseError>();
                    return true;
                }
            }
            else
            {
                ValidationMsg = "String is blank.";
                ErrorsList = new List<HtmlParseError>();
                return true;
            }
        }


        /// <summary>
        /// Test whether a string can be rendered successfully in an HTML document
        /// </summary>
        /// <param name="TextToTest">HTML string to test</param>
        /// <param name="ValidationMsg">Returns a message about any errors</param>
        /// <returns>TRUE if valid, FALSE if not</returns>
        public static bool IsValidHtml(this string TextToTest, out string ValidationMsg)
        {
            if (!string.IsNullOrWhiteSpace(TextToTest))
            {
                //Validate HTML
                HtmlDocument doc = new HtmlDocument();

                doc.LoadHtml(TextToTest);

                if (doc.ParseErrors.Any())
                {
                    //Invalid HTML
                    ValidationMsg = "Invalid HTML. Errors:" + string.Join("; ", doc.ParseErrors);
                    return false;
                }
                else
                {
                    ValidationMsg = "Valid HTML";
                    return true;
                }
            }
            else
            {
                ValidationMsg = "String is blank.";
                return true;
            }
        }

        /// <summary>
        /// Test whether a string can be rendered successfully in an HTML document
        /// </summary>
        /// <param name="TextToTest">HTML string to test</param>
        /// <param name="ErrorsList">Returns a list of errors</param>
        /// <returns>TRUE if valid, FALSE if not</returns>
        public static bool IsValidHtml(this string TextToTest, out IEnumerable<HtmlParseError> ErrorsList)
        {
            if (!string.IsNullOrWhiteSpace(TextToTest))
            {
                //Validate HTML
                HtmlDocument doc = new HtmlDocument();

                doc.LoadHtml(TextToTest);

                if (doc.ParseErrors.Any())
                {
                    //Invalid HTML
                    ErrorsList = doc.ParseErrors;
                    return false;
                }
                else
                {
                    ErrorsList = new List<HtmlParseError>();
                    return true;
                }
            }
            else
            {
                ErrorsList = new List<HtmlParseError>();
                return true;
            }
        }


        [Obsolete("Use identical string extension function 'ContainsValueFromList()'")]
        public static bool StringContainsValueFromList(string StringToTest, string DelimitedListOfTestValues, char DelimChar, bool CaseSensitive = false)
        {
            return StringToTest.ContainsValueFromList(DelimitedListOfTestValues, DelimChar, CaseSensitive);

        }

        [Obsolete("Use identical string extension function 'IsValidEmailAddress()'")]
        public static bool EmailAddressIsValid(string EmailToTest)
        {
            //bool Result;
            //Regex rgx = new Regex(@"([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})");

            //Result = rgx.IsMatch(EmailToTest) ? true : false;

            return EmailToTest.IsValidEmailAddress();
        }

        #endregion

        #region Altering String Values

        /// <summary>
        /// Replaces various special characters in a string
        /// </summary>
        /// <param name="StringToFix"></param>
        /// <param name="WordSeparator">Replace spaces, etc with this</param>
        /// <returns></returns>
        public static string ReplaceBadChars(this string StringToFix, string WordSeparator = "-")
        {
            string NewString = StringToFix;

            NewString = NewString.Replace("'", "");
            NewString = NewString.Replace("\"", WordSeparator);
            NewString = NewString.Replace("\\", WordSeparator);
            NewString = NewString.Replace("|", WordSeparator);
            NewString = NewString.Replace("®", "");
            NewString = NewString.Replace("%", "");
            NewString = NewString.Replace(".", WordSeparator);
            NewString = NewString.Replace(";", WordSeparator);
            NewString = NewString.Replace(":", WordSeparator);
            NewString = NewString.Replace("#", "");
            NewString = NewString.Replace("+", WordSeparator);
            NewString = NewString.Replace("=", WordSeparator);
            NewString = NewString.Replace("[", "");
            NewString = NewString.Replace("]", "");
            NewString = NewString.Replace("{", "");
            NewString = NewString.Replace("}", "");
            NewString = NewString.Replace("*", "");
            NewString = NewString.Replace("&", "and");
            NewString = NewString.Replace("?", "");
            NewString = NewString.Replace("*", "");
            NewString = NewString.Replace("æ", "ae");
            NewString = NewString.Replace("ø", "oe");
            NewString = NewString.Replace("å", "aa");
            NewString = NewString.Replace("ä", "ae");
            NewString = NewString.Replace("Ä", "ae");
            NewString = NewString.Replace("ö", "oe");
            NewString = NewString.Replace("ü", "ue");
            NewString = NewString.Replace("ß", "ss");
            NewString = NewString.Replace("Ö", "oe");

            NewString = NewString.Replace(" ", WordSeparator);

            return NewString;
        }

        /// <summary>
        /// Swap out a bunch of values with replacements via Dictionary (Good for "merge"-type operations)
        /// </summary>
        /// <param name="OriginalString"></param>
        /// <param name="ReplacementsDictionary"></param>
        /// <returns></returns>
        public static string ReplaceMultiple(this string OriginalString, Dictionary<string, string> ReplacementsDictionary)
        {
            var finalString = OriginalString;

            foreach (var replacement in ReplacementsDictionary)
            {
                finalString = finalString.Replace(replacement.Key, replacement.Value);
            }

            return finalString;
        }

        /// <summary>
        /// Clean up a string so it is safe to use as a code name
        /// </summary>
        /// <param name="StringToFix"></param>
        /// <param name="WordSeparator"></param>
        /// <param name="ConvertNumbersToWords"></param>
        /// <returns></returns>
        public static string MakeCodeSafe(this string StringToFix, string WordSeparator = "-", bool ConvertNumbersToWords = false)
        {
            string newString = StringToFix;

            if (ConvertNumbersToWords)
            {
                newString = NumeralsToWords(newString);
            }

            newString = ReplaceBadChars(newString, WordSeparator);

            newString = newString.Replace(" ", "");

            if (WordSeparator != "")
            {
                string separatorDoubled = string.Concat(WordSeparator, WordSeparator);
                bool dupedSeparator = newString.Contains(separatorDoubled);
                do
                {
                    newString = newString.Replace(separatorDoubled, WordSeparator);
                    dupedSeparator = newString.Contains(separatorDoubled);

                } while (dupedSeparator);
            }

            return newString;
        }

        /// <summary>
        /// Replace various numeric values with text versions. Handles fractions and single-digits (ex:"1/2"="Half", "23" = "TwoThree")
        /// </summary>
        /// <param name="StringToFix"></param>
        /// <param name="DoComplexReplacements">Convert fractions as fraction text (if false, will treat fractions like single digits)</param>
        /// <param name="Capitalize">Return as Pascal-Cased (false will return all lowercase)</param>
        /// <returns></returns>
        public static string NumeralsToWords(this string StringToFix, bool DoComplexReplacements = true, bool Capitalize = true)
        {
            string newString = StringToFix;

            if (DoComplexReplacements)
            {
                newString = newString.Replace("1/2", "Half");
                newString = newString.Replace("1/3", "OneThird");
                newString = newString.Replace("2/3", "TwoThirds");
                newString = newString.Replace("1/4", "OneFourth");
                newString = newString.Replace("3/4", "ThreeFourths");
                newString = newString.Replace("1/5", "OneFifth");
                newString = newString.Replace("2/5", "TwoFifths");
                newString = newString.Replace("3/5", "ThreeFifths");
                newString = newString.Replace("4/5", "FourFifths");
                newString = newString.Replace("1/6", "OneSixth");
                newString = newString.Replace("5/6", "FiveSixths");
                newString = newString.Replace("1/7", "OneSeventh");
                newString = newString.Replace("2/7", "TwoSevenths");
                newString = newString.Replace("3/7", "ThreeSevenths");
                newString = newString.Replace("4/7", "FourSevenths");
                newString = newString.Replace("5/7", "FiveSevenths");
                newString = newString.Replace("6/7", "SixSevenths");
                newString = newString.Replace("1/8", "OneEighth");
                newString = newString.Replace("3/8", "ThreeEighths");
                newString = newString.Replace("5/8", "FiveEighths");
                newString = newString.Replace("7/8", "Eighths");
                newString = newString.Replace("1/9", "OneNinth");
                newString = newString.Replace("2/9", "TwoNinths");
                newString = newString.Replace("4/9", "FourNinths");
                newString = newString.Replace("5/9", "FiveNinths");
                newString = newString.Replace("7/9", "SevenNinths");
                newString = newString.Replace("8/9", "EightNinths");
                newString = newString.Replace("1/10", "OneTenth");
                newString = newString.Replace("2/10", "TwoTenths");
                newString = newString.Replace("3/10", "ThreeTenths");
                newString = newString.Replace("4/10", "FourTenths");
                newString = newString.Replace("6/10", "SixTenths");
                newString = newString.Replace("7/10", "SevenTenths");
                newString = newString.Replace("8/10", "EightTenths");
                newString = newString.Replace("9/10", "NineTenths");
                newString = newString.Replace("1/11", "OneEleventh");
                newString = newString.Replace("2/11", "TwoElevenths");
                newString = newString.Replace("3/11", "ThreeElevenths");
                newString = newString.Replace("4/11", "FourElevenths");
                newString = newString.Replace("5/11", "FiveElevenths");
                newString = newString.Replace("6/11", "SixElevenths");
                newString = newString.Replace("7/11", "SevenElevenths");
                newString = newString.Replace("8/11", "EightElevenths");
                newString = newString.Replace("9/11", "NineElevenths");
                newString = newString.Replace("10/11", "TenElevenths");
                newString = newString.Replace("1/12", "OneTwelfth");
                newString = newString.Replace("5/12", "FiveTwelfths");
                newString = newString.Replace("7/12", "SevenTwelfths");
                newString = newString.Replace("11/12", "ElevenTwelfths");
            }


            newString = newString.Replace("0", "Zero");
            newString = newString.Replace("1", "One");
            newString = newString.Replace("2", "Two");
            newString = newString.Replace("3", "Three");
            newString = newString.Replace("4", "Four");
            newString = newString.Replace("5", "Five");
            newString = newString.Replace("6", "Six");
            newString = newString.Replace("7", "Seven");
            newString = newString.Replace("8", "Eight");
            newString = newString.Replace("9", "Nine");

            if (!Capitalize)
            {
                newString = newString.ToLower();
            }

            return newString;
        }

        /// <summary>
        /// Splits a Camel or Pascal Cased string into separate words
        /// </summary>
        /// <param name="StringToConvert"></param>
        /// <param name="SplitCharacters">Characters to use for the separator (default is space)</param>
        /// <returns></returns>
        public static string SplitCamelCase(this string StringToConvert, string SplitCharacters = " ")
        {
            string newString = "";

            //newString = System.Text.RegularExpressions.Regex.Replace(StringToConvert, "([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim();

            newString = Regex.Replace(StringToConvert, @"(?<a>(?<!^)[A-Z][a-z])", @" ${a}");
            newString = Regex.Replace(newString, @"(?<a>[a-z])(?<b>[A-Z0-9])", @"${a} ${b}");


            if (SplitCharacters != " ")
            {
                newString = newString.Replace(" ", SplitCharacters);
            }

            return newString;
        }

        /// <summary>
        /// Using a dictionary of replacement keys with their corresponding values,
        /// replace the placeholders in the Template content. 
        /// </summary>
        /// <param name="TemplateContent">The email template content to process.</param>
        /// <param name="PlaceholdersData">The placeholder data Dictionary</param>
        /// <param name="TemplatePattern">The format pattern to indicate placeholders in the template content</param>
        public static string ReplacePlaceholders(this string TemplateContent, Dictionary<string, string> PlaceholdersData, string TemplatePattern = "[{0}]", bool EscapeHtml = false)
        {
            StringBuilder templ = new StringBuilder(TemplateContent);

            foreach (var kv in PlaceholdersData)
            {
                var placeholder = string.Format(TemplatePattern, kv.Key);
                var val = kv.Value;

                if (EscapeHtml)
                {
                    val = WebUtility.HtmlEncode(val);
                }

                templ.Replace(placeholder, val);
            }

            return templ.ToString();
        }

        /// <summary>
        /// Takes a string with multiple words and returns a string with all words capitalized
        /// </summary>
        /// <param name="Original"></param>
        /// <param name="LowercaseAbbreviations">If TRUE will change ALL CAPS words to lowercase before capitalizing (returns 'All Caps Words')</param>
        /// <returns></returns>
        public static string MakeCamelCase(this string Original, bool LowercaseAbbreviations = false)
        {
            var finalString = "";

            var allWords = Original.Split(' ');

            foreach (var word in allWords)
            {
                if (LowercaseAbbreviations)
                {
                    finalString += word.ToLower().Capitalize();
                }
                else
                {
                    finalString += word.Capitalize();
                }

            }

            return finalString;
        }

        /// <summary>
        /// Return a string with the first letter capitalized
        /// </summary>
        /// <param name="Word"></param>
        /// <returns></returns>
        public static string Capitalize(this string Word)
        {
            var finalString = "";

            var allLetters = Word.ToCharArray();
            var isFirstLetter = true;

            foreach (var letter in allLetters)
            {
                if (isFirstLetter)
                {
                    finalString += letter.ToString().ToUpper();
                    isFirstLetter = false;
                }
                else
                {
                    finalString += letter;
                }
            }

            return finalString;
        }

        /// <summary>
        /// Create an Abbreviation from a full string
        /// </summary>
        /// <param name="FullString"></param>
        /// <returns></returns>
        public static string Abbreviate(this string FullString)
        {
            string abbreviation = new string(
                FullString.Split()
                    .Where(s => s.Length > 0 && char.IsLetter(s[0]) && char.IsUpper(s[0]))
                    .Take(3)
                    .Select(s => s[0])
                    .ToArray());

            return abbreviation;
        }

        ///// <summary>
        ///// Split text 
        ///// </summary>
        ///// <param name="TextToSplit"></param>
        ///// <param name="Token"></param>
        ///// <returns></returns>
        //public static string SplitByTokenIfItExists(string TextToSplit, string Token)
        //{
        //    if (!string.IsNullOrEmpty(TextToSplit))
        //    {
        //        if (TextToSplit.IndexOf(Token) > -1)
        //        {
        //            return TextToSplit.Substring(0, TextToSplit.IndexOf(Token));
        //        }
        //        return TextToSplit;
        //    }
        //    else
        //    {
        //        return "";
        //    }

        //}

        /// <summary>
        /// Removes numbers from a string
        /// </summary>
        /// <param name="TextWithNumbers"></param>
        /// <returns></returns>
        public static string StripNumbers(string TextWithNumbers)
        {
            var stripped = TextWithNumbers;

            stripped = new string(TextWithNumbers.Where(c => c != '-' && (c < '0' || c > '9')).ToArray());

            return stripped;
        }

        /// <summary>
        /// Replaces double spaces with single spaces
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string RemoveDoubleSpaces(this string Text)
        {
            string trimmedText = Text.Trim();
            int dblSpcs = trimmedText.CountStringOccurrences("  ");

            do
            {
                trimmedText = trimmedText.Replace("  ", " ");
                dblSpcs = trimmedText.CountStringOccurrences("  ");
            } while (dblSpcs > 0);

            return trimmedText;
        }

        /// <summary>
        /// Is this string a JSON snippet?
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static bool StringIsJson(this string Input)
        {
            Input = Input.Trim();
            if (Input.StartsWith("{") && Input.EndsWith("}"))
                return true;
            if (Input.StartsWith("["))
                return Input.EndsWith("]");
            else
                return false;
        }

        /// <summary>
        /// Encodes the string
        /// </summary>
        /// <param name="OriginalString"></param>
        /// <returns></returns>
        public static string UrlEncode(this string OriginalString)
        {
            return System.Web.HttpUtility.UrlEncode(OriginalString);
        }

        /// <summary>
        /// Decodes the string
        /// </summary>
        /// <param name="EncodedString"></param>
        /// <returns></returns>
        public static string UrlDecode(this string EncodedString)
        {
            return System.Web.HttpUtility.UrlDecode(EncodedString);
        }


        #endregion

        #region Returning a String from a String or Multiple Strings

        //TODO: Convert to use param list for unlimited testing values, like string.concat()
        public static string NoEmptyString(string FirstPreferredString, string SecondString, string ThirdString = "")
        {
            if (FirstPreferredString != null & FirstPreferredString != "")
            { return FirstPreferredString; }
            else if (SecondString != null & SecondString != "")
            { return SecondString; }
            else
            { return ThirdString; }
        }

        public static string HtmlTagContents(string TagName, string TextToSearch)
        {
            string ReturnContent = "";

            Regex TagRegex = new Regex(
                "<(?<tag>\\w*)>(?<text>.*)</\\k<tag>>",
                RegexOptions.IgnoreCase
                | RegexOptions.CultureInvariant
                | RegexOptions.IgnorePatternWhitespace
                | RegexOptions.Compiled
            );

            bool IsMatch = TagRegex.IsMatch(TextToSearch);
            bool Found = false;

            if (IsMatch)
            {
                // Capture all Matches in the InputText
                MatchCollection AllMatches = TagRegex.Matches(TextToSearch);

                for (int i = 0; i < AllMatches.Count; i++)
                {
                    if (!Found)
                    {
                        string Tag = AllMatches[i].Groups["tag"].Value;
                        string Text = AllMatches[i].Groups["text"].Value;

                        if (Tag == TagName)
                        {
                            ReturnContent = Text;
                            Found = true;
                        }
                    }
                }
            }

            return ReturnContent;
        }

        /// <summary>
        /// Converts a phone number in text format to a format which can be used in a 'tel:' link
        /// </summary>
        /// <param name="PhoneData">Original Phone number</param>
        /// <param name="ReturnAllIfNoMatch">If it doesn't match a defined phone number format, should the whole string be returned as-is? (FALSE will return an empty string)</param>
        /// <param name="StripChars">Remove additional characters such as ( ) - and spaces from the string</param>
        /// <returns></returns>
        public static string GetClickablePhoneNumber(string PhoneData, bool ReturnAllIfNoMatch = false, bool StripChars = false)
        {
            var returnString = "";

            //TODO: Enhancement - Add support for international phone numbers
            //Regex stuff to id US phone numbers
            var countrycodes = "1";
            var delimiters = "-|\\.|—|–|&nbsp;";
            var phonedef = "\\+?(?:(?:(?:" + countrycodes + ")(?:\\s|" + delimiters + ")?)?\\(?[2-9]\\d{2}\\)?(?:\\s|"
                           + delimiters + ")?[2-9]\\d{2}(?:" + delimiters + ")?[0-9a-z]{4})";

            var regEx = new Regex(phonedef);

            if (regEx.IsMatch(PhoneData))
            {
                var match = regEx.Match(PhoneData);
                returnString = match.Value;
            }
            else
            {
                if (ReturnAllIfNoMatch)
                {
                    returnString = PhoneData;
                }
            }

            if (StripChars)
            {
                returnString = returnString.Replace("-", "").Replace(".", "").Replace("(", "").Replace(")", "").Replace(" ", "");
            }

            if (!returnString.StartsWith("+"))
            {
                returnString = string.Format("+{0}", returnString);
            }

            return returnString;
        }

        public static string GetDomain(string Url)
        {
            var domain = new Uri(Url).Host;
            return domain;
        }

        /// <summary>
        /// Similar to String.Join(), but allows for 2 different separators in order to provide a natural text representation of a list 
        /// (Example: 'A, B, and C' uses ', ' &amp; ' and ' as the two separators.)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Separator">First separator (ex: ', ')</param>
        /// <param name="SeparatorLast">Second separator (ex: ' and ')</param>
        /// <param name="Values">IEnumerable to provide strings</param>
        /// <returns>String including all values</returns>
        public static string JoinAsText<T>(string Separator, string SeparatorLast, IEnumerable<T> Values)
        {
            if (Values == null)
            {
                return "";
            }

            var values = Values.ToList();

            if (!values.Any())
            {
                return "";
            }

            var total = values.Count();

            if (total == 1)
            {
                return values.First().ToString();
            }
            else if (total == 2)
            {
                return string.Join(SeparatorLast, values);
            }
            else
            {
                var lastItem = values.Last();
                values.Remove(lastItem);

                var joinedString = string.Join(Separator, values);
                joinedString += SeparatorLast + lastItem;

                return joinedString;
            }

        }

        /// <summary>
        /// Truncates a long string to a maximum length without leaving a partial word at the end.
        /// </summary>
        /// <param name="OriginalString">Long string</param>
        /// <param name="MaxLength">Maximum desired length after truncation and Suffix</param>
        /// <param name="Suffix">Appended to end of truncated string (default = ellipse character)</param>
        /// <returns>Truncated string with suffix</returns>
        public static string TruncateAtWord(this string OriginalString, int MaxLength, string Suffix = "…")
        {
            var length = MaxLength - Suffix.Length;

            if (OriginalString == null || OriginalString.Length <= MaxLength)
                return OriginalString;

            int iNextSpace = OriginalString.LastIndexOf(" ", length, StringComparison.Ordinal);
            var shortString = OriginalString.Substring(0, (iNextSpace > 0) ? iNextSpace : length).Trim();
            return $"{shortString}{Suffix}";
        }

        /// <summary>
        /// Appends a random string (a portion of a GUID) to the end of a provided prefix
        /// </summary>
        /// <param name="Prefix">Beginning of string</param>
        /// <param name="RandomPortionLength">Length of the Random string portion (max 32)</param>
        /// <param name="Divider">String to separate the Prefix and Random string</param>
        /// <returns></returns>
        public static string CreateUniqueName(string Prefix, int RandomPortionLength = 10, string Divider = "-")
        {
            var length = Math.Abs(RandomPortionLength) > 32 ? 32 : Math.Abs(RandomPortionLength);
            var startIndex = length <= 10 ? 20 : (length <= 20 ? 10 : 0);
            var guid = Guid.NewGuid();
            var substring = guid.ToString().Replace("-", "").Substring(startIndex, length);

            return $"{Prefix}{Divider}{substring}";
        }


        #endregion

        #region Misc

        /// <summary>
        /// Count occurrences of a string inside another string.
        /// </summary>
        public static int CountStringOccurrences(this string Text, string Pattern)
        {
            // Loop through all instances of the string 'text'.
            int count = 0;
            int i = 0;
            while ((i = Text.IndexOf(Pattern, i)) != -1)
            {
                i += Pattern.Length;
                count++;
            }
            return count;
        }

        #endregion

        #region Converting between String and Objects

        /// <summary>
        /// Splits a string into a List of strings
        /// </summary>
        /// <param name="DelimitedString"></param>
        /// <param name="Separator"></param>
        /// <returns></returns>
        public static List<string> ConvertToList(this string DelimitedString, char Separator)
        {
            List<string> myList = new List<string>(DelimitedString.Split(Separator));
            return myList;
        }

        /// <summary>
        /// Converts an array of strings, each separated with the provided character, into a Dictionary 
        /// </summary>
        /// <param name="StringArray"></param>
        /// <param name="Separator">Character used to separate values in each item, default is an equal sign (=)</param>
        /// <returns></returns>
        public static Dictionary<string, string> ConvertToDictionary(this string[] StringArray, char Separator = '=')
        {
            var dictionary = new Dictionary<string, string>();

            for (int i = 0; i < StringArray.Count(); i++)
            {
                var item = StringArray[i].Split(Separator);
                var key = item[0];
                var val = item[1];
                dictionary.Add(key, val);
            }
            return dictionary;
        }

        /// <summary>
        /// Converts a Dictionary into an array of strings, each separated with the provided character
        /// </summary>
        /// <param name="Dictionary"></param>
        /// <param name="Separator">Character to separate values in each dictionary item, default is an equal sign (=)</param>
        /// <returns></returns>
        public static string[] ConvertToStringArray(this Dictionary<string, string> Dictionary, char Separator = '=')
        {
            var num = Dictionary.Count;
            string[] items = new string[num];

            int iCount = 0;

            foreach (KeyValuePair<string, string> attribute in Dictionary)
            {
                var combined = attribute.Key + Separator + attribute.Value;
                items[iCount] = combined;
                iCount++;
            }

            return items;
        }

        /// <summary>
        /// Takes a collection of string and turns them into a single string using the provided separator
        /// </summary>
        /// <param name="ListOfStrings"></param>
        /// <param name="Separator"></param>
        /// <returns></returns>
        public static string ConvertToSeparatedString(this IEnumerable<string> ListOfStrings, string Separator)
        {
            string myString = Separator;

            foreach (var listItem in ListOfStrings)
            {
                myString += Separator + listItem;
            }

            myString = myString.Replace(String.Concat(Separator, Separator), "");

            return myString;
        }

        /// <summary>
        /// Converts a string in the format "a=b" into a KeyValuePair
        /// </summary>
        /// <param name="SingleKvString">String in the format "a=b"</param>
        /// <returns>A single KeyValuePair object</returns>
        public static KeyValuePair<string, string> ConvertStringToKvPair(string SingleKvString)
        {
            if (SingleKvString.Contains("="))
            {
                var splitKV = SingleKvString.Split('=');
                var kvp = new KeyValuePair<string, string>(splitKV[0], splitKV[1]);
                return kvp;
            }
            else
            {
                //invalid data, return empty KVP
                var kvp = new KeyValuePair<string, string>();
                return kvp;
            }
        }

        /// <summary>
        /// Converts a string in the format "a=b,x=y" or "a=b&x=y" or "a=b|x=y" into an IEnum of KeyValuePairs
        /// </summary>
        /// <param name="KvString">String in the format "a=b,x=y" or "a=b&x=y" or "a=b|x=y"</param>
        /// <returns>Parsed KeyValuePair objects</returns>
        public static IEnumerable<KeyValuePair<string, string>> ParseStringToKvPairs(string KvString)
        {
            var kvPairs = new List<KeyValuePair<string, string>>();

            var countPairs = KvString.CountStringOccurrences("=");

            if (countPairs == 1)
            {
                //Single pair
                var kvp = ConvertStringToKvPair(KvString);
                kvPairs.Add(kvp);
            }
            else if (countPairs > 1)
            {
                if (KvString.Contains("&"))
                {
                    var splitPairs = KvString.Split('&');

                    foreach (var pair in splitPairs)
                    {
                        var kvp = ConvertStringToKvPair(pair);
                        kvPairs.Add(kvp);
                    }
                }
                else if (KvString.Contains(","))
                {
                    var splitPairs = KvString.Split(',');

                    foreach (var pair in splitPairs)
                    {
                        var kvp = ConvertStringToKvPair(pair);
                        kvPairs.Add(kvp);
                    }
                }
                else if (KvString.Contains("|"))
                {
                    var splitPairs = KvString.Split('|');

                    foreach (var pair in splitPairs)
                    {
                        var kvp = ConvertStringToKvPair(pair);
                        kvPairs.Add(kvp);
                    }
                }
            }

            return kvPairs;
        }

        #endregion
    }
}