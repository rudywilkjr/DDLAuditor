using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Core;
using DataLayer.DTO;
using DataLayer.Repository;
using PresentationLayer.Model;
using PresentationLayer.Utility;
using LiveCharts;
using LiveCharts.Helpers;
using LiveCharts.Wpf;
using PresentationLayer.Views;

namespace PresentationLayer.ViewModel
{
    public class HomeViewModel : INotifyPropertyChanged
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

        #region "ChartProperties"

        public SeriesCollection SeriesCollection
        {
            get { return _seriesCollection; }
            set { _seriesCollection = value; RaisePropertyChanged(); }
        }

        public string[] Labels
        {
            get { return _labels; }
            set { _labels = value; RaisePropertyChanged(); }
        }

        public Func<int, string> Formatter { get; set; }
        #endregion

        #region "Commands"
        public ICommand FilterChangedCommand => new DelegateCommand(RefreshTabOnSelectedChanged);
        public ICommand ClaimObjectsCommand => new DelegateCommand(OpenClaimObjectsWindow);
        public ICommand IgnoreObjectsCommand => new DelegateCommand(IgnoreSelectedObjects);
        public ICommand RefreshCommand => new DelegateCommand(RefreshTabOnSelectedChanged);
        public ICommand OpenSettingsCommand => new DelegateCommand(OpenSettingsWindow);
        #endregion

        #region "Fields"
        private readonly AuditLogRepository _auditLogRepository = new AuditLogRepository();
        private ObservableCollection<AssignableDatabaseObject> _unclaimedDatabaseObjects;
        private ObservableCollection<ClaimedObject> _claimHistory;
        
        private UnclaimedGridViewModel _unclaimedGridViewModel;
        private ClaimedGridViewModel _claimedGridViewModel;
        private AuditLogViewModel _auditLogViewModel;
        private ManualEntryViewModel _manualEntryViewModel;
        private DatabaseItemCollectionViewModel _databaseItemCollectionViewModel;
        //private SettingsViewModel _settingsViewModel;

        //TODO: replace the multiple parameters with this class
        //since all view model user controls will need it
        //private TrackerParameters _trackerParameters;

        private string[] _labels;
        private SeriesCollection _seriesCollection;
        private LinkObjectViewModel _linkObjectViewModel;
        private int _selectedTabIndex;
        private bool _isLoading;
        private bool _isMinimumDateSelected;
        private int _usersUnclaimedChanges;
        private int _teamsUnclaimedChanges;
        private int _usersClaimedChanges;
        private int _teamsClaimedChanges;
        private User _selectedDeveloper;
        private Database _selectedDatabase;
        private bool _isDeveloperSelected;
        private bool _isDatabaseSelected;
        private DateTime _selectedMinimumDate;
        private bool _isMinimumDateFilterEnabled;
        private bool _hasBeenOnClaimedTab;

        private DateTime EffectiveMinimumSelectedDate => _isMinimumDateSelected ? _selectedMinimumDate : DateTime.Now.AddDays(-14);
        private string EffectiveDeveloperUserName => _isDeveloperSelected ? SelectedDeveloper.FullyQualifiedDomainUsername : string.Empty;


        private readonly Logger _log = new Logger();
        //private readonly RogueProjectRepository _rogueProjectRepository = new RogueProjectRepository();
        //private readonly UtilityRepository _utilityRepository = new UtilityRepository();

        #endregion

        #region "ViewModels"
        public LinkObjectViewModel LinkObjectViewModel
        {
            get { return _linkObjectViewModel; }
            set { _linkObjectViewModel = value; RaisePropertyChanged(); }
        }
        public UnclaimedGridViewModel UnclaimedResultsGridViewModel
        {
            get { return _unclaimedGridViewModel; }
            set { _unclaimedGridViewModel = value; RaisePropertyChanged(); }
        }
        public ClaimedGridViewModel ClaimedResultsGridViewModel
        {
            get { return _claimedGridViewModel; }
            set { _claimedGridViewModel = value; RaisePropertyChanged(); }
        }
        public AuditLogViewModel AuditLogViewModel
        {
            get { return _auditLogViewModel; }
            set { _auditLogViewModel = value; RaisePropertyChanged(); }
        }

