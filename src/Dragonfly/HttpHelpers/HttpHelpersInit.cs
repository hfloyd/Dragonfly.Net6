using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragonfly.HttpHelpers
{
    public class HttpHelpersInit
    {
        //TODO: Sort this out with DI stuff in an efficient manner - https://stackoverflow.com/questions/31243068/access-the-current-httpcontext-in-asp-net-core

        private static IHttpContextAccessor _accessor;
        private static IWebHostEnvironment _env;
        public static void Configure(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment env)
        {
            _accessor = httpContextAccessor;
            _env= env;
        }

        //public static HttpContext HttpContext => _accessor.HttpContext;

        //public Startup(IConfiguration configuration, IWebHostEnvironment env)
        //{
        //    contentRootPath = env.ContentRootPath;
        //    webRootPath = env.WebRootPath;
        //    projectRootPath = AppContext.BaseDirectory;
        //}

        /// <summary>
        /// Quick way to get an already instantiated Random
        ///
        /// System.Web.HttpContext.Current.Application.Lock();
        /// System.Web.HttpContext.Current.Application["AppRandom"] = new Random();
        /// System.Web.HttpContext.Current.Application.UnLock();
        /// </summary>
        /// <returns></returns>
        //public static Random GetAppRandomizer()
        //{

        //    return (Random)HttpContext.Current.Application["AppRandom"];
        //}


        public void FilesConfigure()
        {
            // setup app's root folders
            AppDomain.CurrentDomain.SetData("ContentRootPath", _env.ContentRootPath);
            AppDomain.CurrentDomain.SetData("WebRootPath", _env.WebRootPath);
        }
    }
}
