using System;

namespace Sneal.CmdLineParser.PropertySetters
{
    public class StringPropertySetter : IPropertySetter
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

            string val = rawValue == null ? rawValue : rawValue.Trim();
            propertyPair.PropertyInfo.SetValue(instanceToSet, val, null);
        }

        public Type SupportedType
        {
            get { return typeof(string); }
        }

        #endregion
    }
}