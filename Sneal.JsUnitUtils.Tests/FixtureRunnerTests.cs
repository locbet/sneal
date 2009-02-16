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
using NUnit.Framework;
using Rhino.Mocks;
using Sneal.JsUnitUtils;

namespace Sneal.JsUnitUtils.Tests
{
    [TestFixture]
    public class FixtureRunnerTests
    {
        private IWebBrowser browser;
        private IResultParser resultParser;
        private IResultListener resultListener;
        private MockRepository mocks;
        private FixtureRunner fixtureRunner;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            browser = mocks.DynamicMock<IWebBrowser>();
            resultParser = mocks.DynamicMock<IResultParser>();
            resultListener = mocks.DynamicMock<IResultListener>();
            fixtureRunner = new FixtureRunner(resultListener, browser, resultParser);
        }

        [Test]
        public void Should_open_browser_get_results_then_close_browser()
        {
            var fixtureUrl = new Uri("http://localhost:9033/JsUnitRunner.html");

            using (mocks.Ordered())
            {
                browser.OpenUrl(fixtureUrl);
                Expect.Call(resultListener.WaitForResults(5000)).Return("Raw JSUnit results");
                browser.Close();
            }

            SetupResult.For(resultParser.ParseJsUnitErrors("Raw JSUnit results"))
                .IgnoreArguments()
                .Return(new List<JsUnitErrorResult>());

            mocks.ReplayAll();

            fixtureRunner.RunFixture(fixtureUrl, 5000);

            mocks.VerifyAll();
        }

        [Test]
        public void Should_return_error_if_errorlist_is_non_empty()
        {
            var fixtureUrl = new Uri("http://localhost:9033/JsUnitRunner.html?");

            SetupResult.For(resultParser.ParseJsUnitErrors(null))
                .IgnoreArguments()
                .Return(new List<JsUnitErrorResult> { new JsUnitErrorResult() });

            mocks.ReplayAll();

            Assert.IsFalse(fixtureRunner.RunFixture(fixtureUrl, 5000));
            Assert.AreEqual(1, fixtureRunner.Errors.Count);

            mocks.VerifyAll();
        }
    }
}
