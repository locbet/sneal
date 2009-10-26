using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Stormwind.Extensions;
using NUnit.Framework;

namespace Stormwind.UnitTests.Extensions
{
    [TestFixture]
    public class UrlHelperExtensionsTests
    {
        [Test]
        public void Script_should_return_root_content_folder_prepended()
        {
            // How do we mock RequestContext?
            // Or maybe we can move the extension method functionality elsewhere?
            //new UrlHelper(new System.Web.Routing.RequestContext()).Script("my.js");
        }
    }
}
