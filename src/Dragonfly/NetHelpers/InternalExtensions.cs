﻿namespace Dragonfly.NetHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using System.IO;
    using Microsoft.AspNetCore.Hosting;
  

    internal static class InternalExtensions
    {

        /// <summary>
        ///     Maps a virtual path to a physical path to the application's web root
        /// </summary>
        /// <remarks>
        ///     Depending on the runtime 'web root', this result can vary. For example in Net Framework the web root and the
        ///     content root are the same, however
        ///     in netcore the web root is /wwwroot therefore this will Map to a physical path within wwwroot.
        /// </remarks>
        //Copied from https://raw.githubusercontent.com/umbraco/Umbraco-CMS/9326cc5fc64a85e8e7fc19ae7faa726e33c33480/src/Umbraco.Web.Common/Extensions/WebHostEnvironmentExtensions.cs
        public static string MapPathWebRoot(this IWebHostEnvironment webHostEnvironment, string path)
        {
            var root = webHostEnvironment.WebRootPath;

            // Create if missing
            if (string.IsNullOrWhiteSpace(root))
            {
                root = webHostEnvironment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }

            var newPath = path.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);

            // TODO: This is a temporary error because we switched from IOHelper.MapPath to HostingEnvironment.MapPathXXX
            // IOHelper would check if the path passed in started with the root, and not prepend the root again if it did,
            // however if you are requesting a path be mapped, it should always assume the path is relative to the root, not
            // absolute in the file system.  This error will help us find and fix improper uses, and should be removed once
            // all those uses have been found and fixed
            if (newPath.StartsWith(root))
            {
                throw new ArgumentException(
                    "The path appears to already be fully qualified.  Please remove the call to MapPathWebRoot");
            }

            return Path.Combine(root, newPath.TrimStart(TildeForwardSlashBackSlash));
        }

        /// <summary>
        ///     Maps a virtual path to a physical path to the application's web root
        /// </summary>
        /// <remarks>
        ///     Depending on the runtime 'web root', this result can vary. For example in Net Framework the web root and the
        ///     content root are the same, however
        ///     in netcore the web root is /wwwroot therefore this will Map to a physical path within wwwroot.
        /// </remarks>
        //Copied from https://raw.githubusercontent.com/umbraco/Umbraco-CMS/9326cc5fc64a85e8e7fc19ae7faa726e33c33480/src/Umbraco.Web.Common/Extensions/WebHostEnvironmentExtensions.cs
        public static string MapPathContentRoot(this IWebHostEnvironment webHostEnvironment, string path)
        {
	        var root = webHostEnvironment.ContentRootPath;

	        // Create if missing
	        if (string.IsNullOrWhiteSpace(root))
	        {
		        root = Directory.GetCurrentDirectory();
	        }

	        var newPath = path.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);

	        // TODO: This is a temporary error because we switched from IOHelper.MapPath to HostingEnvironment.MapPathXXX
	        // IOHelper would check if the path passed in started with the root, and not prepend the root again if it did,
	        // however if you are requesting a path be mapped, it should always assume the path is relative to the root, not
	        // absolute in the file system.  This error will help us find and fix improper uses, and should be removed once
	        // all those uses have been found and fixed
	        if (newPath.StartsWith(root))
	        {
		        throw new ArgumentException(
			        "The path appears to already be fully qualified.  Please remove the call to MapPathContentRoot");
	        }

	        return Path.Combine(root, newPath.TrimStart(TildeForwardSlashBackSlash));
        }


        /// <summary>
        ///     Char array containing ~ / \
        /// </summary>
        internal static readonly char[] TildeForwardSlashBackSlash = { '~', '/', '\\' };
    }
}
