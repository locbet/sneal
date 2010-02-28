using Autofac;
using NUnit.Framework;
using Stormwind.Infrastructure;

[SetUpFixture]
public class AssemblySetup
{
    public static IContainer ApplicationContainer { get; set; }

    [SetUp]
    public void OneTimeAssemblySetUp()
    {
        var bootstrap = new Bootstrap();
    	bootstrap.StormwindApplication();
        ApplicationContainer = bootstrap.ContainerProvider.ApplicationContainer;
    }

	[TearDown]
	public void Dispose()
	{
		ApplicationContainer.Dispose();
	}
}
