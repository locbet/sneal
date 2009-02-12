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

using System.IO;
using NUnit.Framework;
using Rhino.Mocks;
using Sneal.JsUnitUtils.Utils;

namespace Sneal.JsUnitUtils.Tests
{
    [TestFixture]
    public class FixtureFinderTests
    {
        private IWebServer webserver;

        [SetUp]
        public void SetUp()
        {
            webserver = MockRepository.GenerateMock<IWebServer>();
        }

        [Test]
        public void Should_return_null_when_fixture_not_found()
        {
            FixtureFinder finder = new FixtureFinder(webserver, new TestDiskProvider());
            Assert.IsNull(finder.GetTestRunnerPath());
        }

        [Test]
        public void Should_turn_path_into_relative_http_path()
        {
            var diskProvider = new TestDiskProvider();
            diskProvider.FoundFile = "c:\\src\\myproj\\subdir1\\testRunner.html";

            webserver.Stub(o => o.MakeHttpUrl(diskProvider.FoundFile))
                .Return("http://localhost:8080/subdir1/testRunner.html");

            var finder = new FixtureFinder(webserver, diskProvider);

            Assert.AreEqual("http://localhost:8080/subdir1/testRunner.html",
                finder.GetTestRunnerPath());
        }

        [Test, ExpectedException(typeof(FileNotFoundException))]
        public void Should_throw_exception_when_test_runner_is_not_found()
        {
            var diskProvider = new TestDiskProvider();
            diskProvider.FoundFile = null;
            var finder = new FixtureFinder(webserver, diskProvider);
            finder.GetTestRunnerPath();
        }
    }

    internal class TestDiskProvider : DiskProviderImpl
    {
        public string FoundFile;

        public override string FindFile(string directory, string fileName)
        {
            return FoundFile;
        }
    }
}