#region license
// Copyright 2008 Shawn Neal (sneal@sneal.net)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

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
            if (!PageUsesInjection(handler))
            {
                return;
            }

            Page page = handler as Page;
            if (page != null)
                page.InitComplete += PageInitComplete;
            DependencyBuilder.BuildUp(handler);
        }

        private static bool PageUsesInjection(IHttpHandler handler)
        {
            if (handler == null)
            {
                return false;
            }

            var attributes = handler.GetType().GetCustomAttributes(typeof(UsesInjectionAttribute), true);
            return attributes.Length > 0;
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
