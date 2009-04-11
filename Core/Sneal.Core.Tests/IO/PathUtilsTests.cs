using NUnit.Framework;
using Sneal.Core.IO;

namespace Sneal.Core.Tests.IO
{
    [TestFixture]
    public class PathUtilsTests
    {
        [Test]
        public void Can_combine_two_drive_paths()
        {
            Assert.AreEqual(@"d:\dir1\dir2\", PathUtils.Combine(@"d:\dir1\", @"\dir2\"));
        }

        [Test]
        public void Can_make_http_path_http()
        {
            Assert.AreEqual("http://mysource/dir1", PathUtils.MakePathHttp("http://mysource/dir1"));
        }

        [Test]
        public void Can_make_http_path_with_port_http()
        {
            Assert.AreEqual("http://mysource:80/dir1", PathUtils.MakePathHttp("http://mysource:80/dir1"));
        }

        [Test]
        public void Can_make_local_drive_path_http()
        {
            Assert.AreEqual("http://mysource/dir1", PathUtils.MakePathHttp(@"d:\mysource\dir1"));
        }

        [Test]
        public void Can_make_relative_path_http()
        {
            Assert.AreEqual("http://mysource/dir1", PathUtils.MakePathHttp(@"\mysource\dir1"));
        }

        [Test]
        public void CanNormalizeExtendedPath()
        {
            Assert.AreEqual(@"D:\source\subdir1\subdir4\",
                            PathUtils.NormalizePath(@"D:\source\subdir1\subdir2\subdir3\..\..\subdir4\"));
        }

        [Test]
        public void CanNormalizeSinglePath()
        {
            Assert.AreEqual(@"D:\source\subdir\", PathUtils.NormalizePath(@"D:\source\dir1\..\subdir\"));
        }

        [Test]
        public void Should_combine_relative_http_path()
        {
            Assert.AreEqual("http://localhost:90/dir1/dir2/dir3",
                            PathUtils.Combine("http://localhost:90/dir1/", "/dir2/dir3"));
        }

        [Test]
        public void Should_combine_two_relative_http_paths()
        {
            Assert.AreEqual("/dir1/dir2", PathUtils.Combine("/dir1/", "dir2"));
        }

        [Test]
        public void Should_make_path_relative()
        {
            Assert.AreEqual(@"dir1\dir2",
                            PathUtils.MakePathRelative(@"c:\src\myproj\", @"c:\src\myproj\dir1\dir2"));
        }

        [Test]
        public void Should_make_path_relative_when_not_normalized()
        {
            Assert.AreEqual(@"dir2",
                            PathUtils.MakePathRelative(@"c:\src\myproj\", @"c:\src\myproj\dir1\..\dir2"));
        }
    }
}