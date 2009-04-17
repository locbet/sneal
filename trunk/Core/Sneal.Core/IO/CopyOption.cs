namespace Sneal.Core.IO
{
    /// <summary>
    /// File and directory action options.
    /// </summary>
    public enum CopyOption
    {
        /// <summary>
        /// Do not overwrite the target if it exists.
        /// </summary>
        Overwrite,

        /// <summary>
        /// If the target exists, overwrite it.
        /// </summary>
        DoNotOverwrite
    }
}