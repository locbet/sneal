using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace Sneal.ReSharper.MsTest
{
    [Serializable]
    public class MSTestFixtureTask : RemoteTask, IEquatable<MSTestFixtureTask>
    {
        private readonly string myAssemblyLocation;

        private readonly bool myExplicitly;
        private readonly string myTypeName;

        [NonSerialized] private object myInstance;

        public MSTestFixtureTask(string assemblyLocation, string typeName, bool explicitly)
            : base(MSTestProvider.CSUnit_ID)
        {
            if (typeName == null)
                throw new ArgumentNullException("typeName");

            myAssemblyLocation = assemblyLocation;
            myTypeName = typeName;
            myExplicitly = explicitly;
        }

        [SuppressMessage("", "")]
        public MSTestFixtureTask(XmlElement element)
            : base(element)
        {
            myTypeName = GetXmlAttribute(element, "TypeName");
            myAssemblyLocation = GetXmlAttribute(element, "AssemblyLocation");
            myExplicitly = GetXmlAttribute(element, "Explicitly") == "true";
        }

        public string AssemblyLocation
        {
            get { return myAssemblyLocation; }
        }

        public string TypeName
        {
            get { return myTypeName; }
        }

        public bool Explicitly
        {
            get { return myExplicitly; }
        }

        public object Instance
        {
            get { return myInstance; }
            set { myInstance = value; }
        }

        #region IEquatable<MSTestFixtureTask> Members

        public bool Equals(MSTestFixtureTask msTestFixtureTask)
        {
            if (msTestFixtureTask == null) return false;
            if (!base.Equals(msTestFixtureTask)) return false;
            return Equals(myAssemblyLocation, msTestFixtureTask.myAssemblyLocation)
                   && Equals(myTypeName, msTestFixtureTask.myTypeName)
                   && myExplicitly == msTestFixtureTask.myExplicitly;
        }

        #endregion

        public override void SaveXml(XmlElement element)
        {
            base.SaveXml(element);
            SetXmlAttribute(element, "TypeName", TypeName);
            SetXmlAttribute(element, "AssemblyLocation", AssemblyLocation);
            SetXmlAttribute(element, "Explicitly", Explicitly ? "true" : "false");
        }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            return Equals(obj as MSTestFixtureTask);
        }

        public override int GetHashCode()
        {
            int result = base.GetHashCode();
            result = 29*result + myAssemblyLocation.GetHashCode();
            result = 29*result + myTypeName.GetHashCode();
            result = 29*result + myExplicitly.GetHashCode();
            return result;
        }
    }
}