using System.Data.OleDb;
using MyMeta;

namespace Sneal.SqlMigration.Utils
{
    public static class DataTypeUtil
    {
        public static bool IsDateTime(IColumn col)
        {
            switch ((OleDbType) col.DataType)
            {
                case OleDbType.DBTimeStamp:
                case OleDbType.Date:
                case OleDbType.DBDate:
                case OleDbType.DBTime:
                case OleDbType.Filetime:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsString(IColumn col)
        {
            switch ((OleDbType) col.DataType)
            {
                case OleDbType.BSTR:
                case OleDbType.Char:
                case OleDbType.LongVarChar:
                case OleDbType.LongVarWChar:
                case OleDbType.VarChar:
                case OleDbType.VarWChar:
                case OleDbType.WChar:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsBoolean(IColumn col)
        {
            return ((OleDbType) col.DataType == OleDbType.Boolean);
        }

        public static bool IsNumeric(IColumn col)
        {
            switch ((OleDbType) col.DataType)
            {
                case OleDbType.BigInt:
                case OleDbType.Currency:
                case OleDbType.Decimal:
                case OleDbType.Double:
                case OleDbType.Integer:
                case OleDbType.Numeric:
                case OleDbType.Single:
                case OleDbType.SmallInt:
                case OleDbType.TinyInt:
                case OleDbType.UnsignedBigInt:
                case OleDbType.UnsignedInt:
                case OleDbType.UnsignedSmallInt:
                case OleDbType.UnsignedTinyInt:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// SQL to XML data type mappings:
        /// <see cref="http://msdn2.microsoft.com/en-us/library/aa258643(SQL.80).aspx"/>
        /// </summary>
        public static string ToXmlDataType(IColumn col)
        {
            switch ((OleDbType) col.DataType)
            {
                case OleDbType.BigInt:
                case OleDbType.UnsignedBigInt:
                    return "long";
                case OleDbType.TinyInt:
                case OleDbType.UnsignedTinyInt:
                    return "unsignedByte";
                case OleDbType.SmallInt:
                case OleDbType.Integer:
                case OleDbType.UnsignedSmallInt:
                case OleDbType.UnsignedInt:
                    return "int";
                case OleDbType.Single:
                case OleDbType.Currency:
                case OleDbType.Decimal:
                case OleDbType.Double:
                case OleDbType.Numeric:
                    return "decimal";
                case OleDbType.Boolean:
                    return "boolean";
                case OleDbType.DBTimeStamp:
                case OleDbType.Date:
                case OleDbType.DBDate:
                case OleDbType.DBTime:
                case OleDbType.Filetime:
                    return "dateTime";
                case OleDbType.VarBinary:
                case OleDbType.LongVarBinary:
                case OleDbType.Binary:
                    return "base64Binary";
                default:
                    return "string";
            }
        }
    }
}