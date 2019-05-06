using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using PresentationLayer.Model;
using PresentationLayer.Utility;
using DataLayer.Repository;
using System.Linq;
using System;
using DataLayer.DTO;

namespace PresentationLayer.ViewModel
{
    public class AuditLogViewModel : INotifyPropertyChanged
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

        public ICommand ViewSqlDialog => new DelegateCommand(ViewSqlInWindow);
        public ICommand AuditRecordSqlSelected => new DelegateCommand(SelectSql);
        public ICommand ApplyFilterCommand => new DelegateCommand(FilterRecords);

        #region "Fields"
        private int _selectedAuditLogId;
        private ObservableCollection<AuditRecord> _auditLogRecords;
        private SqlDialogViewModel _sqlDialogViewModelViewModel;
        private string _searchFilter;
        private TrackerParameters _parameters;
        private readonly AuditLogRepository _auditLogRepository = new AuditLogRepository();
        private bool _isExactFilterChecked;
        #endregion

        public string SearchFilter
        {
            get { return _searchFilter; }
            set
            {
                _searchFilter = value;
                RaisePropertyChanged();
            }
        }

        public SqlDialogViewModel SqlDialogViewModelViewModel
        {
            get { return _sqlDialogViewModelViewModel; }
            set
            {
                _sqlDialogViewModelViewModel = value; RaisePropertyChanged();
            }
        }

        public ObservableCollection<AuditRecord> AuditLogRecords
        {
            get { return _auditLogRecords; }
            set { _auditLogRecords = value; RaiseOtherPropertyChanged("FilteredAuditLogRecords"); }
        }

        public ObservableCollection<AuditRecord> FilteredAuditLogRecords
        {
            get
            {
                if (IsExactFilterChecked)
                {
                    return string.IsNullOrEmpty(SearchFilter)
                        ? AuditLogRecords
                        : new ObservableCollection<AuditRecord>(AuditLogRecords
                            .Where(x => x.ObjectName == SearchFilter || x.User == SearchFilter));
                }
                return string.IsNullOrEmpty(SearchFilter)
                    ? AuditLogRecords
                    : new ObservableCollection<AuditRecord>(AuditLogRecords
                        .Where(x => x.ObjectName.IndexOf(SearchFilter, StringComparison.CurrentCultureIgnoreCase) > -1 
                        || x.User.IndexOf(SearchFilter, StringComparison.CurrentCultureIgnoreCase) > -1));

            }
        } 

        public int SelectedAuditLogId
        {
            get { return _selectedAuditLogId; }
            set { _selectedAuditLogId = value; RaisePropertyChanged(); }
        }

        public TrackerParameters Parameters
        {
            get { return _parameters; }
            set
            {
                _parameters = value;
                RaisePropertyChanged();
            }
        }

        public bool IsExactFilterChecked
        {
            get { return _isExactFilterChecked; }
            set { _isExactFilterChecked = value; RaisePropertyChanged(); RaiseOtherPropertyChanged("FilteredAuditLogRecords"); }
        }

        private void ViewSqlInWindow(object o)
        {
            SqlDialogViewModelViewModel.CreateNewTab(SelectedAuditLogId);
        }

        private void SelectSql(object auditRecord)
        {
            SelectedAuditLogId = ((AuditRecord) auditRecord).Id;
            SqlDialogViewModelViewModel.CreateNewTab(SelectedAuditLogId);
            SqlDialogViewModelViewModel.ShowWindow();
        }

        public AuditLogViewModel()
        {
            SqlDialogViewModelViewModel = new SqlDialogViewModel();
            SearchFilter = string.Empty;
            Parameters = new TrackerParameters
            {
                SelectedDeveloper = null,
                MinimumDateTime = DateTime.Today.AddDays(-21)
            };
            ReloadAuditRecords(Parameters);
        }

        public AuditLogViewModel(TrackerParameters parameters)
        {
            SqlDialogViewModelViewModel = new SqlDialogViewModel();
            SearchFilter = string.Empty;
            Parameters = parameters;
            ReloadAuditRecords(Parameters);
        }

        public void ReloadAuditRecords(TrackerParameters parameters)
        {
            var records = new ObservableCollection<AuditRecord>();
            if (Parameters.IsDeveloperSelected)
            {
                if (Parameters.MinimumDateTime != null)
                    records.AddRange(_auditLogRepository.GetDdlAudits(Parameters.MinimumDateTime.Value,
                        Parameters.SelectedDeveloper.DomainUserName));
            }
            else
            {
                if (Parameters.MinimumDateTime != null)
                    records.AddRange(_auditLogRepository.GetDdlAudits(Parameters.MinimumDateTime.Value));
            }

            AuditLogRecords = new ObservableCollection<AuditRecord>(records.OrderByDescending(x => x.PostDateTime));
        }

        private void FilterRecords(object o)
        {
            RaiseOtherPropertyChanged("FilteredAuditLogRecords");
        }
    }
}
