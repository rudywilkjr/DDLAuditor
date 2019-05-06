using System;
using DataLayer.DTO;

namespace PresentationLayer.Model
{
    public class ProjectLabel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public bool IsJunk { get; set; }

        public ProjectLabel(ProjectLabelDomain projectLabelDomain)
        {
            Id = projectLabelDomain.Id;
            Name = projectLabelDomain.Name;
            ReleaseDate = projectLabelDomain.ReleaseDate;
            IsJunk = projectLabelDomain.IsJunk;
        }

        public ProjectLabel()
        {
            
        }
    }
}
