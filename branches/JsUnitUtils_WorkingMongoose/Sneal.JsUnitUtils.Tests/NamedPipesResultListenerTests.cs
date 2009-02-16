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
using System.IO.Pipes;
using System.Threading;
using NUnit.Framework;
using Sneal.JsUnitUtils;

namespace Sneal.JsUnitUtils.Tests
{
    [TestFixture]
    public class NamedPipesResultListenerTests
    {
        [Test]
        public void Should_get_the_results()
        {
            string results = null;
            ThreadStart start = delegate
            {
                results = new NamedPipesResultListener().WaitForResults();
            };

            // start listener in another thread so we don't block
            var listenerThread = new Thread(start);
            listenerThread.Start();

            // now send the results via named pipes to the listener on the other thread
            using (var pipeStream = new NamedPipeClientStream(
                ".", Constants.JsUnitResultNamedPipe, PipeDirection.Out))
            {
                pipeStream.Connect(2000);

                using (var sw = new StreamWriter(pipeStream))
                {
                    sw.AutoFlush = true;
                    sw.Write("w00t!");
                }
            }

            // ensure we give the other thread a chance to assign the value to results
            Thread.Sleep(500);  
            Assert.AreEqual("w00t!", results);
        }

        [Test]
        public void Should_timeout_after_200_milliseconds()
        {
            var listener = new NamedPipesResultListener();
            Assert.IsNull(listener.WaitForResults(200));
        }

        [Test, Ignore("Manual test to make sure the timeout is working as expected")]
        public void Should_timeout_after_5000_milliseconds()
        {
            var listener = new NamedPipesResultListener();
            Assert.IsNull(listener.WaitForResults(5000));
        }
    }
}
