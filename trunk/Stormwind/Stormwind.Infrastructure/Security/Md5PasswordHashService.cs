using System.Security.Cryptography;
using System.Text;
using Sneal.Core;
using Stormwind.Core.Security;

namespace Stormwind.Infrastructure.Security
{
    /// <summary>
    /// Default IPasswordHashService implementation which uses MD5 for encryption.
    /// </summary>
    public class Md5PasswordHashService : IPasswordHashService
    {
        private readonly MD5 _md5Algorithm = MD5.Create();

        public string HashSaltAndPassword(string salt, string clearTextPassword)
        {
            Guard.AgainstNullOrEmpty(salt, "Salt cannot be null or empty");
            Guard.AgainstNullOrEmpty(clearTextPassword, "ClearTextPassword cannot be null or empty");
            return Hash(salt.Trim() + clearTextPassword.Trim());
        }

        private string Hash(string stringToHash)
        {
            byte[] rawHash = _md5Algorithm.ComputeHash(Encoding.UTF8.GetBytes(stringToHash));
            return Encoding.UTF8.GetString(rawHash);
        }
    }
}