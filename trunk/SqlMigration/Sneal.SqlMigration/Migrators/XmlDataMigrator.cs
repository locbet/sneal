using System;
using System.Collections.Generic;
using System.Text;

namespace Sneal.SqlMigration.Migrators
{
    /// <summary>
    /// Turns table data into a SQL Server compatable bulk load XML document.
    /// </summary>
    public class XmlDataMigrator
    {
        // shit, this needs to write out the XML document AND the XSD
        // using a single file scripter would be dumb.

        // can't we just generate the XSD at bulk load time?
    }
}
