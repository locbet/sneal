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
using System.Net.Sockets;

namespace Sneal.JsUnitUtils.Servers
{
    /// <summary>
    /// Utility class used to find an open dynamic port.
    /// </summary>
    public class FreeTcpPortFinder : IFreeTcpPortFinder
    {
        // Dynamic port range is 49152–65535 by default
        public int MinPort = 49152;
        public int MaxPort = 65535;

        /// <summary>
        /// Finds an unused port between 49152 and 65535.
        /// </summary>
        /// <returns>The port number</returns>
        public int FindFreePort()
        {
            int portCandidate;
            do
            {
                portCandidate = GetRandomDynamicPort();
            } while (!IsPortOpen(portCandidate));

            return portCandidate;
        }

        /// <summary>
        /// Finds an unused port between 49152 and 65535 if the suggested port
        /// is not open.  If the suggested port is open, it is returned.
        /// </summary>
        /// <param name="suggestedPort">The port to try and use.</param>
        /// <returns>The port number</returns>
        public int FindFreePort(int suggestedPort)
        {
            if (IsPortOpen(suggestedPort))
            {
                return suggestedPort;
            }
            return FindFreePort();
        }

        /// <summary>
        /// Returns <c>false</c> if the port is in use.
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        private static bool IsPortOpen(int port)
        {
            var tcpClient = new TcpClient();
            try
            {
                tcpClient.Connect("127.0.0.1", port);
                tcpClient.Close();
                return false;
            }
            catch (SocketException)
            {
            }

            return true;
        }

        private int GetRandomDynamicPort()
        {
            Random ran = new Random(DateTime.Now.Millisecond);
            return ran.Next(MinPort, MaxPort);
        }
    }
}
