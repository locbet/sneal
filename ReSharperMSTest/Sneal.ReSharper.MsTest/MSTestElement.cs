using JetBrains.ProjectModel;
using JetBrains.ReSharper.CodeInsight.Services.CamelTyping;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.ReSharper.UnitTestExplorer;

namespace Sneal.ReSharper.MsTest
{
    public class MSTestElement : MSTestElementBase
    {
        private readonly MSTestFixtureElement myFixture;
        private readonly string myMethodName;
        private readonly int myOrder;

        public MSTestElement(IUnitTestProvider provider, MSTestFixtureElement fixture, IProject project,
                                 string declaringTypeName, string methodName, int order)
            : base(provider, fixture, project, declaringTypeName)
        {
            myFixture = fixture;
            myOrder = order;
            myMethodName = methodName;
        }

        public MSTestFixtureElement Fixture
        {
            get { return myFixture; }
        }

        public string MethodName
        {
            get { return myMethodName; }
        }

        public int Order
        {
            get { return myOrder; }
        }

        public override string GetTitle()
        {
            return string.Format("{0}.{1}", myFixture.GetTitle(), myMethodName);
        }

        public override bool Matches(string filter, PrefixMatcher matcher)
        {
            if (myFixture.Matches(filter, matcher))
                return true;
            return matcher.IsMatch(myMethodName);
        }

        public override IDeclaredElement GetDeclaredElement()
        {
            ITypeElement declaredType = GetDeclaredType();
            if (declaredType == null)
                return null;
            foreach (ITypeMember member in MiscUtil.EnumerateMembers(declaredType, myMethodName, false))
            {
                var method = member as IMethod;
                if (method == null)
                    continue;
                if (method.IsAbstract)
                    continue;
                if (method.TypeParameters.Length > 0)
                    continue;
                if (method.AccessibilityDomain.DomainType != AccessibilityDomain.AccessibilityDomainType.PUBLIC)
                    continue;
                return member;
            }
            return null;
        }

        public override string GetKind()
        {
            return "MSTest Test";
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;
            var element = (MSTestElement) obj;
            return Equals(myFixture, element.myFixture) && myMethodName == element.myMethodName;
        }

        public override int GetHashCode()
        {
            int result = base.GetHashCode();
            result = 29*result + myFixture.GetHashCode();
            result = 29*result + myMethodName.GetHashCode();
            return result;
        }
    }
}