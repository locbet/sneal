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
using Sneal.JsUnitUtils;    
using Sneal.JsUnitUtils.Browsers;

namespace Sneal.JsUnitUtils.Tests
{
    [TestFixture]
    public class JsUnitTestManagerTests
    {
        [Test]
        public void Should_create_JsUnitTestRunner()
        {
            var mgr = new JsUnitTestRunnerFactory();
            string testDir = AppDomain.CurrentDomain.BaseDirectory;
            JsUnitTestRunner runner = mgr.CreateRunner(new string[0], testDir, With.InternetExplorer);
            Assert.IsNotNull(runner);
        }

        [Test]
        public void Can_create_more_than_one_JsUnitTestRunner()
        {
            var mgr = new JsUnitTestRunnerFactory();
            string testDir = AppDomain.CurrentDomain.BaseDirectory;

            JsUnitTestRunner runner1 = mgr.CreateRunner(new string[0], testDir, With.InternetExplorer);
            Assert.IsNotNull(runner1);

            JsUnitTestRunner runner2 = mgr.CreateRunner(new string[0], testDir, With.InternetExplorer);
            Assert.IsNotNull(runner2);
        }

    }
}
