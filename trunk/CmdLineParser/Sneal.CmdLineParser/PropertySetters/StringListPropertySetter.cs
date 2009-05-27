using System;
using System.Collections.Generic;

namespace Sneal.CmdLineParser.PropertySetters
{
    /// <summary>
    /// Sets a property of type IList&gt;string&gt;
    /// </summary>
    public class StringListPropertySetter : IPropertySetter
    {
        public void SetPropertyValue(PropertyInfoSwitchAttributePair propertyPair, object instanceToSet, string rawValue)
        {
            var values = new List<string>();
            string val = rawValue == null ? rawValue : rawValue.Trim();
            if (val != null)
            {
                foreach (var s in val.Split(new[] { ' ', ';', ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    values.Add(s);
                }
            }

            propertyPair.PropertyInfo.SetValue(instanceToSet, values, null);
        }

        public Type SupportedType
        {
            get { return typeof(IList<string>); }
        }
    }
}
