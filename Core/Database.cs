using System.Collections.Generic;

namespace Core
{
    public class Database
    {
        public string DatabaseName { get; set; }
        public string ConnectionString { get; set; }
        public string Environment { get; set; }
        public string EnvironmentAndName => Environment + "-" + DatabaseName;

        public static readonly List<Database> AllDatabases = new List<Database>
        {
            new Database
            {
                DatabaseName = "SomeDb",
                Environment = "Dev",
                ConnectionString =
                    @"connection-string"
            }
        };
    }
}
