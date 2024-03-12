namespace Dragonfly.NetHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using Microsoft.AspNetCore.Http;

    public static class Extensions
    {
        /// <summary>
        /// Used for smoother upgrade of .Net Framework code
        /// </summary>
        /// <param name="ContextRequest"></param>
        /// <returns>String of URL</returns>
        [Obsolete("Consider updating to use 'Dragonfly.NetHelpers.Urls.CurrentRequestUri()' directly.")]
        public static string Url(this HttpRequest ContextRequest)
        {
            return Dragonfly.NetHelpers.Urls.CurrentRequestUri(ContextRequest) != null ? Dragonfly.NetHelpers.Urls.CurrentRequestUri(ContextRequest).AbsoluteUri : "";
        }


        #region IEnumerable<string> Extensions

        /// <summary>
        /// Provides the same functionality as string.Join as an extension method
        /// </summary>
        /// <param name="ListOfStrings"></param>
        /// <param name="Delimiter"></param>
        /// <returns></returns>
		public static string ToDelimitedString(this IEnumerable<string> ListOfStrings, string Delimiter)
		{
			return string.Join(Delimiter, ListOfStrings);
		}
        #endregion

        #region ======= IEnumerable<T> 

		/// <summary>
		/// Find the index of an item in the IEnumerable collection similar to List&lt;T&gt;.FindIndex()
		/// </summary>
		/// <param name="finder">The Item to locate</param>
		/// <returns>Integer representing the Index position</returns>
		public static int FindIndex<T>(this IEnumerable<T> list, Predicate<T> finder)
        {
            return list.ToList().FindIndex(finder);
        }
        #endregion

        #region ======= Dictionary 
        public static NameValueCollection ToNameValueCollection(this Dictionary<string, string> Dict)
        {
            var nvc = new NameValueCollection();
            foreach (var item in Dict)
            {
                nvc.Add(item.Key, item.Value);
            }

            return nvc;
        }

        #endregion
    }
}