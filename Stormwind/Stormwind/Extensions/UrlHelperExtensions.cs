using System;
using System.Web.Mvc;
using Microsoft.Practices.ServiceLocation;
using Stormwind.Infrastructure;

namespace Stormwind.Extensions
{
    /// <summary>
    /// MVC Url extension methods.
    /// </summary>
    public static class UrlHelperExtensions
    {
        /// <summary>
        /// Gets a full URL to the specified image file name.
        /// </summary>
        public static string Image(this UrlHelper helper, string fileName)
        {
            return GetContentPath(helper, "Images", fileName);
        }

        /// <summary>
        /// Gets a full URL to the specified stylesheet file name.
        /// </summary>
        public static string Stylesheet(this UrlHelper helper, string fileName)
        {
            return GetContentPath(helper, "Stylesheets", fileName);
        }

        /// <summary>
        /// Gets a full URL to the specified script file name.
        /// </summary>
        public static string Script(this UrlHelper helper, string fileName)
        {
            return GetContentPath(helper, "Scripts", fileName);
        }

        private static string GetContentPath(this UrlHelper helper, string subDir, string fileName)
        {
            return helper.Content("{Content}/{SubDirectory}/{FileName}".FormatWith(
                new { Content = RootContentPath, SubDirectory = subDir, FileName = fileName }));
        }

        private static string RootContentPath
        {
            get { return ServiceLocator.Current.GetInstance<AppSettings>().RootContentPath; }
        }
    } 
}