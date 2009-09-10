using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using JetBrains.Application;
using JetBrains.Application.Progress;
using JetBrains.CommonControls;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.TaskRunnerFramework;
using JetBrains.ReSharper.UnitTestExplorer;
using JetBrains.TreeModels;
using JetBrains.UI.TreeView;
using JetBrains.Util;
using JetBrains.Util.DataStructures;

namespace Sneal.ReSharper.MsTest
{
    [UnitTestProvider]
    internal class MSTestProvider : IUnitTestProvider
    {
        internal const string CSUnit_ID = "CSUnit";
        private static readonly MSTestPresenter ourPresenter = new MSTestPresenter();

        private static readonly CLRTypeName IgnoreAttribute = 
            new CLRTypeName("Microsoft.VisualStudio.TestTools.UnitTesting.IgnoreAttribute");
        private static readonly CLRTypeName TestAttribute =
            new CLRTypeName("Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute");
        private static readonly CLRTypeName TestFixtureAttribute = 
            new CLRTypeName("Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute");

        public string ID
        {
            get { return CSUnit_ID; }
        }

        public string Name
        {
            get { return "Sneal MSTest Runner 4.5"; }
        }

        public Image Icon
        {
            get { return null; }
        }

        public void ExploreAssembly(IMetadataAssembly assembly, IProject project, UnitTestElementConsumer consumer)
        {
            foreach (IMetadataTypeInfo type in assembly.GetTypes())
            {
                if (!IsTestFixture(type))
                    continue;

                var fixture = new MSTestFixtureElement(this, project, type.FullyQualifiedName, assembly.Location);
                fixture.SetExplicit(GetExplicitString(type));

                consumer(fixture);
                int order = 0;
                foreach (IMetadataMethod method in GetAllTestMethods(type))
                {
                    if (!IsTestMethod(method))
                        continue;

                    var testElement = new MSTestElement(this, fixture, project,
                                                            method.DeclaringType.FullyQualifiedName, method.Name,
                                                            order++);
                    testElement.SetExplicit(GetExplicitString(method));
                    consumer(testElement);
                }
            }
        }

        public void ExploreFile(IFile psiFile, UnitTestElementLocationConsumer consumer, CheckForInterrupt interrupted)
        {
            if (psiFile == null)
                throw new ArgumentNullException("psiFile");

            psiFile.ProcessDescendants(new MyFileExplorer(this, consumer, psiFile, interrupted));
        }

        public bool IsUnitTestElement(IDeclaredElement element)
        {
            var typeElement = element as ITypeElement;
            if (typeElement != null && IsTestFixture(typeElement))
                return true;

            var typeMember = element as ITypeMember;
            if (typeMember != null)
            {
                typeElement = typeMember.GetContainingType();
                if (typeElement != null && IsTestFixture(typeElement))
                    if (IsTestMethod(typeMember))
                        return true;
            }
            return false;
        }

        public bool IsUnitTestStuff(IDeclaredElement element)
        {
            bool isUnitTestElement = false;

            IClass elementAsClass = element as IClass;
            if (elementAsClass != null)
            {
                foreach (ITypeElement nestedType in elementAsClass.NestedTypes)
                    isUnitTestElement |= IsUnitTestElement(nestedType);
            }

            return IsUnitTestElement(element) | isUnitTestElement;


        }

        public void ExploreSolution(ISolution solution, UnitTestElementConsumer consumer)
        {
        }

        public ProviderCustomOptionsControl GetCustomOptionsControl(ISolution solution)
        {
            return null;
        }

        public void ExploreExternal(UnitTestElementConsumer consumer)
        {
        }

        public void Present(UnitTestElement element, IPresentableItem presentableItem, TreeModelNode node,
                            PresentationState state)
        {
            ourPresenter.UpdateItem(element, node, presentableItem, state);
        }

        public RemoteTaskRunnerInfo GetTaskRunnerInfo()
        {
            return new RemoteTaskRunnerInfo(typeof (MSTestTaskRunner));
        }

        public string Serialize(UnitTestElement element)
        {
            return null;
        }

        public UnitTestElement Deserialize(ISolution solution, string elementString)
        {
            return null;
        }

