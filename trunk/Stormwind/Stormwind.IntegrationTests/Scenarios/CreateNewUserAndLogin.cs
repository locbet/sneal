using Autofac;
using NUnit.Framework;
using Stormwind.Core.Models;
using Stormwind.Core.Repositories;
using Stormwind.Core.Security;
using Stormwind.Infrastructure.Data;

namespace Stormwind.IntegrationTests.Scenarios
{
	[TestFixture]
	public class CreateNewUserAndLogin
	{
		[Test]
		public void Execute()
		{
			using (IContainer context = NewContext())
			{
				const string newUserEmail = "bill@email.com";
				const string newUserPassword = "secret";

				var userRegistrationService = context.Resolve<UserRegistrationService>();
				UserRegistrationResult registrationResult = userRegistrationService.RegisterNewUser(newUserEmail, newUserPassword);
				registrationResult.IsError.Should().Be.False();

				context.Resolve<IUnitOfWork>().Flush();

				User loadedNewUser = context.Resolve<IUserRepository>().Get(registrationResult.CreatedUser.Id);
				Assert.IsNotNull(loadedNewUser);

				var authenticationService = context.Resolve<AuthenticationService>();
				AuthenticationResult authResult = authenticationService.Authenticate(newUserEmail, newUserPassword);
				authResult.IsError.Should().Be.False();
			}
		}

		private static IContainer NewContext()
		{
			return AssemblySetup.ApplicationContainer.CreateInnerContainer();
		}
	}
}
