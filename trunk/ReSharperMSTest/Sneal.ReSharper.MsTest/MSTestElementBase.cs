using System;
using System.Collections.Generic;
using JetBrains.Application;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestExplorer;
using JetBrains.Util;

namespace Sneal.ReSharper.MsTest
{
    public abstract class MSTestElementBase : UnitTestElement
    {
        private readonly ProjectModelElementEnvoy myProject;
        private readonly string myTypeName;

        protected MSTestElementBase(IUnitTestProvider provider, MSTestElementBase parent, IProject project,
                                    string typeName)
            : base(provider, parent)
        {
            if (project == null && !Shell.Instance.IsTestShell)
                throw new ArgumentNullException("project");
            if (typeName == null)
                throw new ArgumentNullException("typeName");

            if (project != null)
                myProject = new ProjectModelElementEnvoy(project);
            myTypeName = typeName;
        }

        public override IProject GetProject()
        {
            return myProject.GetValidProjectElement() as IProject;
        }

        protected ITypeElement GetDeclaredType()
        {
            IProject project = GetProject();
            if (project == null)
                return null;

            using (ReadLockCookie.Create())
            {
                var solution = GetSolution();
                var module = PsiModuleManager.GetInstance(solution).GetPsiModule(GetProject().ProjectFile);
                IDeclarationsScope scope = DeclarationsScopeFactory.ModuleScope(module, true);
                IDeclarationsCache declarationsCache = PsiManager.GetInstance(solution).GetDeclarationsCache(scope, true);

                return declarationsCache.GetTypeElementByCLRName(myTypeName);
            }
        }

        public override string GetTypeClrName()
        {
            return myTypeName;
        }

        public override UnitTestNamespace GetNamespace()
        {
            return new UnitTestNamespace(new CLRTypeName(myTypeName).NamespaceName);
        }

        public override IList<IProjectFile> GetProjectFiles()
        {
            ITypeElement declaredType = GetDeclaredType();
            if (declaredType == null)
                return EmptyArray<IProjectFile>.Instance;
            return declaredType.GetProjectFiles();
        }

        public override UnitTestElementDisposition GetDisposition()
        {
            IDeclaredElement element = GetDeclaredElement();
            if (element != null && element.IsValid())
            {
                var locations = new List<UnitTestElementLocation>();
                foreach (IDeclaration declaration in element.GetDeclarations())
                {
                    IFile file = declaration.GetContainingFile();
                    if (file != null)
                        locations.Add(new UnitTestElementLocation(file.ProjectFile, declaration.GetNameRange(),
                                                                  declaration.GetDocumentRange().TextRange));
                }
                return new UnitTestElementDisposition(locations, this);
            }
            return UnitTestElementDisposition.InvalidDisposition;
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;
            var elementBase = (MSTestElementBase) obj;
            return Equals(elementBase.myProject, myProject) && elementBase.myTypeName == myTypeName;
        }

        public override int GetHashCode()
        {
            int result = base.GetHashCode();
            result = 29*result + myProject.GetHashCode();
            result = 29*result + myTypeName.GetHashCode();
            return result;
        }
    }
}