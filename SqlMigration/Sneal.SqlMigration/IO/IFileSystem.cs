using System.IO;
using System.Text;

namespace Sneal.SqlMigration.IO
{
    /// <summary>
    /// Interface to interact with files on disk.  This is essentially a
    /// mockable interface in front on System.IO.
    /// </summary>
    public interface IFileSystem
    {
        bool Exists(string filePath);
        void Delete(string filePath);

        /// <summary>
        /// Generates a unique temporary file name and path.
        /// </summary>
        /// <returns>A temp file path.</returns>
        string GetTempFileName();

        /// <summary>
        /// Reads an entire text file from disk into a string.
        /// </summary>
        /// <param name="filePath">The full path to the file.</param>
        /// <returns>The file contents in their entirety.</returns>
        string ReadToEnd(string filePath);

        /// <summary>
        /// Opens a text file for writing. <seealso cref="System.IO.StreamWriter"/>
        /// </summary>
        /// <param name="filePath">The full path to the text file.</param>
        /// <returns>An open text writer.</returns>
        TextWriter OpenFileForWriting(string filePath);
    }
}
