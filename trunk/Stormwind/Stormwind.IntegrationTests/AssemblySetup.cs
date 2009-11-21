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
            DevMode = true
        };

        new Bootstrap(appSettings)
            .DependencyInjectionContainer()
            .NHibernate()
            .Schema()
            .Go();
    }
}
