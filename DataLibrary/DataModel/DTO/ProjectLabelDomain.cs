using System;

namespace DataLibrary.DataModel.DTO
{
    public class ProjectLabelDomain
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public bool IsJunk { get; set; }
    }
}
