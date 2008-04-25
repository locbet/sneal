using System.IO;

namespace Sneal.SqlMigration.IO
{
    /// <summary>
    /// Interface to interact with files on disk.  This is essentially a
    /// mockable interface in front on System.IO.
    /// </summary>
    public interface IFileSystem
    {
        /// <summary>
        /// Checks whether a file or directory exists on disk.
        /// </summary>
        /// <param name="filePath">The directory or file path.</param>
        /// <returns>
        /// <c>true</c> if the file or dir exists, otherwise <c>false</c>.
        /// </returns>
        bool Exists(string filePath);

        /// <summary>
        /// Deletes the specified file or directory.
        /// </summary>
        /// <param name="filePath">The directory or file path.</param>
        void Delete(string filePath);

        /// <summary>
        /// Creates a directory on disk.
        /// </summary>
        /// <param name="dir">The full path to the directory.</param>
        void CreateDirectory(string dir);

        /// <summary>
        /// Sets file attributes on the specified file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="attrs">The attributes to set.</param>
        void SetFileAttributes(string filePath, FileAttributes attrs);

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

        /// <summary>
        /// Opens a file stream for reading or writing.
        /// </summary>
        /// <param name="filePath">The path to the file.</param>
        /// <param name="fileMode">The mode to open the file under.</param>
        /// <returns>An open file stream.</returns>
        Stream OpenFileStream(string filePath, FileMode fileMode);
    }
}