using System.Collections.Generic;
using System.Collections.Specialized;

namespace Core
{
    public class User
    {
        public string DomainUserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FirstLastName => FirstName + " " + LastName;
        public string FullyQualifiedDomainUsername => "Domain\\" + DomainUserName;
        public StringCollection SqlLoginNames { get; set; }

        public static readonly List<User> AllApplicationUsers = new List<User>
            {
                new User { FirstName = "Rudy", LastName = "Wilkinson", DomainUserName = "rwilkinson" }
            };
    }
}
