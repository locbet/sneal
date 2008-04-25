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
            if (!Path.HasExtension(filePath))
                return Directory.Exists(filePath);

            return File.Exists(filePath);
        }

        public void Delete(string filePath)
        {
            if (!Path.HasExtension(filePath))
                Directory.Delete(filePath);
            else
                File.Delete(filePath);
        }

        public void CreateDirectory(string dir)
        {
            Directory.CreateDirectory(dir);
        }

        public void SetFileAttributes(string filePath, FileAttributes attrs)
        {
            File.SetAttributes(filePath, attrs);
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

        public Stream OpenFileStream(string filePath, FileMode fileMode)
        {
            return new FileStream(filePath, fileMode);
        }

        #endregion
    }
}