using NUnit.Framework;
using Stormwind.Core.Models;

namespace Stormwind.Core.UnitTests.Model
{
    [TestFixture]
    public class UserTests
    {
        public User User { get; set; }

        [Test]
        public void SetEncryptedPassword_should_set_salt_and_password()
        {
            CreateUser();
            User.SetEncryptedPassword("salt", "hash");
            User.Salt.Should().Be.EqualTo("salt");
            User.Password.Should().Be.EqualTo("hash");
        }

        private void CreateUser()
        {
            User = new User("bob@email.com");
        }
    }
}
