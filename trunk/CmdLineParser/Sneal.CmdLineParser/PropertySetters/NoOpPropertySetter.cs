using System;
using System.Reflection;

namespace Sneal.CmdLineParser.PropertySetters
{
    /// <summary>
    /// A property setter that does nothing, null object.
    /// </summary>
    public class NoOpPropertySetter : IPropertySetter
    {
        public void SetPropertyValue(Option option, PropertyInfo propertyInfo, object instanceToSet, string rawValue)
        {
        }

        public Type SupportedType
        {
            get { return typeof(object); }
        }

        public bool SupportsType(Type type)
        {
            return false;
        }
    }
}
