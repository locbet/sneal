using System;
using System.IO;
using Sneal.Preconditions;
using Sneal.SqlMigration.IO;

namespace Sneal.SqlMigration.Impl
{
    public class ScriptFile : IScriptFile
    {
        private readonly string path;
        private IFileSystem fileSystem = new FileSystemAdapter();

        public ScriptFile(string path)
        {
            this.path = path;
        }

        public IFileSystem FileSystem
        {
            get { return fileSystem; }
            set
            {
                Throw.If(value, "FileSystem").IsNull();
                fileSystem = value;
            }
        }

        #region IScriptFile Members

        public string Path
        {
            get { return path; }
        }

        public bool IsXml
        {
            get { return path.EndsWith(".xml", StringComparison.InvariantCultureIgnoreCase); }
        }

        public bool IsSql
        {
            get { return path.EndsWith(".sql", StringComparison.InvariantCultureIgnoreCase); }
        }

        public string GetContent()
        {
            return fileSystem.ReadToEnd(path);
        }

        public Stream GetContentStream()
        {
            return fileSystem.OpenFileStream(path, FileMode.Open);
        }

        #endregion
    }
}