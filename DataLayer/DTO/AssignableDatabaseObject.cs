using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DataLayer.DTO
{
    public class AssignableDatabaseObject : INotifyPropertyChanged
    {
        private bool _isAssign;

        #region "NotifyPropertyChanged"
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public bool IsAssign
        {
            get { return _isAssign; }
            set { _isAssign = value; RaisePropertyChanged(); }
        }
        public string DatabaseName { get; set; }
        public string ObjectSchema { get; set; }
        public string Category { get; set; }
        public string ObjectName { get; set; }
        public string DbSql { get; set; }

        public AssignableDatabaseObject(DatabaseObject databaseObject)
        {
            DatabaseName = databaseObject.DatabaseName;
            ObjectName = databaseObject.ObjectName;
            ObjectSchema = databaseObject.ObjectSchema;
            DbSql = databaseObject.DbSql;
            switch (databaseObject.Category)
            {
                case "COLUMN": case "DEFAULT": case "INDEX": case "PRIMARY KEY CONSTRAINT": case "TABLE":
                    Category = "TABLE";
                    break;
                case "FUNCTION":
                    Category = databaseObject.Category;
                    break;
                case "PROCEDURE":
                    Category = databaseObject.Category;
                    break;
                case "VIEW":
                    Category = databaseObject.Category;
                    break;
                default:
                    Category = "UNKNOWN";
                    break;
            }
        }
    }
}
