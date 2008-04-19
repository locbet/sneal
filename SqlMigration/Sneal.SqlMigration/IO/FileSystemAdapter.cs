using System.IO;
using System.Text;
using Sys = System.IO;

namespace Sneal.SqlMigration.IO
{
    /// <summary>
    /// IFileSystem implmentation that wraps System.IO methods.
    /// </summary>
    public class FileSystemAdapter : IFileSystem
    {
        #region IFileSystem Members

        public bool Exists(string filePath)
        {
            return File.Exists(filePath);
        }

        public void Delete(string filePath)
        {
            File.Delete(filePath);
        }

        public string GetTempFileName()
        {
            return Path.GetTempFileName();
        }

        public string ReadToEnd(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException(
                    string.Format("Could not find the file {0}", filePath));

            using (StreamReader reader = new StreamReader(filePath))
            {
                return reader.ReadToEnd();
            }
        }

        public TextWriter OpenFileForWriting(string filePath)
        {
            return new StreamWriter(filePath);
        }

        #endregion
    }
}