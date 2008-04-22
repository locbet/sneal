using System;
using MyMeta;

namespace Sneal.SqlMigration.Migrators
{
    public static class ExecutorFactory
    {
        public static IExecutor CreateExecutor(IDatabase db, IScriptFile script)
        {
            if (script.IsXml)
            {
                if (db.Root.DriverString != "SQL")
                {
                    string msg = "Loading of XML data is not currently supported " + 
                        "by SqlMigration for your RDMS.";
                    throw new NotSupportedException(msg);                    
                }

                return new SqlServerBulkXmlExecutor();
            }
            else
            {
                if (db.Root.DriverString == "SQL")
                {
                    return new SqlServerBatchExecutor();
                }
                else
                {
                    return new BatchExecutor();
                }
            }
        }
    }
}
