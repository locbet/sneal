using System;
using System.Reflection;

namespace Sneal.CmdLineParser
{
    public interface IPropertySetter
    {
        void SetPropertyValue(PropertyInfoSwitchAttributePair propertyInfo, 
            object instanceToSet, string rawValue);

        Type SupportedType { get; }
    }
}