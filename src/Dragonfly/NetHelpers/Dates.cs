namespace Dragonfly.NetHelpers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Text;
    using Dragonfly.NetModels;

    public static class Dates
    {
        private const string ThisClassName = "Dragonfly.NetHelpers.Dates";

        enum ConcatDateFormat
        {
            MMDDYYYY,
            YYYYMMDD
        }

        #region Extensions to DateTime

        /// <summary>
        /// Get a text representation of the provided Date relative to Now
        /// </summary>
        /// <param name="Date"></param>
        /// <returns></returns>
        public static string FuzzyDateFormat(this DateTime Date)
        {
            //Based on:http://www.dotnetperls.com/pretty-date

            // Get time span elapsed since the date.
            TimeSpan s = DateTime.Now.Subtract(Date);

            // Get total number of days elapsed.
            int dayDiff = (int)s.TotalDays;

            // Get total number of seconds elapsed.
            int secDiff = (int)s.TotalSeconds;

            // Don't allow out of range values.
            if (dayDiff < 0)
            {
                return "";
            }

            // Handle same-day times.
            if (dayDiff == 0)
            {
                // Less than one minute ago.
                if (secDiff < 60)
                {
                    return "just now";
                }

                // Less than 2 minutes ago.
                if (secDiff < 120)
                {
                    return "1 minute ago";
                }

                // Less than one hour ago.
                if (secDiff < 3600)
                {
                    var minutes = Math.Floor((double)secDiff / 60);
                    return $"{minutes} minutes ago";
                }

                // Less than 2 hours ago.
                if (secDiff < 7200)
                {
                    return "1 hour ago";
                }

                // Less than one day ago.
                if (secDiff < 86400)
                {
                    var hours = Math.Floor((double)secDiff / 3600);
                    return $"{hours} hours ago";
                }
            }

            // Handle previous days.
            if (dayDiff == 1)
            {
                return "yesterday";
            }
            if (dayDiff < 7)
            {
                return $"{dayDiff} days ago";
            }
            if (dayDiff < 31)
            {
                var weeks = Math.Ceiling((double)dayDiff / 7);
                if (weeks == 1)
                {
                    return $"{weeks} week ago";
                }
                else
                {
                    return $"{weeks} weeks ago";
                }
            }
            if (dayDiff < 366)
            {
                var months = Math.Ceiling((double)dayDiff / 30);
                return $"{months} months ago";
            }
            if (dayDiff > 365 & dayDiff < 730)
            {
                return "More than a year ago";
                //return string.Format("{0} years ago",
                //Math.Ceiling((double)dayDiff / 365));
            }
            else
            {
                var years = Math.Ceiling((double)dayDiff / 365);
                return $"More than {years} years ago";
            }

        }

        public static string AgeFormat(this DateTime Date)
        {
            // Get time span elapsed since the date.
            TimeSpan s = DateTime.Now.Subtract(Date);

            // Get total number of days elapsed.
            int dayDiff = (int)s.TotalDays;

            // Get total number of seconds elapsed.
            int secDiff = (int)s.TotalSeconds;

            // Don't allow out of range values.
            if (dayDiff < 0)
            {
                return "";
            }

            // Handle same-day times.
            if (dayDiff == 0)
            {
                // Less than one minute ago.
                if (secDiff < 60)
                {
                    return "1 minute";
                }

                // Less than 2 minutes ago.
                if (secDiff < 120)
                {
                    return "1 minute";
                }

                // Less than one hour ago.
                if (secDiff < 3600)
                {
                    var minutes = Math.Floor((double)secDiff / 60);
                    return $"{minutes} minutes";
                }

                // Less than 2 hours ago.
                if (secDiff < 7200)
                {
                    return "1 hour";
                }

                // Less than one day ago.
                if (secDiff < 86400)
                {
                    var hours = Math.Floor((double)secDiff / 3600);
                    return $"{hours} hours";
                }
            }

            // Handle previous days.
            if (dayDiff == 1)
            {
                return "1 day";
            }
            if (dayDiff < 7)
            {
                return $"{dayDiff} days";
            }
            if (dayDiff < 31)
            {
                var weeks = Math.Ceiling((double)dayDiff / 7);
                if (weeks == 1)
                {
                    return $"{weeks} week";
                }
                else
                {
                    return $"{weeks} weeks";
                }
            }
            if (dayDiff < 366)
            {
                var months = Math.Ceiling((double)dayDiff / 30);
                return $"{months} months";
            }
            if (dayDiff > 365 & dayDiff < 730)
            {
                return "1 year";
            }
            else
            {
                var years = Math.Ceiling((double)dayDiff / 365);
                return $"{years} years";
            }

        }

        /// <summary>
        /// Get a text representation of the provided Date relative to Now
        /// </summary>
        /// <param name="Date"></param>
        /// <returns></returns>
        public static string FuzzyDateFormat(this Nullable<DateTime> Date)
        {
            if (Date != null)
            {
                return ((DateTime)Date).FuzzyDateFormat();
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Get the number of the current quarter (1-4) using calendar quarters
        /// </summary>
        /// <param name="Date"></param>
        /// <returns></returns>
        public static int GetQuarter(this DateTime Date)
        {
            return (Date.Month + 2) / 3;
        }


        #endregion

        #region Functions

        /// <summary>
        /// Given an integer representing the month, will return a formatted version
        /// Ex: FormatMonthNumber(12, "MMM") = "Dec"
        /// NOTE: First day of month and current year are assumed for formatting purposes.
        /// </summary>
        /// <param name="MonthNumber">Number of the month to format</param>
        /// <param name="Format">Format string (ex: MM, MMM, MMMM)</param>
        /// <returns></returns>
        public static string FormatMonthNumber(int MonthNumber, string Format)
        {
            var tempDate = Convert.ToDateTime(String.Concat(MonthNumber, "/1/", DateTime.Now.Year.ToString()));

            return tempDate.ToString(Format);
        }

        /// <summary>
        /// Takes Month, Day, and Year strings and combines them into a specified date format
        /// </summary>
        /// <param name="Month"></param>
        /// <param name="Day"></param>
        /// <param name="Year"></param>
        /// <param name="DateFormat"></param>
        /// <returns></returns>
        public static string ConcatenateDate(string Month, string Day, string Year, string DateFormat = "MM/dd/yyyy")
        {
            string ReturnString = "";
            string DateString = String.Concat(Month.PadLeft(2, '0'), "/", Day.PadLeft(2, '0'), "/", Year);
            DateTime DateTest;
            try
            {
                //BirthdateTest = Convert.ToDateTime(BirthdateString);
                DateTest = DateTime.ParseExact(DateString, "MM/dd/yyyy", null);

                string DateTestString = DateTest.ToString("MM/dd/yyyy");

                if (DateTestString == DateString)
                {
                    ReturnString = DateString;
                }
                else
                {
                    var functionName = string.Format("{0}.ConcatenateDate", ThisClassName);
                    var msg = string.Format("{0} : DateString={1} NOT EQUAL TO DateTest={2} (DateTestString={3})", functionName, DateString, DateTest, DateTestString);
                    //Info.LogInfo(msg);
                    ReturnString = "INVALID";
                }

            }
            catch (Exception ex)
            {
                var functionName = string.Format("{0}.ConcatenateDate", ThisClassName);
                var msg = string.Format("DateString={0}", DateString);
                //Info.LogException(functionName, ex,msg);
                return "INVALID";
            }

            if (ReturnString != "INVALID")
            {
                DateTime FinalDate = DateTime.ParseExact(DateString, "MM/dd/yyyy", null);
                ReturnString = FinalDate.ToString(DateFormat);
            }

            return ReturnString;
        }

        /// <summary>
        /// Formats a range using two dates.
        /// </summary>
        /// <param name="StartDate">Beginning date</param>
        /// <param name="EndDate">Ending date</param>
        /// <param name="FullDateFormat">Format string for full date (including year) (ex: <c>MMM d, yyyy</c>)</param>
        /// <param name="MonthDateFormat">Format string to use when the months are different (ex: <c>MM d</c> , <c>MMM d</c> , <c>MMMM dd</c>)</param>
        /// <param name="DayDateFormat">Format string to use when the days are different (ex: <c>d, yy</c>, <c>d, yyyy</c>)</param>
        /// <param name="PreferredFormat">Preferred format for single-day (Options: <c>y</c>, <c>m</c>, or <c>d</c>)</param>
        /// <param name="RangeDelim">Separator to use for range </param>
        /// <returns>String with both dates formatted and concatenated together</returns>
        public static string FormatDateRange(DateTime StartDate, DateTime EndDate, string FullDateFormat, string MonthDateFormat, string DayDateFormat, string PreferredFormat, string RangeDelim)
        {
            var finalDates = "";

            if (StartDate.Date == EndDate.Date || EndDate == DateTime.MinValue)
            {
                //Dates are the same, return only 1
                switch (PreferredFormat)
                {
                    case "y":
                        finalDates = StartDate.ToString(FullDateFormat);
                        break;

                    case "M":
                        finalDates = StartDate.ToString(MonthDateFormat);
                        break;

                    case "d":
                        finalDates = StartDate.ToString(DayDateFormat);
                        break;

                    default:
                        finalDates = StartDate.ToString(FullDateFormat);
                        break;
                }

            }
            else
            {
                //Dates are different, combine into range
                if (StartDate.Year != EndDate.Year)
                {
                    //Different years
                    //Format with whole dates
                    finalDates = string.Format("{0}{1}{2}", StartDate.ToString(FullDateFormat), RangeDelim, EndDate.ToString(FullDateFormat));
                }
                else
                {
                    //Same year
                    if (StartDate.Month != EndDate.Month)
                    {
                        //Different months
                        var date1 = StartDate.ToString(MonthDateFormat);
                        var date2 = EndDate.ToString(MonthDateFormat);
                        if (PreferredFormat == "y")
                        {
                            date2 = EndDate.ToString(FullDateFormat);
                        }
                        finalDates = string.Format("{0}{1}{2}", date1, RangeDelim, date2);
                    }
                    else
                    {
                        //Same month
                        if (StartDate.Day != EndDate.Day)
                        {
                            //Different days
                            var date1 = StartDate.ToString(MonthDateFormat);
                            var date2 = EndDate.ToString(DayDateFormat);
                            finalDates = string.Format("{0}{1}{2}", date1, RangeDelim, date2);
                        }

                    }
                }
            }
            return finalDates;
        }

        /// <summary>
        /// Formats a range using two dates.
        /// </summary>
        /// <param name="StartDate">Beginning date</param>
        /// <param name="EndDate">Ending date</param>
        /// <param name="FullDateFormat">Format string for full date (including year) (ex: <c>MMM d, yyyy</c>)</param>
        /// <param name="MonthDateFormat">Format string to use when the months are different (ex: <c>MM d</c> , <c>MMM d</c> , <c>MMMM dd</c>)</param>
        /// <param name="DayDateFormat">Format string to use when the days are different (ex: <c>d, yy</c>, <c>d, yyyy</c>)</param>
        /// <param name="PreferredFormat">Preferred format for single-day (Options: <c>y</c>, <c>m</c>, or <c>d</c>)</param>
        /// <param name="RangeDelim">Separator to use for range </param>
        /// <returns>String with both dates formatted and concatenated together</returns>
        /// <param name="TestMode">When set to TRUE will display a variety of test start &amp; end dates so you can see all possible formats using your formatting params.</param>
        /// <returns>String with both dates formatted and concatenated together</returns>
        public static string FormatDateRange(DateTime StartDate, DateTime EndDate, string FullDateFormat,
            string MonthDateFormat, string DayDateFormat, string PreferredFormat, string RangeDelim,
            bool TestMode = false)
        {
            if (TestMode == true)
            {
                var finalOutput = new StringBuilder();
                DateTime start;
                DateTime end;

                //Single Day
                start = new DateTime(DateTime.Now.Year, 3, 8);
                end = new DateTime(DateTime.Now.Year, 3, 8);
                finalOutput.AppendLine("Same Day ... ");
                finalOutput.AppendLine("Start: " + start.ToString("d") + " End: " + end.ToString("d") + " = ");
                finalOutput.AppendLine(FormatDateRange(start, end, FullDateFormat, MonthDateFormat, DayDateFormat,
                    PreferredFormat, RangeDelim));
                finalOutput.AppendLine(" | ");

                //Single Month
                start = new DateTime(DateTime.Now.Year, 6, 10);
                end = new DateTime(DateTime.Now.Year, 6, 16);
                finalOutput.AppendLine("Same Month ... ");
                finalOutput.AppendLine("Start: " + start.ToString("d") + " End: " + end.ToString("d") + " = ");
                finalOutput.AppendLine(FormatDateRange(start, end, FullDateFormat, MonthDateFormat, DayDateFormat,
                    PreferredFormat, RangeDelim));
                finalOutput.AppendLine(" | ");

                //Single Year
                start = new DateTime(DateTime.Now.Year, 9, 27);
                end = new DateTime(DateTime.Now.Year, 10, 6);
                finalOutput.AppendLine("Same Year ... ");
                finalOutput.AppendLine("Start: " + start.ToString("d") + " End: " + end.ToString("d") + " = ");
                finalOutput.AppendLine(FormatDateRange(start, end, FullDateFormat, MonthDateFormat, DayDateFormat,
                    PreferredFormat, RangeDelim));
                finalOutput.AppendLine(" | ");

                //Across Years
                start = new DateTime(DateTime.Now.Year, 12, 30);
                end = new DateTime(DateTime.Now.Year + 1, 1, 2);
                finalOutput.AppendLine("Different Years ... ");
                finalOutput.AppendLine("Start: " + start.ToString("d") + " End: " + end.ToString("d") + " = ");
                finalOutput.AppendLine(FormatDateRange(start, end, FullDateFormat, MonthDateFormat, DayDateFormat,
                    PreferredFormat, RangeDelim));
                finalOutput.AppendLine(" | ");

                return finalOutput.ToString();
            }
            else
            {
                return FormatDateRange(StartDate, EndDate, FullDateFormat, MonthDateFormat, DayDateFormat,
                    PreferredFormat, RangeDelim);
            }
        }

        [Obsolete("Use version with explicit RangeDelim provided")]
        public static string FormatDateRange(DateTime StartDate, DateTime EndDate, string FullDateFormat,
            string MonthDateFormat, string DayDateFormat, string PreferredFormat)
        {
            var RangeDelim = " - ";
            return FormatDateRange(StartDate, EndDate, FullDateFormat, MonthDateFormat, DayDateFormat,
                PreferredFormat, RangeDelim);
        }

        /// <summary>
        /// Tests a string against a provided format for a valid date
        /// </summary>
        /// <param name="DateStringToTest"></param>
        /// <param name="DateFormat"></param>
        /// <returns></returns>
        public static bool IsValidDate(string DateStringToTest, string DateFormat)
        {
            bool ValidDate = false;
            try
            {
                var DateTest = DateTime.ParseExact(DateStringToTest, DateFormat, null);

                if (DateTest != null)
                {
                    ValidDate = true;
                }

            }
            catch (Exception exNonValidDate)
            {
                //Info.LogException("Functions.StringIsValidDate", exNonValidDate, "[DateStringToTest=" + DateStringToTest + "] [DateFormat=" + DateFormat + "] FALSE value returned. No action necessary");
                ValidDate = false;
            }

            return ValidDate;
        }

        #endregion

        #region Date-related Collections

        public static IEnumerable<CalendarQuarter> GetPreviousQuarters(int NumberToReturn, DateTime Date,
            bool IncludeCurrent = false)
        {
            var qtrs = new List<CalendarQuarter>();

            CalendarQuarter quarter;
            //var lastQuarterYear = 0;

            var currentQuarter = new CalendarQuarter(Date);

            if (IncludeCurrent)
            {
                quarter = currentQuarter;
                //qtrs.Add(currentQuarter);
                //numToGet = numToGet - 1;
            }
            else
            {
                quarter = currentQuarter.GetPriorQuarter();
            }

            qtrs.Add(quarter);
            var numToGet = NumberToReturn - 1;

            for (int i = 0; i < numToGet; i++)
            {
                quarter = quarter.GetPriorQuarter();
                qtrs.Add(quarter);
            }

            return qtrs;
        }

        /// <summary>
        /// Get list of Quarters between two dates
        /// </summary>
        /// <remarks>WARNING: NEEDS TESTING</remarks>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static IEnumerable<CalendarQuarter> GetAllQuarters(DateTime StartDate, DateTime EndDate)
        {
            var qtrs = new List<CalendarQuarter>();

            var firstQuarter = new CalendarQuarter(StartDate);

            var lastQuarter = new CalendarQuarter(EndDate);

            var startDate = firstQuarter.StartDate;

            qtrs.Add(lastQuarter);

            CalendarQuarter quarter;
            quarter = lastQuarter.GetPriorQuarter();
            qtrs.Add(quarter);
            do
            {
                quarter = quarter.GetPriorQuarter();
                qtrs.Add(quarter);
            } while (quarter.StartDate > startDate);

            qtrs.Add(firstQuarter);

            return qtrs;
        }


        #endregion

    }
}
