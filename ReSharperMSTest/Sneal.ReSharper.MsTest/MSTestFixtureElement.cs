using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Filtering;
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
            var solution = GetSolution();
            var module = PsiModuleManager.GetInstance(solution).GetPsiModule(GetProject().ProjectFile);
            IDeclarationsScope scope = DeclarationsScopeFactory.ModuleScope(module, false);
            IDeclarationsCache declarationsCache = PsiManager.GetInstance(solution).GetDeclarationsCache(scope, true);

            return declarationsCache.GetTypeElementByCLRName(GetTypeClrName());
        }

        public override string GetKind()
        {
            return "MSTest Fixture";
        }
    }
}