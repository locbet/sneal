namespace Stormwind.Core.Models
{
    /// <summary>
    /// Represents an interactive user in the Stormwind system.
    /// </summary>
    public class User : Entity<User>
    {
        /// <summary>
        /// The user's first name.
        /// </summary>
        public virtual string FirstName { get; set; }

        /// <summary>
        /// The user's last name.
        /// </summary>
        public virtual string LastName { get; set; }

        /// <summary>
        /// The unique email address used to login with.
        /// </summary>
        public virtual string EmailAddress { get; protected set; }

        /// <summary>
        /// The encrypted password.
        /// </summary>
        public virtual string Password { get; protected set; }

        /// <summary>
        /// The salt string used to encrypt the password.
        /// </summary>
        public virtual string Salt { get; protected set; }

		protected User() { }

        public User(string emailAddress)
        {
            EmailAddress = emailAddress;
        }

        /// <summary>
        /// Sets the user's password.
        /// </summary>
        /// <param name="salt">The salt used during the password hash</param>
        /// <param name="encryptedPassword">The hashed password</param>
        public virtual void SetEncryptedPassword(string salt, string encryptedPassword)
        {
            Salt = salt;
            Password = encryptedPassword;
        }

        /// <summary>
        /// The user's entire name, first and last.
        /// </summary>
        public virtual string FullName
        {
            get { return FirstName + @" " + LastName; }
        }
    }
}