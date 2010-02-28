using System.Collections.Generic;
using Stormwind.Core.Models;

namespace Stormwind.Core.Security
{
    /// <summary>
    /// User registration service response.
    /// </summary>
    public class UserRegistrationResult
    {
        public List<string> Errors { get; private set; }
        public User CreatedUser { get; private set; }

        public static UserRegistrationResult Success(User createdUser)
        {
            return new UserRegistrationResult {CreatedUser = createdUser};
        }

        public static UserRegistrationResult Error(string error)
        {
            return Error(new[] {error});
        }

        public static UserRegistrationResult Error(IEnumerable<string> errors)
        {
            return new UserRegistrationResult { Errors = new List<string>(errors) };
        }

        public bool IsError
        {
            get { return Errors != null && Errors.Count > 0; }
        }
    }
}