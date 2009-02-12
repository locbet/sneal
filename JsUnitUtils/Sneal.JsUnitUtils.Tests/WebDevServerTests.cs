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
using System.IO;
using NUnit.Framework;
using Rhino.Mocks;
using Sneal.JsUnitUtils.Utils;

namespace Sneal.JsUnitUtils.Tests
{
    [TestFixture]
    public class WebDevServerTests
    {
        IDiskProvider diskProvider = MockRepository.GenerateStub<IDiskProvider>();

        [Test]
        public void Should_find_open_random_dynamic_port()
        {
            var server = new WebDevServer(diskProvider, AppDomain.CurrentDomain.BaseDirectory);
            Assert.AreNotEqual(0, server.WebServerPort);
            Assert.IsTrue(server.WebServerPort >= 49152);
        }

        [Test]
        public void Should_make_http_path_to_file_in_subfolder()
        {
            diskProvider = new DiskProviderImpl();
            var server = new WebDevServer(diskProvider, AppDomain.CurrentDomain.BaseDirectory);
            string pathToTestFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"subdir1\subdir2\testFile.html");
            Assert.AreEqual(string.Format("http://localhost:{0}/subdir1/subdir2/testFile.html", server.WebServerPort),
                            server.MakeHttpUrl(pathToTestFile));
        }
    }
}
