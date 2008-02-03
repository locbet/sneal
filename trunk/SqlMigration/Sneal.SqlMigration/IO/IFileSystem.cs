namespace Sneal.SqlMigration.IO
{
    /// <summary>
    /// Interface to interact with files on disk.
    /// </summary>
    public interface IFileSystem
    {
        bool Exists(string filePath);
        void Delete(string filePath);
        string ReadToEnd(string filePath);
    }
}
