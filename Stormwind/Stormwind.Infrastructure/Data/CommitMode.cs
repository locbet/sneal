namespace Stormwind.Infrastructure.Data
{
	/// <summary>
	/// The UOW commit mode, which defaults to Explicit.
	/// </summary>
    public enum CommitMode
    {
        /// <summary>
        /// The UOW will only flush changes to the DB when Commit is explicitly 
        /// called.  This is the default mode.
        /// </summary>
        Explicit,

        /// <summary>
        /// The UOW will flush all changes to the DB on Dispose and on Commit.
        /// </summary>
        Implicit
    }
}