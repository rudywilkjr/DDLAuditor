using System;
using System.Collections.Generic;

namespace DataLayer.Model
{
    public partial class ClaimedObjects
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string ObjectName { get; set; }
        public string ObjectSchema { get; set; }
        public string ObjectDatabase { get; set; }
        public string Notes { get; set; }
        public string ObjectType { get; set; }
        public int? ProjectLabelId { get; set; }

        public virtual ProjectLabels ProjectLabel { get; set; }
    }
}
