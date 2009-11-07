using NUnit.Framework;
using Stormwind.Infrastructure;

[SetUpFixture]
public class AssemblySetup
{
    [SetUp]
    public void OneTimeAssemblySetUp()
    {
        var appSettings = new AppSettings
        {
            ConnectionString = "Server=localhost;Database=StormwindIntegration;Uid=root;Pwd=disk44you;"
        };

        new Bootstrap(appSettings)
            .DependencyInjectionContainer()
            .NHibernate()
            .Schema()
            .Go();
    }
}
