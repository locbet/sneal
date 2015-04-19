# Why Windsor #

My favorite .NET container is Castle [Windsor](http://www.castleproject.org) because it allows me to do more with less work (configuration), but unfortunately there's no built in way to use Windsor in a truly IoC way with ASP.NET.  The only supported way is to get a reference to the container and ask the container to resolve an instance of something, which directly makes your page dependant upon the container, and generally just adds noise where its not needed; you would see calls like this all over:
```
IFooService service = Container.Resolve<IFooService>();
```
Rather than explicitly asking the container for a service instance, I would much rather have the container inject any required dependencies into my page instance when it gets constructed.  The problem is how do we do this with Windsor?

# Windsor AspNetModule #

The approach I've taken is to create an http module that will inject dependencies into a page just after instantiation using setter injection.  This means that the container will inject any dependencies that my page may have using public property setters.  Constructor injection would be better, but IMO its not worth the effort of having to create my own page handler factory, here we can rely on the tried and true ASP.NET infrastructure.  Perhaps someday I'll change my mind, but for now this is acceptable.

Since I plan on using this container with a large amount of legacy code that doesn't use dependency injection I want to make the use of the container somewhat explicit.  I don't want to waste cycles scanning over a page that doesn't need any dependencies injected.  To enforce this, I make the pages that require injection declare so using a class level attribute.  Here's a succinct example with one dependency:
```
[UsesInjection]
public partial class CustomerDetail : Page
{
    public ICustomerRepository CustomerRepository { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        customerGrid.DataSource = CustomerRepository.GetAll();
        customerGrid.DataBind();
    }
}
```
When the module sees the current handler with the UsesInjection attribute it scans the class for public setter properties and tries to resolve any dependencies by type. If an instance is registered with that type in the container, the module tries to set the property. Here an ICustomerRepository instance gets injected. All of this happens before the page lifecycle begins, so its safe to use these dependencies on page init or page load.

If more explicit control is required, we can change the module's behavior by changing the class level UsesInjection attribute.
```
[UsesInjection(For.ExplicitProperties)]
public partial class CustomerDetail : Page
{
    private ICustomerRepository customerRepository;

    [RequiredDependency]
    public ICustomerRepository CustomerRepository
    {
        get { return customerRepository; }
        set { customerRepository = value; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        customerGrid.DataSource = CustomerRepository.GetAll();
        customerGrid.DataBind();
    }
}
```
The For.ExplicitProperties does two things:

  1. Optimizes the scanning process to only consider explicitly tagged properties (the default behavior considers all public settable properties).
  1. An exception will be thrown if a dependency couldn't be found in the container to inject.

This makes is pretty evident if you forget to register a dependency with the container.
With the page level code in place we then need to setup our container and register our components. This is easily accomplished by implementing Castle.Windsor.IContainerAccessor in our global.asax and creating a static container with the registered components our web application needs to run.  Here we register to use an in memory customer repository implementation.
```
public class Global : System.Web.HttpApplication, IContainerAccessor
{
    private readonly IWindsorContainer container = new WindsorContainer();

    public Global()
    {
        container.Register(
            Component.For<ICustomerRepository>()
                .ImplementedBy<InMemoryCustomerRepository>());
    }

    public IWindsorContainer Container
    {
        get { return container; }
    }
}
```
We do this in the global.asax because we share the container between all requests, and thus only want one instance of it. Also, the IContainerAcessor is used as a service locator which is required by the AspNetWindsor http module to find your container instance which it will use to resolve dependencies.

The very last thing we need to do is register the AspNetWindsor http module in our web.config, just like any other module.
```
<add name="WindsorModule" type="Sneal.AspNetWindsorIntegration.AspNetDependencyBuilderModule, Sneal.AspNetWindsorIntegration"/>
```
From here on out, all ASP.NET requests get intercepted by this module and "enriched" by the container as long as they are tagged with the UsesInjection page level attribute.