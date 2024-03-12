namespace Dragonfly.NetModels
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    public class PagerModel
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="PagerBaseUrl">Base Url without 'page' query string key</param>
        /// <param name="ItemsPerPage">Total number of items on each page</param>
        /// <param name="ThisPageNum">Current active page</param>
        /// <param name="TotalNumPages">Total number of pages</param>
        /// <param name="PagerQueryStringKey">Keyword used to represent page number in query string (Default="p")</param>
        public PagerModel(string PagerBaseUrl, int ItemsPerPage, int ThisPageNum, int TotalNumPages,
            string PagerQueryStringKey = "p")
        {
            PageSize = ItemsPerPage;
            CurrentPageNum = ThisPageNum;
            TotalPages = TotalNumPages;
            QueryStringKey = PagerQueryStringKey;

            if (PagerBaseUrl.Contains("?") && !PagerBaseUrl.EndsWith("?"))
            {
                //Contained somewhere in the middle, so there is existing query string data, append page info
                BaseUrlWithQS = string.Format("{0}{1}=", PagerBaseUrl.EnsureEndsWith('&'), PagerQueryStringKey);
            }
            else
            {
                BaseUrlWithQS = string.Format("{0}{1}=", PagerBaseUrl.EnsureEndsWith('?'), PagerQueryStringKey);
            }

            FirstUrl = PagerBaseUrl;
            LastUrl = string.Format("{0}{1}", BaseUrlWithQS, TotalPages);

            var nextPageUrl = TotalPages > ThisPageNum
                ? string.Format("{0}{1}", BaseUrlWithQS, (ThisPageNum + 1))
                : null;
            var prevPageUrl = ThisPageNum > 2 ? string.Format("{0}{1}", BaseUrlWithQS, (ThisPageNum - 1)) :
                ThisPageNum > 1 ? FirstUrl : null;

            NextUrl = nextPageUrl;
            PreviousUrl = prevPageUrl;
        }

        public int PageSize { get; private set; }

        public int TotalPages { get; private set; }

        public int CurrentPageNum { get; private set; }

        public string NextUrl { get; private set; }

        public string PreviousUrl { get; private set; }

        public string FirstUrl { get; private set; }

        public string QueryStringKey { get; private set; }

        public string BaseUrlWithQS { get; private set; }

        public string LastUrl { get; private set; }

        public bool HasNext
        {
            get { return NextUrl != null; }
        }

        public bool HasPrevious
        {
            get { return PreviousUrl != null; }
        }

        public IEnumerable<PagerPageLink> GetPageNumberLinks(int MaxNumberLinks = 0, string MorePreviousDisplayText = "...", string MoreNextDisplayText = "...")
        {
            var links = new List<PagerPageLink>();

            if (MaxNumberLinks == 0 || this.TotalPages <= MaxNumberLinks)
            {
                //Return all
                for (int i = 1; i < this.TotalPages + 1; i++)
                {
                    links.Add(new PagerPageLink(i, i.ToString()));
                }
            }
            else
            {
                //Return only the current bunch for this page
                var centerPosition = Convert.ToInt32(Math.Ceiling((decimal)MaxNumberLinks / 2));
                var pagesBeforeAndAfter = centerPosition - 1;

                var isLastSet = (this.TotalPages - this.CurrentPageNum) <= MaxNumberLinks;
                var isFirstSet = !isLastSet && (this.CurrentPageNum > centerPosition);
                var centerCurrentPage = !isFirstSet && !isLastSet;

                var renderPrev = this.CurrentPageNum >= MaxNumberLinks || centerCurrentPage;
                var renderNext = centerCurrentPage && (this.CurrentPageNum <= (this.TotalPages - centerPosition));

                if (renderPrev)
                {
                    var prevLnk = new PagerPageLink(this.CurrentPageNum - 1, MorePreviousDisplayText);
                    prevLnk.IsPrevious = true;
                    links.Add(prevLnk);
                }

                if (isFirstSet)
                {
                    //Render first set
                    renderNext = true;
                    var firstPageNum = 1;
                    var lastPageNum = MaxNumberLinks - (renderNext ? 1 : 0);
                    for (int i = firstPageNum; i <= lastPageNum; i++)
                    {
                        links.Add(new PagerPageLink(i, i.ToString()));
                    }
                }
                else if (isLastSet)
                {
                    //Render last set
                    var firstPageNum = (this.TotalPages - MaxNumberLinks) +1 + (renderPrev ? 1 : 0);
                    var lastPageNum = this.TotalPages;
                    for (int i = firstPageNum; i <= lastPageNum; i++)
                    {
                        links.Add(new PagerPageLink(i, i.ToString()));
                    }
                }
                else
                {
                    var firstPageNum = this.CurrentPageNum - pagesBeforeAndAfter + (renderPrev ? 1 : 0);
                    var lastPageNum = this.CurrentPageNum + pagesBeforeAndAfter - (renderNext ? 1 : 0);

                    for (int i = firstPageNum; i <= lastPageNum; i++)
                    {
                        if (i > 0 && i <= lastPageNum && i <= this.TotalPages)
                        {
                            {
                                //i is still inside total range of pages, so page can be added
                                links.Add(new PagerPageLink(i, i.ToString()));
                            }
                        }

                    }

                }

                if (renderNext)
                {
                    var nextLnk = new PagerPageLink(this.CurrentPageNum + 1, MoreNextDisplayText);
                    nextLnk.IsNext = true;
                    links.Add(nextLnk);
                }
            }

            return links;
        }
    }

    /// <summary>
    /// List of link data for pager pages
    /// </summary>
    public class PagerPageLink
    {
        public int PageNum { get; set; }
        public string DisplayString { get; set; }
        public bool IsPrevious { get; set; }
        public bool IsNext { get; set; }

        public PagerPageLink(int PageNum, string DisplayString)
        {
            this.PageNum = PageNum;
            this.DisplayString = DisplayString;
        }
    }

    internal static class StringExtensions
    {
        public static string EnsureStartsWith(this string input, char value)
        {
            return input.StartsWith(value.ToString(CultureInfo.InvariantCulture)) ? input : value + input;
        }

        public static string EnsureEndsWith(this string input, char value)
        {
            return input.EndsWith(value.ToString(CultureInfo.InvariantCulture)) ? input : input + value;
        }

        public static string EnsureEndsWith(this string input, string toEndWith)
        {
            return input.EndsWith(toEndWith.ToString(CultureInfo.InvariantCulture)) ? input : input + toEndWith;
        }
    }
}