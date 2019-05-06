using System;
using System.Collections.Generic;

namespace DataLayer.Model
{
    public partial class AuditLog
    {
        public int Id { get; set; }
        public DateTime PostTime { get; set; }
        public string DatabaseName { get; set; }
        public string Event { get; set; }
        public string ObjectType { get; set; }
        public string ObjectSchema { get; set; }
        public string ObjectName { get; set; }
        public string ParentTable { get; set; }
        public string Tsql { get; set; }
        public string Login { get; set; }
        public string HostName { get; set; }
    }
}
