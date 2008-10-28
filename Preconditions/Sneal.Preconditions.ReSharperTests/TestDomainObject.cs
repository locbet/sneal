using System.Net;
using JetBrains.Annotations;

namespace Sneal.Preconditions.ReSharperTests
{
    public class TestDomainObject
    {
        public virtual void String(string p)
        {

        }

        public virtual void StringNotNull([NotNull]string p)
        {

        }

        public virtual void IPAddressNotNull([NotNull]IPAddress ipAddress)
        {

        }
    }
}
