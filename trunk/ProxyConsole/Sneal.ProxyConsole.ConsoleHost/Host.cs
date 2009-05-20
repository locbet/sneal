using System;
using System.ServiceModel;
using Sneal.ProxyConsole.WcfService;

namespace Sneal.ProxyConsole.ConsoleHost
{
    public class Host : IDisposable
    {
        private ServiceHost _host;

        public void Start()
        {
            _host = new ServiceHost(typeof(ConsoleRunner));
            _host.Open(TimeSpan.FromSeconds(30));
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _host.Close();
            }
        }
    }
}
