using System;
using System.IO;
using Sneal.SqlMigration.IO;

namespace Sneal.SqlMigration.Impl
{
    /// <summary>
    /// Returns NVelocity template file paths.
    /// </summary>
    public class SqlServerTemplateManager : ISqlTemplateManager
    {
        private readonly string templateDirectory;
        private readonly IFileSystem fileSystem;

        public SqlServerTemplateManager(IFileSystem fileSystem, string templateDir)
        {
            this.fileSystem = fileSystem;
            templateDirectory = templateDir;
        }

        public SqlServerTemplateManager()
        {
            fileSystem = new FileSystemAdapter();
            templateDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates");

#if DEBUG
            templateDirectory = AppDomain.CurrentDomain.BaseDirectory + @"\..\..\..\Sneal.SqlMigration\Templates";
#endif
        }

        public string TemplateDirectory
        {
            get { return templateDirectory; }
        }

        public virtual string CreateTable
        {
            get { return GetTemplateFile("CreateTable.vm"); }
        }

        public virtual string DropTable
        {
            get { return GetTemplateFile("DropTable.vm"); }
        }

        public virtual string CreateColumn
        {
            get { return GetTemplateFile("AddColumn.vm"); }
        }

        public virtual string DropColumn
        {
            get { return GetTemplateFile("DropColumn.vm"); }
        }

        public virtual string AlterColumn
        {
            get { return GetTemplateFile("AlterColumn.vm"); }
        }

        public virtual string CreateForeignKey
        {
            get { return GetTemplateFile("CreateForeignKey.vm"); }
        }

        public virtual string DropForeignKey
        {
            get { return GetTemplateFile("DropForeignKey.vm"); }
        }

        public virtual string CreateIndex
        {
            get { return GetTemplateFile("CreateIndex.vm"); }
        }

        public virtual string DropIndex
        {
            get { return GetTemplateFile("DropIndex.vm"); }
        }

        public virtual string CreateSproc
        {
            get { return GetTemplateFile("CreateSproc.vm"); }
        }

        public virtual string DropSproc
        {
            get { return GetTemplateFile("DropSproc.vm"); }
        }

        public virtual string AlterSproc
        {
            get { return GetTemplateFile("AlterSproc.vm"); }
        }

        public virtual string CreateView
        {
            get { return GetTemplateFile("CreateView.vm"); }
        }

        public virtual string DropView
        {
            get { return GetTemplateFile("DropView.vm"); }
        }

        public virtual string AlterView
        {
            get { return GetTemplateFile("AlterView.vm"); }
        }

        protected virtual string GetTemplateFile(string fileName)
        {
            string templateFilePath = Path.Combine(templateDirectory, fileName);

            if (!fileSystem.Exists(templateFilePath))
                throw new FileNotFoundException(
                    string.Format("Could not find SQL script template {0}", templateFilePath));

            return fileName;
        }
    }
}