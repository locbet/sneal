using System.Collections.Generic;
using Stormwind.Core.Models;

namespace Stormwind.Core.Security
{
    public class AuthenticationResult
    {
        public List<string> Errors { get; private set; }
        public User AuthenticatedUser { get; private set; }

        public static AuthenticationResult Success(User authenticatedUser)
        {
            return new AuthenticationResult { AuthenticatedUser = authenticatedUser };
        }

        public static AuthenticationResult Error(string error)
        {
            return Error(new[] { error });
        }

        public static AuthenticationResult Error(IEnumerable<string> errors)
        {
            return new AuthenticationResult { Errors = new List<string>(errors) };
        }

        public bool IsError
        {
            get { return Errors != null && Errors.Count > 0; }
        }
    }
}