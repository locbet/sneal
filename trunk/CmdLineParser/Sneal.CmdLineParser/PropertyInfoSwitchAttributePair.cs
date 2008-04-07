using System.Reflection;

namespace Sneal.CmdLineParser
{
    public class PropertyInfoSwitchAttributePair
    {
        public PropertyInfoSwitchAttributePair(PropertyInfo propertyInfo, SwitchAttribute switchAttribute)
        {
            PropertyInfo = propertyInfo;
            SwitchAttribute = switchAttribute;
        }

        public PropertyInfo PropertyInfo;
        public SwitchAttribute SwitchAttribute;
    }
}