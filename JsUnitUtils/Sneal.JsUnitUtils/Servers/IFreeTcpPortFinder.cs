namespace Sneal.JsUnitUtils.Servers
{
    public interface IFreeTcpPortFinder
    {
        /// <summary>
        /// Finds an unused port between 49152 and 65535.
        /// </summary>
        /// <returns>The port number</returns>
        int FindFreePort();

        /// <summary>
        /// Finds an unused port between 49152 and 65535 if the suggested port
        /// is not open.  If the suggested port is open, it is returned.
        /// </summary>
        /// <param name="suggestedPort">The port to try and use.</param>
        /// <returns>The port number</returns>
        int FindFreePort(int suggestedPort);
    }
}