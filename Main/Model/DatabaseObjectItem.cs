using DataLibrary.DataModel.DTO;

namespace DataTracker.Model
{
    public class DatabaseObjectItem
    {
        public string Name { get; set; }
        public string SchemaName { get; set; }
        public string DatabaseName { get; set; }
        public DatabaseObjectTypeCode ItemType { get; set; }

        public bool HasUnclaimedChanges { get; set; }
        public bool HasPendingCheckin { get; set; }
        public bool NeedsCodeReview { get; set; }

        public DatabaseObjectItem(string name, string schemaName, string databaseName, DatabaseObjectTypeCode type)
        {
            Name = name;
            SchemaName = schemaName;
            DatabaseName = databaseName;
            ItemType = type;
            HasUnclaimedChanges = false;
            HasPendingCheckin = false;
            NeedsCodeReview = false;
        }

        public DatabaseObjectItem()
        {
            
        }
    }
}
