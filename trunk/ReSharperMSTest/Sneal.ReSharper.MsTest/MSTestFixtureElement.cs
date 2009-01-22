using JetBrains.ProjectModel;
using JetBrains.ReSharper.CodeInsight.Services.CamelTyping;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.UnitTestExplorer;

namespace Sneal.ReSharper.MsTest
{
    public class MSTestFixtureElement : MSTestElementBase
    {
        private readonly string myAssemblyLocation;

        public MSTestFixtureElement(IUnitTestProvider provider, IProject project, string typeName,
                                        string assemblyLocation)
            : base(provider, null, project, typeName)
        {
            myAssemblyLocation = assemblyLocation;
        }

        public string AssemblyLocation
        {
            get { return myAssemblyLocation; }
        }

        public override bool Matches(string filter, PrefixMatcher matcher)
        {
            return matcher.IsMatch(GetTypeClrName());
        }

        public override string GetTitle()
        {
            return new CLRTypeName(GetTypeClrName()).ShortName;
        }

        public override IDeclaredElement GetDeclaredElement()
        {
            PsiManager manager = PsiManager.GetInstance(GetSolution());
            IDeclarationsCache declarationsCache =
                manager.GetDeclarationsCache(DeclarationsCacheScope.ProjectScope(GetProject(), false), true);
            return declarationsCache.GetTypeElementByCLRName(GetTypeClrName());
        }

        public override string GetKind()
        {
            return "MSTest Fixture";
        }
    }
}