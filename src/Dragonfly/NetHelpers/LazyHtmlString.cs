namespace Dragonfly.NetHelpers
{
    using System;
    using System.Web;

    public class LazyHtmlString : HtmlString
    {
        private const string ThisClassName = "Dragonfly.NetHelpers.LazyHtmlString";

        Lazy<string> _controlRenderer;

        public LazyHtmlString(Func<string> getString)
        {
            this._controlRenderer = new Lazy<string>(getString);
        }

        public string Html
        {
            get
            {
                return this._controlRenderer.Value;
            }
        }

        public override string ToString()
        {
            return this.Html;
        }

        public string ToHtmlString()
        {
            return this.Html;
        }
    }
}