        public IList<UnitTestTask> GetTaskSequence(UnitTestElement element, IList<UnitTestElement> explicitElements)
        {
            var testElement = element as MSTestElement;
            if (testElement != null)
            {
                MSTestFixtureElement parentFixture = testElement.Fixture;
                return new[]
                           {
                               new UnitTestTask(null, new AssemblyLoadTask(parentFixture.AssemblyLocation)),
                               new UnitTestTask(parentFixture,
                                                new MSTestFixtureTask(parentFixture.AssemblyLocation,
                                                                          parentFixture.GetTypeClrName(),
                                                                          explicitElements.Contains(parentFixture))),
                               new UnitTestTask(testElement,
                                                new MSTestTask(parentFixture.GetTypeClrName(),
                                                                   testElement.MethodName,
                                                                   explicitElements.Contains(testElement))),
                           };
            }
            var fixture = element as MSTestFixtureElement;
            if (fixture != null)
                return EmptyArray<UnitTestTask>.Instance;

            throw new ArgumentException(string.Format("element is not MSTest: '{0}'", element));
        }

        public int CompareUnitTestElements(UnitTestElement x, UnitTestElement y)
        {
            if (Equals(x, y))
                return 0;

            int compare = StringComparer.CurrentCultureIgnoreCase.Compare(x.GetTypeClrName(), y.GetTypeClrName());
            if (compare != 0)
                return compare;
            if (x is MSTestElement && y is MSTestFixtureElement)
                return -1;
            if (y is MSTestElement && x is MSTestFixtureElement)
                return 1;
            if (x is MSTestFixtureElement && y is MSTestFixtureElement)
                return 0; // two different elements with same type name??

            var xe = (MSTestElement) x;
            var ye = (MSTestElement) y;
            return xe.Order.CompareTo(ye.Order);
        }

        public void ProfferConfiguration(TaskExecutorConfiguration configuration, UnitTestSession session)
        {
        }

        private static bool IsTestFixture(ITypeElement typeElement)
        {
            if (!(typeElement is IClass) && !(typeElement is IStruct))
                return false;

            if (typeElement.AllTypeParameters.Any())
                return false; // type should be concrete

            var modifiersOwner = (IModifiersOwner) typeElement;
            if (modifiersOwner.IsAbstract || modifiersOwner.GetAccessRights() == AccessRights.INTERNAL)
                return false;

            if (typeElement.HasAttributeInstance(TestFixtureAttribute, false))
                return true;

            var @class = typeElement as IClass;
            if (@class != null)
            {
                var visited = new HashSet<IClass>();
                while ((@class = @class.GetSuperClass()) != null)
                {
                    if (visited.Contains(@class))
                        break;
                    visited.Add(@class);
                    if (@class.HasAttributeInstance(TestFixtureAttribute, false))
                        return true;
                }
            }

            return false;
        }

        private static bool IsTestMethod(ITypeMember element)
        {
            var method = element as IMethod;
            if (method == null)
                return false;

            if (method.IsStatic || method.IsAbstract ||
                method.GetAccessRights() != AccessRights.PUBLIC ||
                method.Parameters.Count > 0 || !PredefinedType.IsVoid(method.ReturnType))
                return false;

            if (method.HasAttributeInstance(TestAttribute, false))
                return true;

            return false;
        }

        private static bool IsTestFixture(IMetadataTypeInfo typeInfo)
        {
            if (!typeInfo.IsAbstract && (typeInfo.IsPublic || typeInfo.IsNestedPublic) &&
                typeInfo.GenericParameters.Length == 0)
                return HasTestFixtureAttribute(typeInfo);
            return false;
        }

        private static bool HasTestFixtureAttribute(IMetadataTypeInfo typeInfo)
        {
            if (typeInfo.HasCustomAttribute(TestFixtureAttribute.ClrName))
                return true;

            IMetadataClassType baseType = typeInfo.Base;
            if (baseType != null)
                return HasTestFixtureAttribute(baseType.Type);

            return false;
        }

        private static bool IsTestMethod(IMetadataMethod method)
        {
            if (method.IsStatic || method.IsAbstract || !method.IsPublic) return false;
            if (method.Parameters.Length != 0 || method.GenericArguments.Length != 0) return false;
            if (!method.ReturnValue.Type.PresentableName.StartsWith("System.Void")) return false;

            if (method.HasCustomAttribute(TestAttribute.ClrName))
                return true;

            return false;
        }

        private static string GetExplicitString(IMetadataEntity entity)
        {
            return string.Empty;
        }

        private static List<IMetadataMethod> GetAllTestMethods(IMetadataTypeInfo typeInfo)
        {
            var list = new List<IMetadataMethod>();
            var map = new OneToListMap<string, IMetadataMethod>();

            while (typeInfo != null)
            {
                foreach (IMetadataMethod method in typeInfo.GetMethods())
                {
                    if (!IsTestMethod(method)) continue;

                    if (map.ContainsKey(method.Name) && (method.IsVirtual))
                    {
                        bool hasOverride = false;
                        foreach (IMetadataMethod metadataMethod in map[method.Name])
                        {
                            if (metadataMethod.IsVirtual && !metadataMethod.IsNewSlot)
                                hasOverride = true;
                        }

                        if (hasOverride)
                            continue;
                    }

                    map.AddValue(method.Name, method);
                    list.Add(method);
                }

                IMetadataClassType baseType = typeInfo.Base;
                typeInfo = (baseType != null) ? baseType.Type : null;
            }

            return list;
        }

