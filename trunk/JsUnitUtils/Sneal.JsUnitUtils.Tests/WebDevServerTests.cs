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

namespace Sneal.JsUnitUtils.Tests
{
    [TestFixture]
    public class WebDevServerTests
    {
        [Test]
        public void Should_find_open_random_dynamic_port()
        {
            var server = new WebDevServer(AppDomain.CurrentDomain.BaseDirectory);
            Assert.AreNotEqual(0, server.WebServerPort);
            Assert.IsTrue(server.WebServerPort >= 49152);
        }
    }
}
