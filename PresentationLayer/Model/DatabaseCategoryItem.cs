using DataLayer.DTO;
using System.Collections.Generic;

namespace PresentationLayer.Model
{
    public class DatabaseCategoryItem
    {
        private readonly List<DatabaseObjectItem> _objects = new List<DatabaseObjectItem>();
        
        #region Constructors

        public DatabaseCategoryItem(string name, string databaseName)
        {
            Name = name;
            DatabaseName = databaseName;
        }

        public DatabaseCategoryItem(string name, string databaseName, DatabaseObjectTypeCode type)
        {
            Name = name;
            DatabaseName = databaseName;
            Type = type;
        }

        #endregion Constructors

        #region Members

        public string Name { get; private set; }
        public string DatabaseName { get; private set; }
        public DatabaseObjectTypeCode Type { get; private set; }

        public List<DatabaseObjectItem> Objects
        {
            get
            {
                return _objects;
            }
        }

        #endregion Members
    }
}
