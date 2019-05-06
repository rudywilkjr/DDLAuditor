using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using DataLayer.Repository;
using PresentationLayer.Utility;
using DataLayer.DTO;
using System.Collections.ObjectModel;
using PresentationLayer.Model;

namespace PresentationLayer.ViewModel
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        #region "NotifyPropertyChanged"
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public ICommand RefreshCommand => new DelegateCommand(RefreshScreen);
        public ICommand DeleteLabelCommand => new DelegateCommand(DeleteProjectLabel);
        public ICommand SaveLabelCommand => new DelegateCommand(EditProjectLabel);
        public ICommand ClearLabelCommand => new DelegateCommand(ClearLabel);
        public ICommand DeleteIgnoredObjectCommand => new DelegateCommand(DeleteIgnoredObject);

        #region Fields
        private ProjectLabelDomain _label;
        private ObservableCollection<ProjectLabelDomain> _projectLabels;
        private string _message;
        #endregion Fields

        private readonly AuditLogRepository _auditLogRepository = new AuditLogRepository();
        private readonly RogueProjectRepository _projectRepository = new RogueProjectRepository();
        private ObservableCollection<IgnoredObjectModel> _ignoredObjects;

        public ObservableCollection<ProjectLabelDomain> ProjectLabels
        {
            get { return _projectLabels; }
            set
            {
                _projectLabels = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<IgnoredObjectModel> IgnoredObjects
        {
            get { return _ignoredObjects; }
            set { _ignoredObjects = value; RaisePropertyChanged(); }
        }

        public ProjectLabelDomain Label
        {
            get { return _label; }
            set
            {
                _label = value;
                RaisePropertyChanged();
            }
        }

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                RaisePropertyChanged();
            }
        }

        public SettingsViewModel()
        {
            RefreshLabels();
            IgnoredObjects =
                new ObservableCollection<IgnoredObjectModel>(_auditLogRepository.GetIgnoredDatabaseObjects()
                    .Select(x => new IgnoredObjectModel(x.Id, x.ObjectName, x.ObjectSchema, x.ObjectDatabase, x.IgnoredByTime, x.IgnoredByUser)));
        }

        private void RefreshScreen(object obj)
        {
            RaisePropertyChanged();
        }

        private void EditProjectLabel(object obj)
        {
            if(string.IsNullOrEmpty(Label?.Name))
            {
                MessageBox.Show("No name is entered for this label");
            }
            else
            {
                if (Label.Id == 0)
                {
                    _projectRepository.AddLabel(Label);
                }
                else
                {
                    _projectRepository.UpdateLabel(Label);
                }
                Message = $"Label {Label.Name} saved.";
                RefreshLabels();
            }
        }

        private void DeleteProjectLabel(object obj)
        {
            _projectRepository.DeleteLabel(Label);
            Message = $"Label {Label.Name} deleted.";
            RefreshLabels();
        }
        
        private void ClearLabel(object obj)
        {
            Label = new ProjectLabelDomain
            {
                Name = "<Enter A New Name>"
            };
        }

        private void RefreshLabels()
        {
            ProjectLabels = new ObservableCollection<ProjectLabelDomain>(_projectRepository.GetProjectLabels());
            Label = new ProjectLabelDomain
            {
                Id = 0,
                Name = string.Empty,
                ReleaseDate = null
            };
        }

        private void DeleteIgnoredObject(object obj)
        {
            _auditLogRepository.DeleteIgnoredDatabaseObject(((IgnoredObjectModel) obj).Id);
            IgnoredObjects =
                new ObservableCollection<IgnoredObjectModel>(_auditLogRepository.GetIgnoredDatabaseObjects()
                    .Select(x => new IgnoredObjectModel(x.Id, x.ObjectName, x.ObjectSchema, x.ObjectDatabase, x.IgnoredByTime, x.IgnoredByUser)));
        }
    }
}
