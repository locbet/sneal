using System.Xml.Schema;
using MyMeta;

namespace Sneal.SqlMigration.Migrators
{
    /// <summary>
    /// Generates an XML schema from the specified table meta data.
    /// </summary>
    public interface ITableXsdGenerator
    {
        XmlSchema GenerateXsd(ITable table);
    }
}