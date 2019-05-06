using System;

namespace DataLayer.DTO
{
    public class DDLAuditClaimedObjectDomain
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string ObjectName { get; set; }
        public string ObjectSchema { get; set; }
        public string ObjectDatabase { get; set; }
        public string Notes { get; set; }
        public string ObjectType { get; set; }
        public ProjectLabelDomain Label { get; set; }
    }
}