        public ManualEntryViewModel ManualEntryViewModel
        {
            get { return _manualEntryViewModel; }
            set { _manualEntryViewModel = value; RaisePropertyChanged(); }
        }

        #endregion

        public ObservableCollection<AssignableDatabaseObject> UnclaimedDatabaseObjects
        {
            get { return _unclaimedDatabaseObjects; }
            set { _unclaimedDatabaseObjects = value; RaisePropertyChanged(); }
        }

        public ObservableCollection<ClaimedObject> ClaimHistory
        {
            get { return _claimHistory; }
            set { _claimHistory = value; RaisePropertyChanged(); }
        }

        public ObservableCollection<AuditRecord> AuditLogRecords
        {
            get { return AuditLogViewModel.AuditLogRecords; }
            set { AuditLogViewModel.AuditLogRecords = value; RaisePropertyChanged(); }
        }

        public DatabaseItemCollectionViewModel DatabaseItemCollectionViewModel
        {
            get { return _databaseItemCollectionViewModel; }
            set
            {
                _databaseItemCollectionViewModel = value;
                RaisePropertyChanged();
            }
        }

        public int UsersUnclaimedChanges
        {
            get { return _usersUnclaimedChanges; }
            set { _usersUnclaimedChanges = value; RaisePropertyChanged(); }
        }

        public int TeamsUnclaimedChanges
        {
            get { return _teamsUnclaimedChanges; }
            set { _teamsUnclaimedChanges = value; RaisePropertyChanged(); }
        }

        public int UsersClaimedChanges
        {
            get { return _usersClaimedChanges; }
            set { _usersClaimedChanges = value; RaisePropertyChanged(); }
        }

        public int TeamsClaimedChanges
        {
            get { return _teamsClaimedChanges; }
            set { _teamsClaimedChanges = value; RaisePropertyChanged(); }
        }

        public DateTime SelectedMinimumDate
        {
            get { return _selectedMinimumDate; }
            set
            {
                _selectedMinimumDate = value;
                IsMinimumDateSelected = true; RaisePropertyChanged();
            }
        }

        public User[] DeveloperOptions => User.AllApplicationUsers.OrderBy(x => x.FirstLastName).ToArray();

        public User SelectedDeveloper
        {
            get { return _selectedDeveloper; }
            set { _selectedDeveloper = value; IsDeveloperSelected = true; RaisePropertyChanged(); }
        }

        //ToDo: Implement Database filter
        public Database[] DatabaseOptions => Database.AllDatabases.OrderBy(x => x.EnvironmentAndName).ToArray();

        //ToDo: Implement Database filter
        public Database SelectedDatabase
        {
            get { return _selectedDatabase; }
            set { _selectedDatabase = value; IsDatabaseSelected = true; RaisePropertyChanged(); }
        }

