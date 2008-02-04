using System;
using System.Runtime.Serialization;

namespace Sneal.SqlMigration
{
    /// <summary>
    /// Base exception type for SqlMigration tool.
    /// </summary>
    public class SqlMigrationException : ApplicationException
    {
        public SqlMigrationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public SqlMigrationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public SqlMigrationException(string message) : base(message)
        {
        }

        public SqlMigrationException()
        {
        }
    }
}