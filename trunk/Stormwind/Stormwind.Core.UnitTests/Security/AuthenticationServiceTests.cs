using NUnit.Framework;
using Rhino.Mocks;
using Stormwind.Core.Models;
using Stormwind.Core.Repositories;
using Stormwind.Core.Security;
using Stormwind.TestUtils;

namespace Stormwind.Core.UnitTests.Security
{
    [TestFixture]
    public class AuthenticationServiceTests
    {
        private AuthenticationService AuthenticationService { get; set; }
        private IUserRepository UserRepository { get; set; }
        private IPasswordHashService PasswordHashService { get; set; }

        [Test]
        public void Authenticate_should_authenticate_user_with_matching_email_and_password()
        {
            CreateServices();
            CreateUserWithPassword("sneal@sneal.net", "secret");
            PasswordHashService.Stub(o => o.HashSaltAndPassword("salt", "secret")).Return("secret");
            AuthenticationResult result = AuthenticationService.Authenticate("sneal@sneal.net", "secret");
            result.IsError.Should().Be.False();
        }

        [Test]
        public void Authenticate_should_deny_user_without_registered_email_address()
        {
            CreateServices();
            PasswordHashService.Stub(o => o.HashSaltAndPassword("salt", "secret")).Return("$%^UW");
            AuthenticationResult result = AuthenticationService.Authenticate("sneal@sneal.net", "secret");
            result.IsError.Should().Be.True();
        }

        [Test]
        public void Authenticate_should_deny_user_without_correct_password()
        {
            CreateServices();
            CreateUserWithPassword("sneal@sneal.net", "secret");
            PasswordHashService.Stub(o => o.HashSaltAndPassword("salt", "secret")).Return("nonmatchingpasswrod");
            AuthenticationResult result = AuthenticationService.Authenticate("sneal@sneal.net", "secret");
            result.IsError.Should().Be.True();
        }

        private void CreateUserWithPassword(string emailAddress, string secret)
        {
            var user = UserRepository.Put(new User(emailAddress));
            user.SetEncryptedPassword("salt", secret);
        }

        private void CreateServices()
        {
            UserRepository = new InMemoryUserRepository();
            PasswordHashService = MockRepository.GenerateMock<IPasswordHashService>();
            AuthenticationService = new AuthenticationService(UserRepository, PasswordHashService);
        }
    }
}