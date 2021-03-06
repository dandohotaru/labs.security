using System.Collections.Generic;
using System.Security.Claims;
using Labs.Security.Domain.Shared.Compares;

namespace Labs.Security.Domain.Features.Users
{
    public class UserData
    {
        public UserData()
        {
            Claims = new HashSet<Claim>(new ClaimsComparer());
        }

        public string SubjectId { get; set; }

        public string ConnectId { get; set; }

        public string ExternalId { get; set; }

        public string Provider { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public ICollection<Claim> Claims { get; set; }
    }
}