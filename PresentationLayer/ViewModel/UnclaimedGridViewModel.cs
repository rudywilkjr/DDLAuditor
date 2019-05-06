using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using DataLayer.DTO;
using PresentationLayer.Utility;

namespace PresentationLayer.ViewModel
{
    public class UnclaimedGridViewModel : INotifyPropertyChanged
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

        #region "Commands"
        public ICommand GoToAuditLogCommand => new DelegateCommand(LookupInAuditLog);
        #endregion
        
        #region "Fields"
        private List<AssignableDatabaseObject> _unclaimedRecords;

        #endregion

        #region "Events"
        public event EventHandler UnclaimedObjectViewInAuditLogSelected;
        #endregion

        public List<AssignableDatabaseObject> UnclaimedRecords
        {
            get { return _unclaimedRecords; }
            set { _unclaimedRecords = value; RaisePropertyChanged(); }
        }

        public AssignableDatabaseObject SelectedUnclaimedRecord { get; set; }

        private void LookupInAuditLog(object o)
        {
            SelectedUnclaimedRecord = (AssignableDatabaseObject) o;
            UnclaimedObjectViewInAuditLogSelected?.Invoke(null, new EventArgs());
        }
    }
}
