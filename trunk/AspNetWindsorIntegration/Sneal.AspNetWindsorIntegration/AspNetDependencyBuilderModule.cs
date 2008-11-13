using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using Castle.Windsor;

namespace Sneal.AspNetWindsorIntegration
{
    /// <summary>
    /// Intercepts page invocations and injects any dependencies the page
    /// and it's child controls need to execute from the global application
    /// Windsor container.
    /// </summary>
    public class AspNetDependencyBuilderModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.PreRequestHandlerExecute += PreRequestHandlerExecute;
        }
        
        /// <summary>
        /// Runs just before page processing, injecting any dependencies the
        /// page may have.
        /// </summary>
        private void PreRequestHandlerExecute(object sender, EventArgs e)
        {
            IHttpHandler handler = HttpContext.Current.Handler;
            
            Page page = handler as Page;
            if (page == null)
            {
                return;
            }

            page.InitComplete += PageInitComplete;
            DependencyBuilder.BuildUp(page);
        }

        /// <summary>
        /// Runs just after the page has completed initializing injecting
        /// any dependencies the page's controls may have.
        /// </summary>
        private void PageInitComplete(object sender, EventArgs e)
        {
            Page page = (Page)sender;
            foreach (Control pageControl in GetControlTree(page))
            {
                DependencyBuilder.BuildUp(pageControl);
            }
        }

        /// <summary>
        /// Get the controls in the page's control tree excluding the page itself.
        /// </summary>
        /// <param name="root">The current control instance</param>
        /// <returns>The direct children of this control</returns>
        private static IEnumerable<Control> GetControlTree(Control root)
        {
            foreach (Control child in root.Controls)
            {
                yield return child;
                foreach (Control c in GetControlTree(child))
                {
                    yield return c;
                }
            }
        }

        protected virtual AspNetDependencyBuilder DependencyBuilder
        {
            get
            {
                return new AspNetDependencyBuilder(Container);
            }
        }

        /// <summary>
        /// Returns a reference to the global application Windsor container.
        /// </summary>
        protected virtual IWindsorContainer Container
        {
            get
            {
                var containerAccessor = HttpContext.Current.ApplicationInstance as IContainerAccessor;
                if (containerAccessor == null)
                {
                    throw new InvalidOperationException(string.Format(
                        "Your Global.asax application must implement {0}",
                        typeof(IContainerAccessor)));
                }

                if (containerAccessor.Container == null)
                {
                    throw new InvalidOperationException(string.Format(
                        "Your Global.asax application must provide a non-null {0} instance",
                        typeof(IWindsorContainer)));
                }

                return containerAccessor.Container;
            }
        }

        public void Dispose()
        {
            // no-op
        }
    }
}
