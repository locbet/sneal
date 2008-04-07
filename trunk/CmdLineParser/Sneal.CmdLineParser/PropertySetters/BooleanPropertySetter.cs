using System;

namespace Sneal.CmdLineParser.PropertySetters
{
    public class BooleanPropertySetter : IPropertySetter
    {
        #region IPropertySetter Members

        public void SetPropertyValue(PropertyInfoSwitchAttributePair propertyPair, object instanceToSet, string rawValue)
        {
            if (propertyPair.PropertyInfo.PropertyType != SupportedType)
            {
                throw new ArgumentException(
                    "The property given was not of the expected type for this " +
                    "IPropertySetter implementation.");
            }

            bool bVal;
            if (!Boolean.TryParse(rawValue, out bVal))
            {
                throw new CmdLineParserException(
                    string.Format(
                        "The command line argument for flag {0} was not a boolean.  {1}",
                        propertyPair.SwitchAttribute.Name, propertyPair.SwitchAttribute.Description));
            }

            propertyPair.PropertyInfo.SetValue(instanceToSet, bVal, null);
        }

        public Type SupportedType
        {
            get { return typeof(bool); }
        }

        #endregion
    }
}