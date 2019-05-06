
using System;
using System.Linq;

namespace DataLibrary.DataModel.DTO
{
    public class DatabaseObject : object
    {
        private static string formattedSchema = "[{0}].[{1}].[{2}]";

        public string DatabaseName { get; set; }
        public string ObjectSchema { get; set; }
        public string Category { get; set; }
        public string ObjectName { get; set; }
        public DatabaseObjectTypeCode TypeCode { get; set; }
        public string FormattedName
        {
            get
            {
                return string.Format(formattedSchema, DatabaseName, ObjectSchema, ObjectName);
            }
        }
        public string TfsSql { get; set; }
        public string DbSql { get; set; }
        public bool IsUpToDate
        {
            get
            {
                return Enumerable.SequenceEqual(
                    TfsSql.Where(c => !char.IsWhiteSpace(c)).Select(char.ToUpperInvariant),
                    DbSql.Where(c => !char.IsWhiteSpace(c)).Select(char.ToUpperInvariant));
            }
        }
        public DateTime? LastDdlChange { get; set; }
        public DateTime? LastTfsChange { get; set; }

        public DatabaseObject CopyObject()
        {
            return (DatabaseObject)this.MemberwiseClone();
        }
    }

    public enum DatabaseObjectTypeCode
    {
        StoredProcedure,
        Table,
        View,
        Function
    }
}
