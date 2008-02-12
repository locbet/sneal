using System;
using System.IO;
using MyMeta;
using NVelocity;
using NVelocity.App;
using NVelocity.Runtime;

namespace Sneal.SqlMigration.Impl
{
    public class SqlServerScriptBuilder : IScriptBuilder
    {
        private readonly VelocityEngine velocityEngine = new VelocityEngine();
        private readonly ISqlTemplateManager templateManager;
        private static readonly SqlServerScriptHelper serverScriptHelper = new SqlServerScriptHelper();

        public SqlServerScriptBuilder(ISqlTemplateManager templateManager)
        {
            this.templateManager = templateManager;
            InitializeVelocity();
        }

        public SqlServerScriptBuilder()
        {
            templateManager = new SqlServerTemplateManager();
            InitializeVelocity();
        }

        private void InitializeVelocity()
        {
            velocityEngine.SetProperty(RuntimeConstants.RUNTIME_LOG_LOGSYSTEM_CLASS, "NVelocity.Runtime.Log.NullLogSystem");
            velocityEngine.SetProperty(RuntimeConstants.FILE_RESOURCE_LOADER_PATH, templateManager.TemplateDirectory);
            velocityEngine.SetProperty(RuntimeConstants.RESOURCE_MANAGER_CLASS, "NVelocity.Runtime.Resource.ResourceManagerImpl");
            velocityEngine.Init();
        }

        #region IScriptBuilder Members

        public virtual SqlScript Create(ITable table)
        {
            VelocityContext context = new VelocityContext();
            context.Put("table", table);
            return CreateScript(templateManager.CreateTable, context);
        }

        public virtual SqlScript Drop(ITable table)
        {
            VelocityContext context = new VelocityContext();
            context.Put("table", table);
            return CreateScript(templateManager.DropTable, context);
        }

        public virtual SqlScript Create(IColumn column)
        {
            VelocityContext context = new VelocityContext();
            context.Put("column", column);
            return CreateScript(templateManager.CreateColumn, context);
        }

        public virtual SqlScript Drop(IColumn column)
        {
            VelocityContext context = new VelocityContext();
            context.Put("column", column);
            return CreateScript(templateManager.DropColumn, context);
        }

        public virtual SqlScript Alter(IColumn column)
        {
            VelocityContext context = new VelocityContext();
            context.Put("column", column);
            return CreateScript(templateManager.AlterColumn, context);
        }

        public virtual SqlScript Create(IIndex index)
        {
            VelocityContext context = new VelocityContext();
            context.Put("index", index);
            return CreateScript(templateManager.CreateIndex, context);
        }

        public virtual SqlScript Drop(IIndex index)
        {
            VelocityContext context = new VelocityContext();
            context.Put("index", index);
            return CreateScript(templateManager.DropIndex, context);
        }

        public virtual SqlScript Create(IForeignKey foreignKey)
        {
            VelocityContext context = new VelocityContext();
            context.Put("foreignkey", foreignKey);
            return CreateScript(templateManager.CreateForeignKey, context);
        }

        public virtual SqlScript Drop(IForeignKey foreignKey)
        {
            VelocityContext context = new VelocityContext();
            context.Put("foreignkey", foreignKey);
            return CreateScript(templateManager.DropForeignKey, context);
        }

        public virtual SqlScript Create(IProcedure sproc)
        {
            VelocityContext context = new VelocityContext();
            context.Put("sproc", sproc);
            return CreateScript(templateManager.CreateSproc, context);
        }

        public virtual SqlScript Drop(IProcedure sproc)
        {
            VelocityContext context = new VelocityContext();
            context.Put("sproc", sproc);
            return CreateScript(templateManager.DropSproc, context);
        }

        public virtual SqlScript Alter(IProcedure sproc)
        {
            VelocityContext context = new VelocityContext();
            context.Put("sproc", sproc);
            return CreateScript(templateManager.AlterSproc, context);
        }

        public virtual SqlScript Create(IView view)
        {
            VelocityContext context = new VelocityContext();
            context.Put("view", view);
            return CreateScript(templateManager.CreateView, context);
        }

        public virtual SqlScript Drop(IView view)
        {
            VelocityContext context = new VelocityContext();
            context.Put("view", view);
            return CreateScript(templateManager.DropView, context);
        }

        public virtual SqlScript Alter(IView view)
        {
            VelocityContext context = new VelocityContext();
            context.Put("view", view);
            return CreateScript(templateManager.AlterView, context);
        }

        #endregion

        protected virtual SqlScript CreateScript(string templatePath, VelocityContext context)
        {
            context.Put("time", DateTime.Now);
            context.Put("SqlScriptHelper", serverScriptHelper);

            StringWriter writer = new StringWriter();
            Template template = velocityEngine.GetTemplate(templatePath);
            template.Merge(context, writer);

            SqlScript script = new SqlScript();
            script.Append(writer.GetStringBuilder());
            return script;
        }
    }
}