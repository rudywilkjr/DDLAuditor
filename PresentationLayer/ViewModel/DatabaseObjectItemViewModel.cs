using DataLayer.DTO;
using DataLayer.Repository;
using PresentationLayer.Model;
using PresentationLayer.Utility;
using PresentationLayer.Views.UserControls;
using System.Windows.Input;

namespace PresentationLayer.ViewModel
{
    public class DatabaseObjectItemViewModel : TreeViewItemViewModel
    {
        readonly DatabaseObjectItem _databaseObjectItem;
        
        public DatabaseObjectItemViewModel(DatabaseObjectItem databaseObjectItem, DatabaseCategoryItemViewModel parentCategory)
            : base (parentCategory, false)
        {
            _databaseObjectItem = databaseObjectItem;
        }

        public ICommand ShowDatabaseScriptCommand => new DelegateCommand(ShowDatabaseScript);

        public bool HasPendingCheckin => _databaseObjectItem.HasPendingCheckin;

        public bool HasUnclaimedChanges => _databaseObjectItem.HasUnclaimedChanges;

        public string Name => _databaseObjectItem.Name;

        public string SchemaName => _databaseObjectItem.SchemaName;

        public string DatabaseName => _databaseObjectItem.DatabaseName;

        public string SchemaPrefixedName =>
            $"[{_databaseObjectItem.DatabaseName}].[{_databaseObjectItem.SchemaName}].[{_databaseObjectItem.Name}]";

        public DatabaseObjectTypeCode Type => _databaseObjectItem.ItemType;

        public void ShowDatabaseScript(object obj)
        {
            var model = (DatabaseObjectItemViewModel)obj;
            var dbObject = new DatabaseObject
            {
                DatabaseName = model.DatabaseName,
                ObjectSchema = model.SchemaName,
                ObjectName = model.Name,
                TypeCode = model.Type
            };

            var utility = new UtilityRepository();
            utility.GetSmoObject(dbObject);
            ShowScript(dbObject.DbSql, $"Source {"Database"}: {dbObject.FormattedName}");
        }

        private void ShowScript(string script, string title)
        {
            var viewer = new ScriptViewer
            {
                TextScript = {Text = script},
                Title = title
            };
            viewer.Show();
        }
    }
}
