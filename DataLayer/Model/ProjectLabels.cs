using System;
using System.Collections.Generic;

namespace DataLayer.Model
{
    public partial class ProjectLabels
    {
        public ProjectLabels()
        {
            ClaimedObjects = new HashSet<ClaimedObjects>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public bool? IsJunk { get; set; }

        public virtual ICollection<ClaimedObjects> ClaimedObjects { get; set; }
    }
}
