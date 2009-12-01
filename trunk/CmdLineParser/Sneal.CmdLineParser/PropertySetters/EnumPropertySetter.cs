using System;
using System.Reflection;

namespace Sneal.CmdLineParser.PropertySetters
{
    /// <summary>
    /// Strategy for setting enum properties.
    /// </summary>
    public class EnumPropertySetter : IPropertySetter
    {
        public void SetPropertyValue(Option option, PropertyInfo propertyInfo, object instanceToSet, string rawValue)
        {
            string value = rawValue ?? "";
            object enumValue;
            try
            {
                enumValue = Enum.Parse(propertyInfo.PropertyType, value, true);
            }
            catch (ArgumentException ex)
            {
                throw new CommandLineException(
                    string.Format(
                        "The command line argument for flag {0} was not a valid enumerated value.  {1}",
                        option.Name,
                        option.Description), ex);                
            }
            propertyInfo.SetValue(instanceToSet, enumValue, null);
        }

        public Type SupportedType
        {
            get { return typeof(Enum); }
        }

        public bool SupportsType(Type type)
        {
            return (typeof(Enum).IsAssignableFrom(type));
        }
    }
}
