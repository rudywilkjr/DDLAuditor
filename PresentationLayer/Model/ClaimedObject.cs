using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DataLayer.DTO;

namespace PresentationLayer.Model
{
    public class ClaimedObject : INotifyPropertyChanged
    {
        private int _id;
        private string _userName;
        private DateTime _releaseDate;
        private string _notes;
        private string _objectDatabase;
        private string _objectSchema;
        private string _objectName;
        private string _objectType;
        private string _label;

        #region "NotifyPropertyChanged"
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public int Id
        {
            get { return _id; }
            set { _id = value;
                RaisePropertyChanged();
            }
        }

        public string UserName
        {
            get { return _userName; }
            set { _userName = value;
                RaisePropertyChanged();
            }
        }

        public string ObjectDatabase
        {
            get { return _objectDatabase; }
            set { _objectDatabase = value;
                RaisePropertyChanged();
            }
        }

        public string ObjectSchema
        {
            get { return _objectSchema; }
            set { _objectSchema = value;
                RaisePropertyChanged();
            }
        }

        public string ObjectType
        {
            get { return _objectType; }
            set { _objectType = value;
                RaisePropertyChanged(); }
        }

        public string ObjectName
        {
            get { return _objectName; }
            set { _objectName = value;
                RaisePropertyChanged();
            }
        }

        public DateTime ReleaseDate
        {
            get { return _releaseDate; }
            set { _releaseDate = value;
                RaisePropertyChanged();
            }
        }

        public string Notes
        {
            get { return _notes; }
            set { _notes = value;
                RaisePropertyChanged();
            }
        }

        public string Label
        {
            get { return _label; }
            set
            {
                _label = value;
                RaisePropertyChanged();
            }
        }
    }

    public class ClaimedAuditRecord : ClaimedObject
    {
        #region "NotifyPropertyChanged"
        public new event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region "Fields"
        private bool _isConflict;
        #endregion

        public bool IsConflict
        {
            get { return _isConflict; }
            set { _isConflict = value; RaisePropertyChanged(); }
        }

        public ClaimedAuditRecord(DDLAuditClaimedObjectDomain claimedObject)
        {
            Id = claimedObject.Id;
            ObjectDatabase = claimedObject.ObjectDatabase;
            ObjectSchema = claimedObject.ObjectSchema;
            ObjectName = claimedObject.ObjectName;
            ObjectType = claimedObject.ObjectType;
            Notes = claimedObject.Notes;
            ReleaseDate = claimedObject.Label?.ReleaseDate ?? claimedObject.ReleaseDate.GetValueOrDefault();
            UserName = claimedObject.Username;
            Label = claimedObject.Label?.Name;
        }
    }
}