        public int SelectedTabIndex
        {
            get { return _selectedTabIndex; }
            set { _selectedTabIndex = value;
                RefreshTabOnSelectedChanged(new object());
                RaisePropertyChanged();
                RaiseOtherPropertyChanged("IsSubmitButtonVisible");}
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                RaisePropertyChanged();
            }
        }

        public bool IsMinimumDateFilterEnabled
        {
            get { return _isMinimumDateFilterEnabled; }
            set { _isMinimumDateFilterEnabled = value; RaisePropertyChanged(); }
        }

        public bool IsSubmitButtonVisible => _selectedTabIndex == 0;

        public bool IsMinimumDateSelected
        {
            get { return _isMinimumDateSelected;}
            set { _isMinimumDateSelected = value; RaisePropertyChanged(); }
        }


        public bool IsDeveloperSelected
        {
            get { return _isDeveloperSelected; }
            set { _isDeveloperSelected = SelectedDeveloper != null && value; RaisePropertyChanged(); }
        }

        public bool IsDatabaseSelected
        {
            get { return _isDatabaseSelected; }
            set { _isDatabaseSelected = value; RaisePropertyChanged(); }
        }

        public void GetUnclaimedTabData()
        {
            List<DatabaseObject> data;
            if (IsMinimumDateSelected)
            {
                data = IsDeveloperSelected
                    ? _auditLogRepository.GetUnclaimedDatabaseObjects(SelectedDeveloper.DomainUserName, SelectedMinimumDate)
                    : _auditLogRepository.GetUnclaimedDatabaseObjects(null, SelectedMinimumDate);
            }
            else
            {
                data = IsDeveloperSelected
                    ? _auditLogRepository.GetUnclaimedDatabaseObjects(SelectedDeveloper.DomainUserName, DateTime.Now)
                    : _auditLogRepository.GetUnclaimedDatabaseObjects(null, DateTime.Now);
            }
            data = data.OrderBy(x => x.DatabaseName).ThenBy(x => x.ObjectSchema).ThenBy(x => x.ObjectName).ToList();
            var assignableDatabaseObjects = new List<AssignableDatabaseObject>();
            data.ForEach(z => assignableDatabaseObjects.Add(new AssignableDatabaseObject(z)));
            UnclaimedResultsGridViewModel.UnclaimedRecords = assignableDatabaseObjects;
            UpdateSummaryBoxNumbers();
            RefreshChart();
        }

        public void GetAuditLogTabData()
        {
            var records = new ObservableCollection<AuditRecord>();
            if (IsMinimumDateSelected)
            {
                if (IsDeveloperSelected)
                {
                    records.AddRange(_auditLogRepository.GetDdlAudits(SelectedMinimumDate, SelectedDeveloper.DomainUserName));
                }
                else
                {
                    records.AddRange(_auditLogRepository.GetDdlAudits(SelectedMinimumDate));
                }
            }
            else
            {
                if (IsDeveloperSelected)
                {
                    records.AddRange(_auditLogRepository.GetDdlAudits(DateTime.MinValue, SelectedDeveloper.DomainUserName));
                }
                else
                {
                    records.AddRange(_auditLogRepository.GetDdlAudits(DateTime.MinValue));
                }
            }

            AuditLogViewModel.AuditLogRecords = new ObservableCollection<AuditRecord>(records.OrderByDescending(x => x.PostDateTime));
        }

        public void RefreshChart()
        {
            //ToDo: Refactor. Move most of this to database.
            SeriesCollection = new SeriesCollection();
            var records = _auditLogRepository.GetDdlAutitsForChart(IsMinimumDateSelected ? SelectedMinimumDate : DateTime.MinValue);
            var uniqueDevs = records.Select(login => login).Distinct();
            var enumerable = uniqueDevs.OrderByDescending(x => x).ToArray();
            var chartValues = new ChartValues<int>();
            foreach (var dev in enumerable)
            {
                chartValues.Add(records.Count(login => login == dev));
            }
            SeriesCollection.Add(new RowSeries
            {
                Title = "# Changes",
                Values = chartValues
            });
            Labels = enumerable;
        }

        public void UpdateSummaryBoxNumbers()
        {
            if (IsMinimumDateSelected)
            {
                UsersUnclaimedChanges = _auditLogRepository.GetCountOfUserUnclaimedObjects(SelectedDeveloper.DomainUserName,
                    SelectedMinimumDate);
                TeamsUnclaimedChanges = _auditLogRepository.GetCountOfTeamUnclaimedObjects(SelectedMinimumDate);
                UsersClaimedChanges = _auditLogRepository.GetCountOfUserClaimedObjects(SelectedDeveloper.DomainUserName,
                    SelectedMinimumDate);
                TeamsClaimedChanges = _auditLogRepository.GetCountOfTeamClaimedObjects(SelectedMinimumDate);
            }
            else
            {
                UsersUnclaimedChanges = _auditLogRepository.GetCountOfUserUnclaimedObjects(SelectedDeveloper.DomainUserName,
                    DateTime.MinValue);
                TeamsUnclaimedChanges = _auditLogRepository.GetCountOfTeamUnclaimedObjects(DateTime.MinValue);
                UsersClaimedChanges = _auditLogRepository.GetCountOfUserClaimedObjects(SelectedDeveloper.DomainUserName,
                    DateTime.MinValue);
                TeamsClaimedChanges = _auditLogRepository.GetCountOfTeamClaimedObjects(DateTime.MinValue);
            }

        }

        private void OpenClaimObjectsWindow(object o)
        {
            //new viewmodel for new window
            LinkObjectViewModel = new LinkObjectViewModel { DatabaseObjects = new ObservableCollection<AssignableDatabaseObject>() };
            //Get the objects which were marked in the unclaimed grid on the main window and add them to new vm collection
            UnclaimedResultsGridViewModel.UnclaimedRecords.Where(x => x.IsAssign).ForEach(y => LinkObjectViewModel.DatabaseObjects.Add(y));
            //new window
            var linkWindow = new LinkObject { DataContext = LinkObjectViewModel };
            linkWindow.Show();
            LinkObjectViewModel.LinkObjectsChanged += HandleObjectsLinked;
        }

        private void OpenSettingsWindow(object obj)
        {
            var settingsWindow = new SettingsWindow
            {
                DataContext = new SettingsViewModel()
            };
            settingsWindow.Show();
        }

        private void HandleObjectsLinked(object sender, EventArgs e)
        {
            if (sender.GetType() == typeof (LinkObjectViewModel))
            {
                LinkObjectViewModel.CloseWindow(new object());
            }
            GetUnclaimedTabData();
        }

        private void OpenAuditLogTabWithObject(object sender, EventArgs e)
        {
            AuditLogViewModel.SearchFilter = UnclaimedResultsGridViewModel.SelectedUnclaimedRecord.ObjectName;
            AuditLogViewModel.IsExactFilterChecked = true;
            SelectedTabIndex = 2;
        }

        private void RefreshTabOnSelectedChanged(object o)
        {
            IsLoading = true;
            //Turn the Minimum Date Filter back on after leaving from the Claimed Tab where it is disabled.
            if (_hasBeenOnClaimedTab)
            {
                IsMinimumDateSelected = true;
                _hasBeenOnClaimedTab = false;
            }
            IsMinimumDateFilterEnabled = true;
            switch (SelectedTabIndex)
            {
                case 0:
                    GetUnclaimedTabData();
                    break;
                case 1:
                    ClaimedResultsGridViewModel.RefreshGrid();
                    _hasBeenOnClaimedTab = true;
                    IsMinimumDateFilterEnabled = false;
                    IsMinimumDateSelected = false;
                    break;
                case 2:
                    GetAuditLogTabData();
                    break;
            }
            IsLoading = false;
        }

        private void IgnoreSelectedObjects(object o)
        {
            foreach (var objectToIgnore in UnclaimedResultsGridViewModel.UnclaimedRecords.Where(x => x.IsAssign))
            {
                _auditLogRepository.AddIgnoredDatabaseObject(objectToIgnore.ObjectName, objectToIgnore.ObjectSchema
                    , objectToIgnore.DatabaseName, IsDeveloperSelected ? SelectedDeveloper.DomainUserName : Environment.UserName);
            }
            GetUnclaimedTabData();
        }

        public HomeViewModel()
        {
            try
            {
                UnclaimedResultsGridViewModel = new UnclaimedGridViewModel();
                LinkObjectViewModel = new LinkObjectViewModel();
                ClaimedResultsGridViewModel = new ClaimedGridViewModel(this);
                ManualEntryViewModel = new ManualEntryViewModel();
                DatabaseItemCollectionViewModel = new DatabaseItemCollectionViewModel();

                if (DesignerProperties.GetIsInDesignMode(new DependencyObject())) return;

                //Do not execute code below this line when in Designer
                #region "Default Values"
                _selectedDeveloper = DeveloperOptions.SingleOrDefault(x => x.DomainUserName.Contains(Environment.UserName));
                _isDeveloperSelected = _selectedDeveloper != null;
                _selectedMinimumDate = DateTime.Today.AddDays(-21);
                _isMinimumDateSelected = true;
                _isLoading = true;
                #endregion

                AuditLogViewModel = new AuditLogViewModel(new TrackerParameters
                {
                    SelectedDeveloper = _selectedDeveloper,
                    MinimumDateTime = _selectedMinimumDate
                });
                GetUnclaimedTabData();
                ManualEntryViewModel.LinkObjectsChanged += HandleObjectsLinked;
                UnclaimedResultsGridViewModel.UnclaimedObjectViewInAuditLogSelected += OpenAuditLogTabWithObject;
            }
            catch (Exception ex)
            {
                _log.Exception(ex);
            }
        }
    }
}
