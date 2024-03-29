using System.Drawing;
using JetBrains.CommonControls;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestExplorer;
using JetBrains.TreeModels;
using JetBrains.UI.TreeView;
using JetBrains.ReSharper.Features.Common.TreePsiBrowser;
namespace Sneal.ReSharper.MsTest
{
    public class MSTestPresenter :  TreeModelBrowserPresenter
    {
        public MSTestPresenter()
        {
            Present<MSTestFixtureElement>(PresentTestFixture);
            Present<MSTestElement>(PresentTest);
        }

        protected virtual void PresentTest(MSTestElement value, IPresentableItem item, TreeModelNode modelNode,
                                           PresentationState state)
        {
            item.Clear();
            if (value.Fixture.GetTypeClrName() != value.GetTypeClrName())
                item.RichText = string.Format("{0}.{1}", new CLRTypeName(value.GetTypeClrName()).ShortName,
                                              value.MethodName);
            else
                item.RichText = value.MethodName;

            if (value.IsExplicit)
                item.RichText.SetForeColor(SystemColors.GrayText);

            Image typeImage = UnitTestManager.GetStandardImage(UnitTestElementImage.Test);
            Image stateImage = UnitTestManager.GetStateImage(state);
            if (stateImage != null)
                item.Images.Add(stateImage);
            else if (typeImage != null)
                item.Images.Add(typeImage);
        }

        protected virtual void PresentTestFixture(MSTestFixtureElement value, IPresentableItem item,
                                                  TreeModelNode modelNode, PresentationState state)
        {
            item.Clear();
            if (IsNodeParentNatural(modelNode, value))
                item.RichText = new CLRTypeName(value.GetTypeClrName()).ShortName;
            else
            {
                var name = new CLRTypeName(value.GetTypeClrName());
                if (string.IsNullOrEmpty(name.NamespaceName))
                    item.RichText = string.Format("{0}", name.ShortName);
                else
                    item.RichText = string.Format("{0}.{1}", name.NamespaceName, name.ShortName);
            }

            Image typeImage = UnitTestManager.GetStandardImage(UnitTestElementImage.TestContainer);
            Image stateImage = UnitTestManager.GetStateImage(state);
            if (stateImage != null)
                item.Images.Add(stateImage);
            else if (typeImage != null)
                item.Images.Add(typeImage);
            AppendOccurencesCount(item, modelNode, "test");
        }

        protected override bool IsNaturalParent(object parentValue, object childValue)
        {
            var ns = parentValue as UnitTestNamespace;
            var fixture = childValue as MSTestFixtureElement;
            if (fixture != null && ns != null)
                return ns.Equals(fixture.GetNamespace());

            return base.IsNaturalParent(parentValue, childValue);
        }

        protected override object Unwrap(object value)
        {
            var testElement = value as MSTestElement;
            if (testElement != null)
                value = testElement.GetDeclaredElement();
            var testFixture = value as MSTestFixtureElement;
            if (testFixture != null)
                value = testFixture.GetDeclaredElement();
            return base.Unwrap(value);
        }
    }
}