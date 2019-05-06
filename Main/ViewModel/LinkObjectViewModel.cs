using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Core;
using Core.Helper;
using DataLibrary.DataModel.DTO;
using DataLibrary.Repository;
using DataTracker.Model;
using DataTracker.Utility;
using MessageBox = Xceed.Wpf.Toolkit.MessageBox;

namespace DataTracker.ViewModel
{
    public class LinkObjectViewModel :INotifyPropertyChanged
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

        public ICommand LinkObjectsCommand => new DelegateCommand(Save);
        public ICommand CancelCommand => new DelegateCommand(CloseWindow);

        #region "Fields"
        private User _selectedDeveloper;
        private DateTime _releaseDate;
        private ObservableCollection<AssignableDatabaseObject> _databaseObjects;
        private ObservableCollection<User> _developerOptions;
        private ProjectLabel _label;
        private ObservableCollection<ProjectLabel> _labels;

        private readonly AuditLogRepository _auditLogRepository = new AuditLogRepository();
        private readonly RogueProjectRepository _rogueProjectRepository = new RogueProjectRepository();
        #endregion

        #region "Events"
        public event EventHandler LinkObjectsChanged;
        #endregion
        
        public ObservableCollection<ProjectLabel> Labels
        {
            get { return _labels; }
            set
            {
                _labels = value;
                RaiseOtherPropertyChanged();
            }
        }

        public User SelectedDeveloper
        {
            get { return _selectedDeveloper; }
            set { _selectedDeveloper = value; RaisePropertyChanged(); }
        }

        public ObservableCollection<User> DeveloperOptions
        {
            get { return _developerOptions; }
            set { _developerOptions = value; RaisePropertyChanged(); }
        }

        public DateTime ReleaseDate
        {
            get { return _releaseDate; }
            set { _releaseDate = value; RaisePropertyChanged(); }
        }

        public ObservableCollection<AssignableDatabaseObject> DatabaseObjects
        {
            get { return _databaseObjects; }
            set { _databaseObjects = value; RaisePropertyChanged(); }
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

        private void Save(object o)
        {
            try
            {
                foreach (var databaseObject in DatabaseObjects)
                {
                    _auditLogRepository.ClaimAuditRecord(SelectedDeveloper.DomainUserName, ReleaseDate, databaseObject.ObjectName, databaseObject.ObjectSchema, 
                        databaseObject.DatabaseName, databaseObject.Category, string.Empty, SelectedLabel.Id);
                }
                LinkObjectsChanged?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception e)
            {
                MessageBox.Show($"{e.Message}{Environment.NewLine}{e.InnerException?.Message}{Environment.NewLine}{e.InnerException?.InnerException?.Message}{Environment.NewLine}{e.StackTrace}","Error");
            }
        }

        public void CloseWindow(object o)
        {
            Application.Current.Windows.OfType<Window>().Single(x => x.Name == "LinkObjectWindow").DataContext = null;
            Application.Current.Windows.OfType<Window>().Single(x => x.Name == "LinkObjectWindow").Close();
        }

        public LinkObjectViewModel()
        {
            DeveloperOptions = new ObservableCollection<User>(User.AllApplicationUsers);
            SelectedDeveloper =
                DeveloperOptions.SingleOrDefault(
                    x => x.DomainUserName.Equals(Environment.UserName, StringComparison.InvariantCultureIgnoreCase));
            ReleaseDate = DateHelper.GetNextWeekday(DateTime.Now, DayOfWeek.Thursday);
            LinkObjectsChanged?.Invoke(this, EventArgs.Empty);
            Labels = new ObservableCollection<ProjectLabel>();
            Labels.AddRange(_rogueProjectRepository.GetProjectLabels().Select(x => new ProjectLabel(x)));
            Labels.Insert(0, new ProjectLabel {Id = 0, Name = "Select a Label"});
            SelectedLabel = Labels.FirstOrDefault();
        }
    }
}
