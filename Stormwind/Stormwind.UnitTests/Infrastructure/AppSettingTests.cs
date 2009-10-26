using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Stormwind.Infrastructure;

namespace Stormwind.UnitTests.Infrastructure
{
    [TestFixture]
    public class AppSettingTests
    {
        [Test]
        public void RootContentPath_should_default_to_Content_folder_under_root_of_website()
        {
            new AppSettings().RootContentPath.Should().Be.EqualTo("~/Content");
        }
    }
}
