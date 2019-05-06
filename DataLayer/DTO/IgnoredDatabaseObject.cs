using System;

namespace DataLayer.DTO
{
    public class IgnoredDatabaseObject
    {
        public int Id { get; set; }
        public string ObjectDatabase { get; set; }
        public string ObjectSchema { get; set; }
        public string ObjectName { get; set; }
        public string IgnoredByUser { get; set; }
        public DateTime IgnoredByTime { get; set; }
    }
}
