
namespace DataTracker.Model
{
    public class SqlServerObjectType
    {
        public string TypeDescription { get; set; }

        public string TypeCode { get; set; }

        //This is the field which stores the same generalized term used in the DDLAudit table
        public string Category { get; set; }

        public SqlServerObjectType(string typeCode, string typeDescription)
        {
            TypeCode = typeCode;
            TypeDescription = typeDescription;
            switch (typeCode)
            {
                case "AF":
                case "FN":
                case "FS":
                case "IF":
                case "TF":
                    Category = "FUNCTION";
                    break;
                case "C":
                case "D":
                case "F":
                case "PK":
                case "UQ":
                case "U":
                    Category = "TABLE";
                    break;
                case "P":
                case "PC":
                case "X":
                    Category = "PROCEDURE";
                    break;
                case "V":
                    Category = "VIEW";
                    break;
                default:
                    Category = "UNKNOWN";
                    break;
            }
        }
    }
}
