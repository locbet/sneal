using NUnit.Framework;
using Rhino.Mocks;
using Stormwind.Core.Models;
using Stormwind.Core.Repositories;
using Stormwind.Core.Security;
using Stormwind.TestUtils;

namespace Stormwind.Core.UnitTests.Security
{
    [TestFixture]
    public class UserRegistrationServiceTests
    {
        private UserRegistrationService Service { get; set; }
        private IUserRepository UserRepository { get; set; }
		private IPasswordHashService PasswordHashService { get; set; }

        [Test]
        public void RegisterNewUser_should_return_error_when_email_address_already_taken()
        {
            CreateUserRegistrationService();
            AddUserToRepository("taken@email.com");
            UserRegistrationResult result = Service.RegisterNewUser("taken@email.com", "password");
            result.IsError.Should().Be.True();
        }

        [Test]
        public void RegisterNewUser_should_return_success_when_email_address_available()
        {
            CreateUserRegistrationService();
            UserRegistrationResult result = Service.RegisterNewUser("available@email.com", "password");
            result.IsError.Should().Be.False();
        }

        [Test]
        public void RegisterNewUser_should_return_new_user_instance_when_successful()
        {
            CreateUserRegistrationService();
            UserRegistrationResult result = Service.RegisterNewUser("available@email.com", "password");
            Assert.IsNotNull(result.CreatedUser);
            result.CreatedUser.EmailAddress.Should().Be.EqualTo("available@email.com");
        }

		[Test]
		public void RegisterNewUser_should_create_salt_and_hash_password()
		{
			CreateUserRegistrationService();
			PasswordHashService.Stub(o => o.HashSaltAndPassword(null, null))
				.IgnoreArguments()
				.Return("hashedpwd");

			UserRegistrationResult result = Service.RegisterNewUser("available@email.com", "password");

			result.CreatedUser.Salt.Should().Not.Be.Null();
			result.CreatedUser.Salt.Should().Not.Be.Empty();
			result.CreatedUser.Password.Should().Be.EqualTo("hashedpwd");
		}

        private void CreateUserRegistrationService()
        {
			PasswordHashService = MockRepository.GenerateStub<IPasswordHashService>();
            UserRepository = new InMemoryUserRepository();
			Service = new UserRegistrationService(UserRepository, PasswordHashService);
        }

        private void AddUserToRepository(string emailAddress)
        {
            UserRepository.Put(new User(emailAddress));
        }
    }
}