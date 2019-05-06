using System.Collections.Generic;

namespace PresentationLayer.Model
{
    public class DatabaseItem
    {

        #region Constructors

        public DatabaseItem(string name)
        {
            Name = name;
        }

        #endregion Constructors

        readonly List<DatabaseCategoryItem> _categories = new List<DatabaseCategoryItem>();

        //public bool NeedsClaimed { get; set; }
        //public bool NeedsCheckin { get; set; }
        //public bool NeedsCodeReview { get; set; }
        //public bool HasDrillDown { get; set; }
        public string Name { get; private set; }

        public List<DatabaseCategoryItem> Categories
        {
            get
            {
                return _categories;
            }
        }
    }
}
