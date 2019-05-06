using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Core.Helper;
using DataLayer.Repository;
using PresentationLayer.Model;
using PresentationLayer.Utility;
using System.Collections.ObjectModel;
using Core;
using ProjectLabel = PresentationLayer.Model.ProjectLabel;

namespace PresentationLayer.ViewModel
{
    public class ManualEntryViewModel : INotifyPropertyChanged
    {
        #region "NotifyPropertyChanged"
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void RaiseOtherPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public ICommand RefreshObjectsCommand => new DelegateCommand(RefreshObjects);
        public ICommand RefreshSchemasCommand => new DelegateCommand(RefreshSchemas);
        public ICommand SaveManualEntryCommand => new DelegateCommand(SaveManualEntry);

        #region "Events"
        public event EventHandler LinkObjectsChanged;
        #endregion

        #region "Fields"
        private readonly AuditLogRepository _auditLogRepository = new AuditLogRepository();
        private readonly UtilityRepository _utilityRepository = new UtilityRepository();
        private User _selectedDeveloper;
        private DateTime _releaseDate;
        private string[] _objectSchemaOptions;
        private string _selectedObjectSchema;
        private string[] _objectNameOptions;
        private string _selectedObjectName;
        private Database _selectedDatabase;
        private SqlServerObjectType _selectedSqlServerObjectType;
        private string _actionMessage;
        private bool _isActionError;
        private bool _isActionSomething;
        private ProjectLabel _label;
        private ObservableCollection<ProjectLabel> _labels;
        private readonly RogueProjectRepository _rogueProjectRepository = new RogueProjectRepository();
        #endregion

        public List<SqlServerObjectType> ObjectTypeOptions { get; set; }

        public List<ProjectLabel> ProjectLabels { get; set; }

        public List<Database> DatabaseOptions => Database.AllDatabases;

        public SqlServerObjectType SelectedSqlServerObjectType
        {
            get { return _selectedSqlServerObjectType; }
            set { _selectedSqlServerObjectType = value; RaisePropertyChanged(); }
        }

        public User SelectedDeveloper
        {
            get { return _selectedDeveloper; }
            set { _selectedDeveloper = value; RaisePropertyChanged(); }
        }

        public List<User> DeveloperOptions => User.AllApplicationUsers;

        public DateTime ReleaseDate
        {
            get { return _releaseDate; }
            set { _releaseDate = value; RaisePropertyChanged(); }
        }

        public Database SelectedDatabase
        {
            get { return _selectedDatabase; }
            set { _selectedDatabase = value; RaisePropertyChanged(); }
        }

        public string[] ObjectSchemaOptions
        {
            get { return _objectSchemaOptions; }
            set { _objectSchemaOptions = value; RaisePropertyChanged(); }
        }

        public string SelectedObjectSchema
        {
            get { return _selectedObjectSchema; }
            set { _selectedObjectSchema = value; RaisePropertyChanged(); }
        }

        public string[] ObjectNameOptions
        {
            get { return _objectNameOptions; }
            set { _objectNameOptions = value; RaisePropertyChanged(); }
        }

        public string SelectedObjectName
        {
            get { return _selectedObjectName; }
            set { _selectedObjectName = value; RaisePropertyChanged(); }
        }

        //ToDo: Refactor. Should be lookup of other property.
        public bool IsActionSomething
        {
            get { return _isActionSomething; }
            set { _isActionSomething = value; RaisePropertyChanged(); }
        }

        public string ActionMessage
        {
            get { return _actionMessage; }
            set { _actionMessage = value;
                IsActionSomething = value != null; RaisePropertyChanged(); }
        }

        public bool IsActionError
        {
            get { return _isActionError; }
            set { _isActionError = value; RaisePropertyChanged(); }
        }

        public ObservableCollection<ProjectLabel> Labels
        {
            get { return _labels; }
            set
            {
                _labels = value;
                RaiseOtherPropertyChanged();
            }
        }

        public ProjectLabel SelectedLabel
        {
            get { return _label; }
            set
            {
                _label = value;
                RaisePropertyChanged();
            }
        }

        private void SaveManualEntry(object o)
        {
            try
            {
                _auditLogRepository.ClaimAuditRecord(SelectedDeveloper.DomainUserName, ReleaseDate, SelectedObjectName, SelectedObjectSchema, SelectedDatabase.DatabaseName, 
                    SelectedSqlServerObjectType.Category, string.Empty, SelectedLabel.Id);
                LinkObjectsChanged?.Invoke(this, EventArgs.Empty);
                ActionMessage = "Save Successful";
                IsActionError = false;
            }
            catch (Exception e)
            {
                MessageBox.Show("Error in SaveManualEntry: " + e.Message.Substring(0, 50));
                ActionMessage = "Error: " + e.Message.Substring(0, 50);
                IsActionError = true;
            }

        }

        private void RefreshSchemas(object o)
        {
            try { 
            ObjectSchemaOptions = _utilityRepository.GetDatabaseSchemaNames(SelectedDatabase?.DatabaseName);
            SelectedObjectSchema = ObjectSchemaOptions.SingleOrDefault(x => x == "dbo");
            RefreshObjects(o);
            }
            catch(Exception e)
            {
                MessageBox.Show("Error in RefreshSchemas: " + e.Message);
            }
        }

        private void RefreshObjects(object o)
        {
            try { 
            ActionMessage = null;
            ObjectNameOptions = _utilityRepository.GetObjectNames(SelectedDatabase.DatabaseName, SelectedObjectSchema, SelectedSqlServerObjectType.TypeCode);
            SelectedObjectName = ObjectNameOptions.FirstOrDefault();
            }
            catch(Exception e)
            {
                MessageBox.Show("Error in RefreshObjects: " + e.Message);
            }
        }

        public ManualEntryViewModel()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject())) return;
            ObjectTypeOptions = new List<SqlServerObjectType>
            {
                new SqlServerObjectType("U", "Table"),
                new SqlServerObjectType("P", "Stored Procedure"),
                new SqlServerObjectType("V", "View"),
                new SqlServerObjectType("TF", "Table Function"),
                new SqlServerObjectType("FN", "Scalar Function")
            };

            Labels = new ObservableCollection<ProjectLabel>();
            Labels.AddRange(_rogueProjectRepository.GetProjectLabels().Select(x => new ProjectLabel(x)));
            Labels.Insert(0, new ProjectLabel {Name = "Select a Label"});
            SelectedLabel = Labels.FirstOrDefault();

            SelectedSqlServerObjectType = ObjectTypeOptions.Single(x => x.TypeCode == "P");
            SelectedDatabase = DatabaseOptions.SingleOrDefault(x => x.DatabaseName.Equals("StoneEdgeRogue") && x.Environment == "NA");
            ObjectSchemaOptions = _utilityRepository.GetDatabaseSchemaNames(SelectedDatabase?.DatabaseName);
            SelectedObjectSchema = ObjectSchemaOptions.SingleOrDefault(x => x == "dbo");
            SelectedDeveloper = DeveloperOptions.SingleOrDefault(x => x.DomainUserName.Equals(Environment.UserName));
            ReleaseDate = DateHelper.GetNextWeekday(DateTime.Now, DayOfWeek.Thursday);
            if (SelectedDatabase != null && SelectedObjectSchema != null)
            {
                ObjectNameOptions = _utilityRepository.GetObjectNames(SelectedDatabase.DatabaseName, SelectedObjectSchema, SelectedSqlServerObjectType.TypeCode);
            }
            SelectedObjectName = ObjectNameOptions.FirstOrDefault();
        }
    }
}