        #region Nested type: MyFileExplorer

        private class MyFileExplorer : IRecursiveElementProcessor
        {
            private readonly string myAssemblyPath;
            private readonly UnitTestElementLocationConsumer myConsumer;
            private readonly IFile myFile;

            private readonly Dictionary2<IDeclaredElement, MSTestFixtureElement> myFixtures =
                new Dictionary2<IDeclaredElement, MSTestFixtureElement>();

            private readonly CheckForInterrupt myInterrupted;

            private readonly Dictionary2<IDeclaredElement, int> myOrders = new Dictionary2<IDeclaredElement, int>();
            private readonly IProject myProject;
            private readonly IUnitTestProvider myProvider;

            public MyFileExplorer(IUnitTestProvider provider, UnitTestElementLocationConsumer consumer, IFile file,
                                  CheckForInterrupt interrupted)
            {
                if (file == null)
                    throw new ArgumentNullException("file");
                if (provider == null)
                    throw new ArgumentNullException("provider");

                myConsumer = consumer;
                myProvider = provider;
                myFile = file;
                myInterrupted = interrupted;
                myProject = myFile.ProjectFile.GetProject();
                myAssemblyPath = UnitTestManager.GetOutputAssemblyPath(myProject).FullPath;
            }

            #region IRecursiveElementProcessor Members

            public bool InteriorShouldBeProcessed(IElement element)
            {
                return !(element is ITypeMemberDeclaration) || (element is ITypeDeclaration);
            }

            public bool ProcessingIsFinished
            {
                get
                {
                    if (myInterrupted())
                        throw new ProcessCancelledException();
                    return false;
                }
            }

            public void ProcessBeforeInterior(IElement element)
            {
                var declaration = element as IDeclaration;
                if (declaration == null)
                    return;
                MSTestElementBase testElement = null;

                IDeclaredElement declaredElement = declaration.DeclaredElement;
                var typeElement = declaredElement as ITypeElement;
                if (typeElement != null && IsTestFixture(typeElement))
                {
                    MSTestFixtureElement fixtureElement;

                    if (!myFixtures.ContainsKey(typeElement))
                    {
                        fixtureElement = new MSTestFixtureElement(myProvider, myProject, typeElement.CLRName,
                                                                      myAssemblyPath);
                        myFixtures.Add(typeElement, fixtureElement);
                        myOrders.Add(typeElement, 0);
                    }
                    else
                        fixtureElement = myFixtures[typeElement];

                    testElement = fixtureElement;
                    int order = 0;
                    AppendTests(fixtureElement, typeElement.GetSuperTypes(), ref order);
                }


                var typeMember = declaredElement as ITypeMember;
                if (typeMember != null && IsTestMethod(typeMember))
                {
                    ITypeElement type = typeMember.GetContainingType();
                    if (type != null)
                    {
                        MSTestFixtureElement fixtureElement = myFixtures[type];
                        if (fixtureElement != null)
                        {
                            int order = myOrders[type] + 1;
                            myOrders[type] = order;
                            testElement = new MSTestElement(myProvider, fixtureElement, myProject, type.CLRName,
                                                                typeMember.ShortName, order);
                        }
                    }
                }

                if (testElement == null)
                    return;
                var disposition = new UnitTestElementDisposition(testElement, myFile.ProjectFile,
                                                                 declaration.GetNameRange(),
                                                                 declaration.GetDocumentRange().TextRange);
                myConsumer(disposition);
            }


            public void ProcessAfterInterior(IElement element)
            {
            }

            #endregion

            private void AppendTests(MSTestFixtureElement fixtureElement, IEnumerable<IDeclaredType> types,
                                     ref int order)
            {
                foreach (IDeclaredType type in types)
                {
                    ITypeElement typeElement = type.GetTypeElement();
                    if (typeElement == null)
                        continue;
                    foreach (ITypeMember member in typeElement.GetMembers())
                    {
                        if (IsTestMethod(member))
                            new MSTestElement(myProvider, fixtureElement, myProject, typeElement.CLRName,
                                                  member.ShortName, order++);
                    }
                    AppendTests(fixtureElement, type.GetSuperTypes(), ref order);
                }
            }
        }

        #endregion
    }
}