using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Method_Stub_Generator
{
    public static class TypeConverter
    {

        public static string ToClrType(SqlDbType sqlType, bool nullType)
        {
            switch (sqlType)
            {
                case SqlDbType.BigInt:
                    return nullType ? "long?" : "long";

                case SqlDbType.Binary:
                case SqlDbType.Image:
                case SqlDbType.Timestamp:
                case SqlDbType.VarBinary:
                    return "byte[]";

                case SqlDbType.Bit:
                    return nullType ? "bool?" : "bool";

                case SqlDbType.Char:
                case SqlDbType.NChar:
                case SqlDbType.NText:
                case SqlDbType.NVarChar:
                case SqlDbType.Text:
                case SqlDbType.VarChar:
                case SqlDbType.Xml:
                    return "string";

                case SqlDbType.DateTime:
                case SqlDbType.SmallDateTime:
                case SqlDbType.Date:
                case SqlDbType.Time:
                case SqlDbType.DateTime2:
                    return nullType ? "DateTime?" : "DateTime";

                case SqlDbType.Decimal:
                case SqlDbType.Money:
                case SqlDbType.SmallMoney:
                    return nullType ? "decimal?" : "decimal";

                case SqlDbType.Float:
                    return nullType ? "double?" : "double";

                case SqlDbType.Int:
                    return nullType ? "int?" : "int";

                case SqlDbType.Real:
                    return nullType ? "float?" : "float";

                case SqlDbType.UniqueIdentifier:
                    return "Guid?";

                case SqlDbType.SmallInt:
                    return nullType ? "short?" : "short";

                case SqlDbType.TinyInt:
                    return nullType ? "byte?" : "byte";

                case SqlDbType.Variant:
                case SqlDbType.Udt:
                    return "object";

                case SqlDbType.Structured:
                    return "DataTable";

                case SqlDbType.DateTimeOffset:
                    return nullType ? "DateTimeOffset?" : "DateTimeOffset";

                default:
                    throw new ArgumentOutOfRangeException("sqlType");
            }
        }
    }
}
