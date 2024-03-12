namespace Dragonfly.NetModels
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A Model which can be used to group a list of any object type into Pages of a certain size
    /// </summary>
    /// <typeparam name="T">Object Model</typeparam>
    public class PagingCollection<T> : IEnumerable<T>
    {
        private const string ThisClassName = "Dragonfly.NetModels.PagingCollection<T>";

        #region fields

        private const int DefaultPageSize = 10;

        private readonly IEnumerable<T> _collection;

        private int _pageSize = DefaultPageSize;

        private List<CollectionPage<T>> _pages = new List<CollectionPage<T>>();
        private int _totalItems;

        #endregion

        #region properties

        /// <summary>
        /// Gets or sets page size
        /// </summary>
        public int PageSize
        {
            get
            {
                return this._pageSize;
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException();
                }
                this._pageSize = value;
                UpdatePages();
            }
        }

        /// <summary>
        /// Gets pages count
        /// </summary>
        public int PagesCount
        {
            get
            {
                return (int)Math.Ceiling(this._collection.Count() / (decimal)this.PageSize);
            }
        }

        /// <summary>
        /// Get All Pages
        /// </summary>
        public IEnumerable<CollectionPage<T>> Pages
        {
            get
            {
                return this._pages;
            }
        }

        public int TotalItems
        {
            get => _totalItems;
           
        }

        #endregion

        #region ctor

        /// <summary>
        /// Creates paging collection and sets page size
        /// </summary>
        public PagingCollection(IEnumerable<T> Collection, int PageSize)
        {
            if (Collection == null)
            {
                throw new ArgumentNullException("Collection");
            }

            this._totalItems = Collection.Count();
            this._pageSize = PageSize;
            this._collection = Collection.ToArray();
            UpdatePages();

        }

        /// <summary>
        /// Creates paging collection
        /// </summary>
        public PagingCollection(IEnumerable<T> Collection) : this(Collection, DefaultPageSize)
        {
            if (Collection == null)
            {
                throw new ArgumentNullException("Collection");
            }
            this._pageSize = DefaultPageSize;
            this._collection = Collection.ToArray();
            UpdatePages();
        }

        #endregion

        #region public methods

        /// <summary>
        /// Returns the Page using the specified page number
        /// </summary>
        /// <param name="PageNumber"></param>
        /// <returns></returns>
        public CollectionPage<T> GetPage(int PageNumber, bool SupressError)
        {
            if (SupressError && PageNumber > _pages.Count)
            {
                return null;
            }
            else
            {
                var pageIndex = PageNumber - 1;
                return _pages[pageIndex];
            }
        }

        /// <summary>
        /// Returns data by page number
        /// </summary>
        [Obsolete("Use GetPage().Collection instead")]
        public IEnumerable<T> GetData(int PageNumber)
        {
            return GetDataFromCollection(PageNumber);
        }

        /// <summary>
        /// Returns number of items on page by number
        /// </summary>
        public int GetCount(int pageNumber)
        {
            return this.GetPage(pageNumber,false).ResultsOnPage;
        }

        #endregion

        #region static methods

        /// <summary>
        /// Returns data by page number and page size
        /// </summary>
        public static IEnumerable<T> GetPaging(IEnumerable<T> collection, int pageNumber, int pageSize)
        {
            return new PagingCollection<T>(collection, pageSize).GetData(pageNumber);
        }

        /// <summary>
        /// Returns data by page number
        /// </summary>
        public static IEnumerable<T> GetPaging(IEnumerable<T> collection, int pageNumber)
        {
            return new PagingCollection<T>(collection, DefaultPageSize).GetData(pageNumber);
        }

        #endregion

        #region IEnumerable<T> Members

        /// <summary>
        /// Returns an enumerator that iterates through collection
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            return this._collection.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator that iterates through collection
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region private functions

        private void UpdatePages()
        {
            //divide results into pages
            var totalItems = this._collection.Count();
            //var numPages = totalResults % pageLength == 0 ? totalResults / pageLength : totalResults / pageLength + 1;
            var pagesList = new List<CollectionPage<T>>();
            if (this._pageSize > 0)
            {
                for (int i = 0; i < this.PagesCount; i++)
                {
                    var page = new CollectionPage<T>();
                    var toSkip = i * this._pageSize;

                    page.PageNumber = i + 1;
                    page.Collection = GetDataFromCollection(page.PageNumber);
                    page.ResultsOnPage = page.Collection.Count();
                    page.FirstResult = toSkip + 1;

                    var lastResult = toSkip + this._pageSize;
                    if (lastResult > totalItems)
                    {
                        lastResult = totalItems;
                    }

                    page.LastResult = lastResult;

                    pagesList.Add(page);
                }
            }
            
            //for (int i = 1; i <= this.PagesCount; i++)
            //{
            //    var newPage = new CollectionPage<T>();
            //    newPage.PageNumber = i;
            //    newPage.Collection = this.GetData(i);

            //    pagesList.Add(newPage);
            //}

            this._pages = pagesList;
        }

        private IEnumerable<T> GetDataFromCollection(int pageNumber)
        {
            if (pageNumber < 0 || pageNumber > this.PagesCount)
            {
                return new T[] { };
            }

            int offset = (pageNumber - 1) * this.PageSize;

            return this._collection.Skip(offset).Take(this.PageSize);
        }
        #endregion
    }

    public class CollectionPage<T>
    {
        public int PageNumber { get; set; }
        public int ResultsOnPage { get; set; }
        public int FirstResult { get; set; }
        public int LastResult { get; set; }
        public IEnumerable<T> Collection;
    }
}