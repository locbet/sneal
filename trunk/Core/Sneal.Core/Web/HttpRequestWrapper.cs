using System;
using System.Collections.Specialized;
using System.Web;

namespace Sneal.Core.Web
{
    /// <summary>
    /// Wraps a native ASP.NET request.
    /// </summary>
    public class HttpRequestWrapper : IHttpRequest
    {
        private readonly HttpRequest _request;

        /// <summary>
        /// Constructs a new HttpRequestWrapper that adapts the ASP.NET request
        /// from the current http context.
        /// </summary>
        public HttpRequestWrapper()
        {
            _request = HttpContext.Current.Request;
        }

        /// <summary>
        /// Constructs a new HttpRequestWrapper that adapts the specified ASP.NET request.
        /// </summary>
        /// <param name="request">An ASP.NET request</param>
        public HttpRequestWrapper(HttpRequest request)
        {
            _request = request;
        }

        /// <summary>
        /// The wrapped ASP.NET request.
        /// </summary>
        public HttpRequest Request
        {
            get { return _request; }
        }

        public NameValueCollection Params
        {
            get { return _request.Params; }
            set { throw new NotSupportedException(); }
        }

        public NameValueCollection Headers
        {
            get { return _request.Headers; }
            set { throw new NotSupportedException(); }
        }

        public NameValueCollection QueryString
        {
            get { return _request.QueryString; }
            set { throw new NotSupportedException(); }
        }

        public NameValueCollection Form
        {
            get { return _request.Form; }
            set { throw new NotSupportedException(); }
        }

        public Uri Url
        {
            get { return _request.Url; }
            set { throw new NotSupportedException(); }
        }

        public string ApplicationPath
        {
            get { return _request.ApplicationPath; }
            set { throw new NotSupportedException(); }
        }

        public string PhysicalApplicationPath
        {
            get { return _request.PhysicalApplicationPath; }
            set { throw new NotSupportedException(); }
        }

        public bool IsLocal
        {
            get { return _request.IsLocal; }
            set { throw new NotSupportedException(); }
        }

        public string MapPath(string virtualPath)
        {
            return _request.MapPath(virtualPath);
        }
    }
}
