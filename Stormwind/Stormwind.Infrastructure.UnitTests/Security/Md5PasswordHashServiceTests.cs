using System;
using NUnit.Framework;
using Stormwind.Infrastructure.Security;

namespace Stormwind.Infrastructure.UnitTests.Security
{
    [TestFixture]
    public class Md5PasswordHashServiceTests
    {
        public Md5PasswordHashService HashService { get; set; }

        [Test]
        public void HashSaltAndPassword_should_encrypt_password()
        {
            CreatePasswordHashService();
            string hash = HashService.HashSaltAndPassword("salt", "secret");
            hash.Should().Not.Be.EqualTo("salt");
            hash.Should().Not.Be.EqualTo("secret");
            hash.Should().Not.Be.EqualTo("saltsecret");
        }

        [Test]
        public void HashSaltAndPassword_should_trim_white_space()
        {
            CreatePasswordHashService();
            string secretWithWhiteSpace = HashService.HashSaltAndPassword("salt", "secret ");
            string secretNoWhiteSpace = HashService.HashSaltAndPassword("salt", "secret");
            secretNoWhiteSpace.Should().Be.EqualTo(secretWithWhiteSpace);
        }

        [Test]
        public void HashSaltAndPassword_should_create_different_hashes_when_salt_varies()
        {
            CreatePasswordHashService();
            string hash = HashService.HashSaltAndPassword("salt", "secret");
            string hash2 = HashService.HashSaltAndPassword("salt2", "secret");
            hash.Should().Not.Be.EqualTo(hash2);
        }

        [Test]
        public void HashSaltAndPassword_should_create_different_hashes_when_password_varies()
        {
            CreatePasswordHashService();
            string hash = HashService.HashSaltAndPassword("salt", "secret");
            string hash2 = HashService.HashSaltAndPassword("salt", "secret2");
            hash.Should().Not.Be.EqualTo(hash2);
        }

        [Test]
        public void HashSaltAndPassword_should_create_the_same_hashes_when_password_and_salt_are_the_same()
        {
            CreatePasswordHashService();
            string hash = HashService.HashSaltAndPassword("salt", "secret");
            CreatePasswordHashService();
            string hash2 = HashService.HashSaltAndPassword("salt", "secret");
            hash.Should().Be.EqualTo(hash2);
        }

        [Test]
        public void HashSaltAndPassword_should_throw_when_null_or_empty_salt()
        {
            CreatePasswordHashService();
            Assert.Throws<ArgumentException>(() => HashService.HashSaltAndPassword("", "secret"));
            Assert.Throws<ArgumentException>(() => HashService.HashSaltAndPassword(null, "secret"));
        }

        [Test]
        public void HashSaltAndPassword_should_throw_when_null_or_empty_password()
        {
            CreatePasswordHashService();
            Assert.Throws<ArgumentException>(() => HashService.HashSaltAndPassword("salt", ""));
            Assert.Throws<ArgumentException>(() => HashService.HashSaltAndPassword("salt", null));
        }

        private void CreatePasswordHashService()
        {
            HashService = new Md5PasswordHashService();
        }
    }
}