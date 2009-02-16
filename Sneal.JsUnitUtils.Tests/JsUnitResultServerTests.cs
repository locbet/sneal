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

using NUnit.Framework;
using Rhino.Mocks;
using Sneal.JsUnitUtils.Servers;
using Sneal.JsUnitUtils.Utils;

namespace Sneal.JsUnitUtils.Tests
{
    [TestFixture]
    public class JsUnitResultServerTests
    {
        private readonly IDiskProvider diskProvider = MockRepository.GenerateStub<IDiskProvider>();
        private readonly ITemplates templates = MockRepository.GenerateStub<ITemplates>();
        private readonly IFreeTcpPortFinder portFinder = MockRepository.GenerateStub<IFreeTcpPortFinder>();
        private JsUnitResultServer posthandler;
 
        [SetUp]
        public void SetUp()
        {
            templates.Stub(o => o.AshxHandlerFileName).Return("Handler.ashx");
            portFinder.Stub(o => o.FindFreePort(60000)).Return(60000);
            posthandler = new JsUnitResultServer(diskProvider, templates, portFinder);
        }

        [Test]
        public void Ashx_handler_defaults_to_port_60000()
        {
            Assert.IsTrue(posthandler.HandlerAddress.Contains(":60000"));
        }

        [Test]
        public void Ashx_handler_is_localhost()
        {
            Assert.IsTrue(posthandler.HandlerAddress.StartsWith("http://localhost"));
        }

        [Test]
        public void Ashx_handler_is_file_comes_from_templates()
        {
            Assert.IsTrue(posthandler.HandlerAddress.EndsWith("Handler.ashx"));
        }
    }
}
