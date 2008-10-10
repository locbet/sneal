﻿#region license
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
using System.Threading;
using NUnit.Framework;
using Rhino.Mocks;
using Sneal.JsUnitUtils;
using Sneal.JsUnitUtils.Utils;

namespace Sneal.JsUnitUtils.Tests
{
    [TestFixture]
    public class JsUnitWebPostHandlerTests
    {
        IDiskProvider diskProvider = MockRepository.GenerateStub<IDiskProvider>();

        [Test]
        public void Should_find_webdev_server_exe()
        {
            var posthandler = new JsUnitWebServer(diskProvider, AppDomain.CurrentDomain.BaseDirectory, new Templates());
            Assert.IsNotNull(posthandler.WebDevServerPath);
        }

        [Test, Ignore("Integration test")]
        public void Should_start_the_web_server_then_stop()
        {
            var posthandler = new JsUnitWebServer(diskProvider, AppDomain.CurrentDomain.BaseDirectory, new Templates());
            posthandler.Start();

            // pause so we can see it start
            Thread.Sleep(2000);

            posthandler.Stop();
        }
    }
}
