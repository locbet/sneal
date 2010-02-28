using Stormwind.Core.Models;
using Stormwind.Core.Repositories;
using Stormwind.Resources;

namespace Stormwind.Core.Security
{
    /// <summary>
    /// Default service for authenticating existing users.
    /// </summary>
    public class AuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHashService _passwordHashService;

        public AuthenticationService(IUserRepository userRepository, IPasswordHashService passwordHashService)
        {
            _userRepository = userRepository;
            _passwordHashService = passwordHashService;
        }

        /// <summary>
        /// Authenticates the given email address and password against existing users.
        /// </summary>
        /// <param name="emailAddress">The user's email address</param>
        /// <param name="clearTextPassword">The user's clear text password</param>
        /// <returns>The authentication result</returns>
        public AuthenticationResult Authenticate(string emailAddress, string clearTextPassword)
        {
            User user = _userRepository.GetByFilter(u => u.EmailAddress == emailAddress);
            if (user != default(User))
            {
                string hashedPassword = _passwordHashService.HashSaltAndPassword(user.Salt, clearTextPassword);
                if (hashedPassword == user.Password)
                {
                    // FormsAuthentication.SetAuthCookie(user.EmailAddress, false);
                    return AuthenticationResult.Success(user);
                }
            }
            return AuthenticationResult.Error(Errors.EmailAddressOrPasswordIsIncorrect);
        }

        public void SignOut()
        {
//            FormsAuthentication.SignOut();
        }
    }
}