#region license
// Copyright 2009 Shawn Neal (sneal@sneal.net)
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
using Sneal.Core.IO;

namespace Sneal.Core.Tests.IO
{
    [TestFixture]
    public class PathBuilderTests
    {
        private IPathBuilder _pathBuilder = new PathBuilder();

        [Test]
        public void EnsureDirectorySeparator_replaces_any_instances_of_backslash()
        {
            Assert.AreEqual(@"d:/myfolder/",
                _pathBuilder.EnsureDirectorySeparator(@"d:\myfolder\", '/'));
        }

        [Test]
        public void EnsureDirectorySeparator_replaces_any_instances_of_forwardslash()
        {
            Assert.AreEqual(@"d:\myfolder\",
                _pathBuilder.EnsureDirectorySeparator(@"d:/myfolder/", '\\'));
        }

        [Test]
        public void Can_make_http_path_http()
        {
            Assert.AreEqual("http://mysource/dir1",
                _pathBuilder.ChangeScheme("http://mysource/dir1", Uri.UriSchemeHttp));
        }

        [Test]
        public void Can_make_http_path_with_port_http()
        {
            Assert.AreEqual("http://mysource:80/dir1",
                _pathBuilder.ChangeScheme("http://mysource:80/dir1", Uri.UriSchemeHttp));
        }

        [Test]
        public void Can_make_local_drive_path_http()
        {
            Assert.AreEqual("http://mysource/dir1",
                _pathBuilder.ChangeScheme(@"d:\mysource\dir1", Uri.UriSchemeHttp));
        }

        [Test]
        public void Can_make_relative_path_http()
        {
            Assert.AreEqual("http://mysource/dir1",
                _pathBuilder.ChangeScheme(@"\mysource\dir1", Uri.UriSchemeHttp));
        }

        [Test]
        public void Can_normalize_extended_path()
        {
            Assert.AreEqual(@"D:\source\subdir1\subdir4\",
                _pathBuilder.Normalize(@"D:\source\subdir1\subdir2\subdir3\..\..\subdir4\"));
        }


        [Test]
        public void Can_normalize_http_path()
        {
            Assert.AreEqual(@"http://www.somedomain.com/dir1/subdir1",
                _pathBuilder.Normalize(@"http://www.somedomain.com/dir1/dir2/../subdir1"));
        }

        [Test]
        public void Can_normalize_single_path()
        {
            Assert.AreEqual(@"D:\source\subdir\",
                _pathBuilder.Normalize(@"D:\source\dir1\..\subdir\"));
        }

        [Test]
        public void Can_combine_two_drive_paths()
        {
            Assert.AreEqual(@"d:\dir1\dir2\", _pathBuilder.Combine(@"d:\dir1\", @"\dir2\"));
        }

        [Test]
        public void Should_combine_relative_http_path()
        {
            Assert.AreEqual("http://localhost:90/dir1/dir2/dir3",
                _pathBuilder.Combine("http://localhost:90/dir1/", "/dir2/dir3"));
        }

        [Test]
        public void Should_combine_two_relative_http_paths()
        {
            Assert.AreEqual("/dir1/dir2", _pathBuilder.Combine("/dir1/", "dir2"));
        }

        [Test]
        public void Should_combine_three_local_paths()
        {
            Assert.AreEqual(@"c:\dir1\dir2\dir3\dir4",
                _pathBuilder.Combine(@"c:\dir1\", "dir2", "dir3/dir4"));
        }

        [Test]
        public void Combine_should_combine_directory_and_filename()
        {
            Assert.AreEqual(@"c:\folder\filename.txt",
                _pathBuilder.Combine(@"c:\folder", "filename.txt"));
        }

        [Test]
        public void Combine_should_combine_two_directory_parts_and_filename()
        {
            Assert.AreEqual(@"c:\folder\folder2\filename.txt",
                _pathBuilder.Combine(@"c:\folder", "folder2", "filename.txt"));
        }


        [Test]
        public void Should_make_path_relative()
        {
            Assert.AreEqual(@"dir1\dir2",
                _pathBuilder.MakeRelative(@"c:\src\myproj\", @"c:\src\myproj\dir1\dir2"));
        }

        [Test]
        public void Should_make_path_relative_when_not_normalized()
        {
            Assert.AreEqual(@"dir2",
                _pathBuilder.MakeRelative(@"c:\src\myproj\", @"c:\src\myproj\dir1\..\dir2"));
        }

        [Test]
        public void Should_get_file_extension()
        {
            Assert.AreEqual(@".txt",
                _pathBuilder.FileExtension(@"c:\folder\file.txt"));
        }

        [Test]
        public void Should_get_http_file_extension()
        {
            Assert.AreEqual(@".txt",
                _pathBuilder.FileExtension(@"http://domain.com/file.txt"));
        }

        [Test]
        public void Should_get_file_name()
        {
            Assert.AreEqual(@"file.txt",
                _pathBuilder.FileName(@"c:\folder\file.txt"));
        }

        [Test]
        public void Should_get_http_file_name()
        {
            Assert.AreEqual(@"file.txt",
                _pathBuilder.FileName(@"http://domain.com/dir/file.txt"));
        }

        [Test]
        public void Should_get_file_name_without_extension()
        {
            Assert.AreEqual(@"file",
                _pathBuilder.FileNameWithoutExtension(@"c:\folder\file.txt"));
        }

        [Test]
        public void Should_get_directory_name()
        {
            Assert.AreEqual(@"c:\folder",
                _pathBuilder.DirectoryName(@"c:\folder\file.txt"));
        }
    }
}