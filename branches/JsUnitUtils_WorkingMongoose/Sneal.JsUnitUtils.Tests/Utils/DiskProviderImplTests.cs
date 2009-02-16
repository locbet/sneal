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
using NUnit.Framework;
using Sneal.JsUnitUtils.Utils;

namespace Sneal.JsUnitUtils.Tests.Utils
{
    [TestFixture]
    public class DiskProviderImplTests
    {
        DiskProviderImpl Path = new DiskProviderImpl();

        [Test]
        public void Can_combine_two_drive_paths()
        {
            Assert.AreEqual(@"d:\dir1\dir2\", Path.Combine(@"d:\dir1\", @"\dir2\"));
        }

        [Test]
        public void Should_combine_two_relative_http_paths()
        {
            Assert.AreEqual("/dir1/dir2", Path.Combine("/dir1/", "dir2"));
        }

        [Test]
        public void Should_combine_relative_http_path()
        {
            Assert.AreEqual("http://localhost:90/dir1/dir2/dir3",
                            Path.Combine("http://localhost:90/dir1/", "/dir2/dir3"));
        }

        [Test]
        public void Can_make_local_drive_path_http()
        {
            Assert.AreEqual("http://mysource/dir1", Path.MakeHttp(@"d:\mysource\dir1"));
        }

        [Test]
        public void Can_make_relative_path_http()
        {
            Assert.AreEqual("http://mysource/dir1", Path.MakeHttp(@"\mysource\dir1"));
        }

        [Test]
        public void Can_make_http_path_http()
        {
            Assert.AreEqual("http://mysource/dir1", Path.MakeHttp("http://mysource/dir1"));
        }

        [Test]
        public void Can_make_http_path_with_port_http()
        {
            Assert.AreEqual("http://mysource:80/dir1", Path.MakeHttp("http://mysource:80/dir1"));
        }

        [Test]
        public void CanNormalizeSinglePath()
        {
            Assert.AreEqual(@"D:\source\subdir\", Path.NormalizePath(@"D:\source\dir1\..\subdir\"));
        }

        [Test]
        public void CanNormalizeExtendedPath()
        {
            Assert.AreEqual(@"D:\source\subdir1\subdir4\", Path.NormalizePath(@"D:\source\subdir1\subdir2\subdir3\..\..\subdir4\"));
        }

        [Test]
        public void Can_find_file_in_child_dir()
        {
            string startFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            startFolderPath = Path.Combine(startFolderPath, GetType().ToString());

            System.IO.Directory.CreateDirectory(startFolderPath);
            System.IO.Directory.CreateDirectory(Path.Combine(startFolderPath, "subdir1"));
            System.IO.Directory.CreateDirectory(Path.Combine(startFolderPath, "subdir1\\subsubdir1"));
            System.IO.Directory.CreateDirectory(Path.Combine(startFolderPath, "subdir1\\subsubdir2"));

            string filePath = Path.Combine(startFolderPath, "subdir1\\subsubdir2\\testfile.txt");
            using (System.IO.StreamWriter writer = System.IO.File.CreateText(filePath))
            {
                writer.WriteLine("hi!");
            }

            string foundPath = Path.FindFile(startFolderPath, "testfile.txt");

            Assert.AreEqual(filePath, foundPath);
        }

        [Test]
        public void Should_make_path_relative()
        {
            Assert.AreEqual(@"dir1\dir2",
                Path.MakePathRelative(@"c:\src\myproj\", @"c:\src\myproj\dir1\dir2"));
        }

        [Test]
        public void Should_make_path_relative_when_not_normalized()
        {
            Assert.AreEqual(@"dir2",
                Path.MakePathRelative(@"c:\src\myproj\", @"c:\src\myproj\dir1\..\dir2"));
        }

    }
}