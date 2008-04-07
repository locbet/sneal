using System;

namespace Sneal.CmdLineParser.PropertySetters
{
    public class IntegerPropertySetter : IPropertySetter
    {
        public void SetPropertyValue(PropertyInfoSwitchAttributePair propertyPair, object instanceToSet, string rawValue)
        {
            if (propertyPair.PropertyInfo.PropertyType != SupportedType)
            {
                throw new ArgumentException(
                    "The property given was not of the expected type for this " +
                    "IPropertySetter implementation.");
            }

            int iVal;
            if (!Int32.TryParse(rawValue, out iVal))
            {
                throw new CmdLineParserException(
                    string.Format(
                        "The command line argument for flag {0} was not an integer.  {1}",
                        propertyPair.SwitchAttribute.Name,
                        propertyPair.SwitchAttribute.Description));
            }

            propertyPair.PropertyInfo.SetValue(instanceToSet, iVal, null);
        }

        public Type SupportedType
        {
            get { return typeof(int); }
        }
    }
}