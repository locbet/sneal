using System;

namespace Sneal.CmdLineParser
{
    /// <summary>
    /// Attribute to apply to a class property to be set from the command
    /// line.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SwitchAttribute : Attribute
    {
        private string description;
        private string name;

        public SwitchAttribute(string name, string description)
        {
            this.name = name;
            this.description = description;
        }

        public SwitchAttribute(string name)
        {
            this.name = name;
        }

        public SwitchAttribute()
        {
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }
    }
}
