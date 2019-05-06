using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using DataLayer.Repository;
using PresentationLayer.Model;
using PresentationLayer.Utility;
using Core.Helper;
using System.Collections.Generic;
using DataLayer.DTO;
using MessageBox = Xceed.Wpf.Toolkit.MessageBox;

namespace PresentationLayer.ViewModel
{
    public class ClaimedGridViewModel : INotifyPropertyChanged
    {
        #region "NotifyPropertyChanged"
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        // ReSharper disable once UnusedMember.Local
        private void RaiseOtherPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public ICommand FilterChangedCommand => new DelegateCommand(FilterChanged);

        public ICommand DeleteCommand => new DelegateCommand(DeleteClaimedObject);

        #region "Fields"
        private readonly AuditLogRepository _auditLogRepository = new AuditLogRepository();
        private ObservableCollection<ClaimedAuditRecord> _claimedRecords;
        private DateTime _releaseDate;
        private bool _isReleaseDateChecked;
        private readonly HomeViewModel _homeViewModel;
        private bool _isShowConflicts = true;

        #endregion

        public DateTime ReleaseDate
        {
            get { return _releaseDate; }
            set { _releaseDate = value;
                IsReleaseDateChecked = true; RaisePropertyChanged(); }
        }

        public bool IsReleaseDateChecked
        {
            get { return _isReleaseDateChecked; }
            set { _isReleaseDateChecked = value; RaisePropertyChanged(); }
        }

        public ObservableCollection<ClaimedAuditRecord> ClaimedRecords
        {
            get { return _claimedRecords; }
            set { _claimedRecords = value; IdentifyConflictObjects(); RaisePropertyChanged(); }
        }

        public ClaimedGridViewModel(HomeViewModel homeViewModel)
        {
            _homeViewModel = homeViewModel;
            _releaseDate = DateHelper.GetNextWeekday(DateTime.Now, DayOfWeek.Thursday);
        }

        public bool IsShowConflicts
        {
            get { return _isShowConflicts; }
            set { _isShowConflicts = value; RaisePropertyChanged(); }
        }

        public ClaimedGridViewModel()
        {
            
        }

        public void RefreshGrid()
        {
            var records = new List<ClaimedAuditRecord>();
            List<DDLAuditClaimedObjectDomain> claimedRecords;
            if (IsReleaseDateChecked)
            {
                claimedRecords = _homeViewModel.IsDeveloperSelected ?
                    _auditLogRepository.GetClaimedObjectsByReleaseDate(_homeViewModel.SelectedDeveloper.DomainUserName, ReleaseDate.Date) :
                    _auditLogRepository.GetClaimedObjectsByReleaseDate(ReleaseDate.Date);
            }
            else
            {
                claimedRecords = _homeViewModel.IsDeveloperSelected ?
                    _auditLogRepository.GetClaimedObjects(_homeViewModel.SelectedDeveloper.DomainUserName, _homeViewModel.SelectedMinimumDate.Date) :
                    _auditLogRepository.GetClaimedObjects(_homeViewModel.SelectedMinimumDate.Date);
            }
            
            claimedRecords.ForEach(c =>
                records.Add(new ClaimedAuditRecord(c))
            );

            ClaimedRecords = new ObservableCollection<ClaimedAuditRecord>(records);
        }

        private void IdentifyConflictObjects()
        {
            //ToDo: Refactor to do this in database via repo then remove this method
            var allClaimedAuditRecords = ClaimedRecords;

            foreach (var record in allClaimedAuditRecords.Where
                (record => allClaimedAuditRecords.Count(x => x.ObjectDatabase == record.ObjectDatabase &&
                                                                x.ObjectSchema == record.ObjectSchema &&
                                                                x.ObjectName == record.ObjectName) > 1))
            {
                record.IsConflict = true;
            }

            foreach (
                var record in
                    _claimedRecords.Where(
                        record => allClaimedAuditRecords.Any(x => x.IsConflict && x.Id == record.Id)))
            {
                record.IsConflict = true;
            }
            _claimedRecords = IsShowConflicts
                ? new ObservableCollection<ClaimedAuditRecord>(
                    _claimedRecords.OrderByDescending(x => x.IsConflict)
                        .ThenBy(x => x.ObjectDatabase)
                        .ThenByDescending(x => x.ObjectType == "TABLE")
                        .ThenByDescending(x => x.ObjectType == "VIEW")
                        .ThenByDescending(x => x.ObjectType == "PROCEDURE")
                        .ThenByDescending(x => x.ObjectType == "FUNCTION")
                        .ThenBy(x => x.ObjectSchema)
                        .ThenBy(x => x.ObjectName)
                        .ThenBy(x => x.ReleaseDate))
                : new ObservableCollection<ClaimedAuditRecord>(
                    _claimedRecords.OrderBy(x => x.ObjectDatabase)
                        .ThenByDescending(x => x.ObjectType == "TABLE")
                        .ThenByDescending(x => x.ObjectType == "VIEW")
                        .ThenByDescending(x => x.ObjectType == "PROCEDURE")
                        .ThenByDescending(x => x.ObjectType == "FUNCTION")
                        .ThenBy(x => x.ObjectSchema)
                        .ThenBy(x => x.ObjectName)
                        .ThenBy(x => x.ReleaseDate));
        }

        private void FilterChanged(object o)
        {
            RefreshGrid();
        }

        private void DeleteClaimedObject(object o)
        {
            try
            {
                _auditLogRepository.DeleteClaimedObject(((ClaimedAuditRecord) o).Id);
                ClaimedRecords.Remove((ClaimedAuditRecord) o);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            
        }
    }
}
