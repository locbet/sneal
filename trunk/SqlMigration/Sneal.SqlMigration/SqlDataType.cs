namespace Sneal.SqlMigration
{
    public class SqlDataType
    {
        public static readonly SqlDataType Bit = new SqlDataType("BIT");
        public static readonly SqlDataType Char = new SqlDataType("CHAR", 10);
        public static readonly SqlDataType DateTime = new SqlDataType("DATETIME");
        public static readonly SqlDataType Int = new SqlDataType("INT");
        public static readonly SqlDataType Money = new SqlDataType("MONEY");
        public static readonly SqlDataType NChar = new SqlDataType("NCHAR", 10);
        public static readonly SqlDataType NVarChar = new SqlDataType("NVARCHAR", 50);
        public static readonly SqlDataType SmallDateTime = new SqlDataType("SMALLDATETIME");
        public static readonly SqlDataType SmallMoney = new SqlDataType("SMALLMONEY");
        public static readonly SqlDataType Text = new SqlDataType("TEXT");
        public static readonly SqlDataType TimeStamp = new SqlDataType("TIMESTAMP");
        public static readonly SqlDataType VarChar = new SqlDataType("VARCHAR", 50);

        private string dataType;
        private int length = 0;

        public SqlDataType() {}

        protected SqlDataType(string dataType)
        {
            this.dataType = dataType;
        }

        protected SqlDataType(string dataType, int length)
        {
            this.dataType = dataType;
            this.length = length;
        }

        public int Length
        {
            get { return length; }
            set { length = value; }
        }

        public string Name
        {
            get { return dataType; }
            set { dataType = value; }
        }

        public string ToScript()
        {
            if (length > 0)
                return string.Format("[{0}] ({1})", dataType, length);
            else
                return string.Format("[{0}]", dataType);
        }
    }
}