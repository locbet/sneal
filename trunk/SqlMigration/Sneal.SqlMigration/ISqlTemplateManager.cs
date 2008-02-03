namespace Sneal.SqlMigration
{
    public interface ISqlTemplateManager
    {
        string TemplateDirectory { get; }

        string CreateTable { get; }

        string DropTable { get; }

        string CreateColumn { get; }

        string DropColumn { get; }

        string AlterColumn { get; }

        string CreateForeignKey { get; }

        string DropForeignKey { get; }

        string CreateIndex { get; }

        string DropIndex { get; }

        string CreateSproc { get; }

        string DropSproc { get; }

        string AlterSproc { get; }

        string CreateView { get; }

        string DropView { get; }

        string AlterView { get; }
    }
}