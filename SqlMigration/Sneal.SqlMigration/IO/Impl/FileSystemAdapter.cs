using System.IO;
using Sys = System.IO;

namespace Sneal.SqlMigration.IO.Impl
{
    /// <summary>
    /// Wrapper around System.IO.FileSystemAdapter functions.
    /// </summary>
    public class FileSystemAdapter : IFileSystem
    {
        public bool Exists(string filePath)
        {
            return Sys.File.Exists(filePath);
        }

        public void Delete(string filePath)
        {
            Sys.File.Delete(filePath);
        }

        public string ReadToEnd(string filePath)
        {
            if (!Sys.File.Exists(filePath))
                throw new FileNotFoundException(
                    string.Format("Could not find the file {0}", filePath));

            using (StreamReader reader = new StreamReader(filePath))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
