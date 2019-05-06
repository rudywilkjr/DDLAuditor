using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using DataLayer.Repository;

namespace PresentationLayer.ViewModel
{
    public class SqlDialogViewModel : INotifyPropertyChanged
    {
        #region "NotifyPropertyChanged"
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region "Fields"
        private readonly AuditLogRepository _auditLogRepository = new AuditLogRepository();
        private int _selectedTabIndex;
        private ObservableCollection<TabItem> _tabs;
        #endregion
        
        public ObservableCollection<TabItem> Tabs
        {
            get { return _tabs; }
            set { _tabs = value; RaisePropertyChanged(); }
        }

        public int SelectedTabIndex
        {
            get { return _selectedTabIndex; }
            set { _selectedTabIndex = value; RaisePropertyChanged(); }
        }

        public void CreateNewTab(int auditLogId)
        {
            var auditRecord = _auditLogRepository.GetDdlAudit(auditLogId).SingleOrDefault();
            if (auditRecord != null)
            {
                Tabs.Add(new TabItem { Header = auditRecord.ObjectName, Content = "Edited By: " + auditRecord.Login + "\n\n" + auditRecord.TSQL});
                SelectedTabIndex = Tabs.Count() - 1;
            }
                
        }

        public void ShowWindow()
        {
            foreach (Window w in Application.Current.Windows)
            {
                if (w.DataContext != this) continue;
                w.Show();
                w.Focus();
                return;
            }
            var sqlDialogWindow = new Views.ViewSqlWindow { DataContext = this };
            sqlDialogWindow.Closing += OnWindowClosing;
            sqlDialogWindow.Show();
        }

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            ((Window) sender).DataContext = null;
            Tabs = new ObservableCollection<TabItem>();
            SelectedTabIndex = 0;
        }

        public SqlDialogViewModel()
        {
            Tabs = new ObservableCollection<TabItem>();
        }
    }
}
