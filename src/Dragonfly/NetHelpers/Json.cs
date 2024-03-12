namespace Dragonfly.NetHelpers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Script.Serialization;

    public static class Json
    {
        private const string ThisClassName = "Dragonfly.NetHelpers.Json";

        public static T FromJson<T>(this string s)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            return ser.Deserialize<T>(s);
        }

        public static string ToJson(this object obj)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            return ser.Serialize(obj);
        }
    }
}
