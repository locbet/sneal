using System;
using Stormwind.Core.Models;
using Stormwind.Core.Repositories;
using Stormwind.Resources;

namespace Stormwind.Core.Security
{
    /// <summary>
    /// Application service for registering a new user.
    /// </summary>
    public class UserRegistrationService
    {
    	private readonly IPasswordHashService _passwordHashService;
    	private readonly IUserRepository _userRepository;

        public UserRegistrationService(IUserRepository userRepository, IPasswordHashService passwordHashService)
        {
        	_passwordHashService = passwordHashService;
			_userRepository = userRepository;
        }

    	/// <summary>
        /// Attempts to register a new user with the given email address and
        /// password. If successful a new user instance is created, otherwise
        /// and error result is returned.
        /// </summary>
        /// <param name="email">The new user's unique email address</param>
        /// <param name="clearTextPassword">The password the user would like to use</param>
        /// <returns>Success or failure response.</returns>
        public virtual UserRegistrationResult RegisterNewUser(string email, string clearTextPassword)
        {
            return !IsEmailAddressAvailable(email) ?
                UserRegistrationResult.Error(Errors.EmailAddressAlreadyTaken) :
				UserRegistrationResult.Success(CreateNewUser(email, clearTextPassword));
        }

        private bool IsEmailAddressAvailable(string email)
        {
			var existingUser = _userRepository.GetByFilter(o => o.EmailAddress == email);
            return existingUser == default(User);
        }

        private User CreateNewUser(string email, string clearTextPassword)
        {
			var newUser = new User(email);
        	SetUsersPassword(newUser, clearTextPassword);
			return _userRepository.Put(newUser);
        }

		private void SetUsersPassword(User user, string clearTextPassword)
		{
			var salt = Guid.NewGuid().ToString("N");
			string hashedPassword = _passwordHashService.HashSaltAndPassword(
				salt, clearTextPassword);

			user.SetEncryptedPassword(salt, hashedPassword);			
		}
    }
}