using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DataLibrary.DataModel.DTO
{
    public class AuditRecord : INotifyPropertyChanged
    {
        #region "NotifyPropertyChanged"
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private int _id;
        private DateTime _postDateTime;
        private string _databaseName;
        private string _event;
        private string _objectType;
        private string _objectSchema;
        private string _objectName;
        private string _user;

        public int Id
        {
            get { return _id; }
            set { _id = value;
                RaisePropertyChanged();
            }
        }

        public DateTime PostDateTime
        {
            get { return _postDateTime; }
            set { _postDateTime = value;
                RaisePropertyChanged();
            }
        }

        public string DatabaseName
        {
            get { return _databaseName; }
            set { _databaseName = value;
                RaisePropertyChanged();
            }
        }

        public string Event
        {
            get { return _event; }
            set { _event = value;
                RaisePropertyChanged();
            }
        }

        public string ObjectType
        {
            get { return _objectType; }
            set { _objectType = value;
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

        public string ObjectName
        {
            get { return _objectName; }
            set { _objectName = value;
                RaisePropertyChanged();
            }
        }

        public string User
        {
            get { return _user.Remove(0,6); }
            set { _user = value;
                RaisePropertyChanged();
            }
        }

        public AuditRecord(DDLAudit auditLog)
        {
            _id = auditLog.Id;
            _postDateTime = auditLog.PostTime;
            _databaseName = auditLog.DatabaseName;
            _event = auditLog.Event;
            _objectType = auditLog.ObjectType;
            _objectSchema = auditLog.ObjectSchema;
            _objectName = string.IsNullOrEmpty(auditLog.ParentTable) ? auditLog.ObjectName : auditLog.ParentTable;
            _user = auditLog.Login;
        }

        public AuditRecord()
        {
            
        }

    }
}
