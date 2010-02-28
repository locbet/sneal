namespace Stormwind.Core.Security
{
    /// <summary>
    /// Service for hashing user's clear text passwords.
    /// </summary>
    public interface IPasswordHashService
    {
        string HashSaltAndPassword(string salt, string clearTextPassword);
    }
}