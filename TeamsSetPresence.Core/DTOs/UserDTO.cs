using System.Collections.Generic;

namespace TeamsSetPresence.Core.DTOs
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string Mail { get; set; }
        public List<string> BusinessPhones { get; set; }
        public string GivenName { get; set; }
        public object JobTitle { get; set; }
        public object MobilePhone { get; set; }
        public object OfficeLocation { get; set; }
        public object PreferredLanguage { get; set; }
        public string Surname { get; set; }
        public string UserPrincipalName { get; set; }
    }
}
