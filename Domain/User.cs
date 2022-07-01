using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace Domain
{
    public class User : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool InOffice { get; set; }
        public bool InTunnel { get; set; }

        [JsonIgnore]
        public Tag Tag { get; set; }
    }
}