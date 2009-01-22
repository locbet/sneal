using System;
using System.Xml;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace Sneal.ReSharper.MsTest
{
    [Serializable]
    public class MSTestTask : RemoteTask, IEquatable<MSTestTask>
    {
        private readonly bool myExplicitly;
        private readonly string myTestMethod;
        private readonly string myTestType;

        public MSTestTask(string testType, string testMethod, bool explicitly)
            : base(MSTestProvider.CSUnit_ID)
        {
            if (testMethod == null)
                throw new ArgumentNullException("testMethod");
            if (testType == null)
                throw new ArgumentNullException("testType");

            myTestType = testType;
            myTestMethod = testMethod;
            myExplicitly = explicitly;
        }

        public MSTestTask(XmlElement element)
            : base(element)
        {
            myTestMethod = GetXmlAttribute(element, "TestMethod");
            myTestType = GetXmlAttribute(element, "TestType");
            myExplicitly = GetXmlAttribute(element, "Explicitly") == "true";
        }

        public bool Explicitly
        {
            get { return myExplicitly; }
        }

        public string TestMethod
        {
            get { return myTestMethod; }
        }

        private string TestType
        {
            get { return myTestType; }
        }

        #region IEquatable<MSTestTask> Members

        public bool Equals(MSTestTask msTestTask)
        {
            if (msTestTask == null) return false;
            if (!base.Equals(msTestTask)) return false;
            return Equals(myTestType, msTestTask.myTestType) && Equals(myTestMethod, msTestTask.myTestMethod)
                   && myExplicitly == msTestTask.myExplicitly;
        }

        #endregion

        public override void SaveXml(XmlElement element)
        {
            base.SaveXml(element);
            SetXmlAttribute(element, "TestMethod", TestMethod);
            SetXmlAttribute(element, "TestType", TestType);
            SetXmlAttribute(element, "Explicitly", Explicitly ? "true" : "false");
        }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            return Equals(obj as MSTestTask);
        }

        public override int GetHashCode()
        {
            int result = base.GetHashCode();
            result = 29*result + myTestType.GetHashCode();
            result = 29*result + myTestMethod.GetHashCode();
            result = 29*result + myExplicitly.GetHashCode();
            return result;
        }
    }
}