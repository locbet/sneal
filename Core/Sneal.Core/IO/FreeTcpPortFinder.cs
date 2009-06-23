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
using System.Net.Sockets;

namespace Sneal.Core.IO
{
    /// <summary>
    /// Utility class used to find an available TCP port.
    /// </summary>
    /// <remarks>
    /// The dynamic port range is 49152–65535 by default.
    /// </remarks>
    public class FreeTcpPortFinder
    {
        /// <summary>
        /// The minimum port number this class will consider, the default is 49152.
        /// </summary>
        public int MinPort = 49152;

        /// <summary>
        /// The maximum port number this class will consider, the default is 65535.
        /// </summary>
        public int MaxPort = 65535;

        /// <summary>
        /// Finds an unused port between MinPort and MaxPort.
        /// </summary>
        /// <returns>The port number</returns>
        public virtual int FindFreePort()
        {
            int portCandidate;
            do
            {
                portCandidate = GetRandomDynamicPort();
            } while (!IsPortAvailable(portCandidate));

            return portCandidate;
        }

        /// <summary>
        /// Finds an unused port between MinPort and MaxPort. If the suggested
        /// port is available it is returned, otherwise a random available
        /// port is returned.
        /// </summary>
        /// <param name="suggestedPort">The port to try and use.</param>
        /// <returns>The port number</returns>
        public virtual int FindFreePort(int suggestedPort)
        {
            if (IsPortAvailable(suggestedPort))
            {
                return suggestedPort;
            }
            return FindFreePort();
        }

        /// <summary>
        /// Returns <c>false</c> if the port is in use.
        /// </summary>
        /// <param name="port">The port number to test.</param>
        /// <returns>True if nothing is listening on the specified port.</returns>
        public virtual bool IsPortAvailable(int port)
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

        protected virtual int GetRandomDynamicPort()
        {
            Random ran = new Random(DateTime.Now.Millisecond);
            return ran.Next(MinPort, MaxPort);
        }
    }
}