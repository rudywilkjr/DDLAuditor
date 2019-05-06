using System;

namespace DataTracker.Model
{
    public class IgnoredObjectModel : DatabaseObjectItem
    {
        public int Id { get; set; }
        public DateTime IgnoredOnTime { get; set; }

        public string IgnoredByUser { get; set; }

        public IgnoredObjectModel(int id, string name, string schemaName, string databaseName, DateTime ignoredDateTime, string ignoredByUserName)
        {
            Id = id;
            Name = name;
            SchemaName = schemaName;
            DatabaseName = databaseName;
            IgnoredOnTime = ignoredDateTime;
            IgnoredByUser = ignoredByUserName;
        }
    }
}
