using System;
using System.Collections.Generic;
using Sneal.CmdLineParser;

namespace Sneal.SqlMigration.Console
{
    /// <summary>
    /// Sets an object property of type IList&lt;DbObjectType&gt;.  Takes
    /// comma or semi-colon delimited last and adds each element to the
    /// List.
    /// </summary>
    public class ListPropertySetter<T> : IPropertySetter
    {
        public delegate T CreateItemAction(string argPart);
        private readonly CreateItemAction createItemAction;

        public ListPropertySetter(CreateItemAction createItemAction)
        {
            this.createItemAction = createItemAction;
        }

        #region IPropertySetter Members

        public void SetPropertyValue(PropertyInfoSwitchAttributePair propertyPair, object instanceToSet, string rawValue)
        {
            if (propertyPair.PropertyInfo.PropertyType != SupportedType)
            {
                throw new ArgumentException(
                    "The property given was not of the expected type for this " +
                    "IPropertySetter implementation.");
            }

            if (string.IsNullOrEmpty(rawValue))
            {
                throw new CmdLineParserException(
                    string.Format(
                        "The flag {0} must have an associated value",
                        propertyPair.SwitchAttribute.Name));
            }

            string[] parts = rawValue.Split(new char[] {',', ';'});

            foreach (string part in parts)
            {
                object listObj = propertyPair.PropertyInfo.GetValue(instanceToSet, null);
                IList<T> list = listObj as IList<T>;
                if (list == null)
                {
                    throw new ArgumentException(
                        string.Format(
                            "Expected the property {0} to return type {1}",
                            propertyPair.PropertyInfo.Name, propertyPair.PropertyInfo.PropertyType));
                }

                list.Add(createItemAction(part));
            }
        }

        public Type SupportedType
        {
            get { return typeof (IList<T>); }
        }

        #endregion
    }
}