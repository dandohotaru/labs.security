using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using IdentityModel;

namespace Labs.Security.Auth.Quickstart.Account
{
    public class UserStore
    {
        public UserStore(List<UserData> users)
        {
            Users = users;
        }

        protected List<UserData> Users { get; }

        public bool ValidateCredentials(string username, string password)
        {
            var byUsername = FindByUsername(username);
            if (byUsername != null)
                return byUsername.Password.Equals(password);
            return false;
        }

        public UserData FindBySubjectId(string subjectId)
        {
            return Users.FirstOrDefault(x => x.SubjectId == subjectId);
        }

        public UserData FindByUsername(string username)
        {
            return Users.FirstOrDefault(x => x.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }

        public UserData FindByExternalProvider(string provider, string userId)
        {
            return Users.FirstOrDefault(x =>
            {
                if (x.ProviderName == provider)
                    return x.ProviderSubjectId == userId;
                return false;
            });
        }

        public UserData AutoProvisionUser(string provider, string userId, List<Claim> claims)
        {
            var source1 = new List<Claim>();
            foreach (var claim in claims)
            {
                if (claim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")
                    source1.Add(new Claim("name", claim.Value));
                else if (JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.ContainsKey(claim.Type))
                    source1.Add(new Claim(JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap[claim.Type], claim.Value));
                else
                    source1.Add(claim);
            }
            var source2 = source1;
            Func<Claim, bool> predicate = x => x.Type == "name";
            if (!source2.Any(predicate))
            {
                var claim1 = source1.FirstOrDefault(x => x.Type == "given_name");
                var str1 = claim1 != null ? claim1.Value : null;
                var claim2 = source1.FirstOrDefault(x => x.Type == "family_name");
                var str2 = claim2 != null ? claim2.Value : null;
                if (str1 != null && str2 != null)
                    source1.Add(new Claim("name", str1 + " " + str2));
                else if (str1 != null)
                    source1.Add(new Claim("name", str1));
                else if (str2 != null)
                    source1.Add(new Claim("name", str2));
            }
            var uniqueId = CryptoRandom.CreateUniqueId(length: 32);
            var claim3 = source1.FirstOrDefault(c => c.Type == "name");
            var str = (claim3 != null ? claim3.Value : null) ?? uniqueId;
            var testUser = new UserData
            {
                //SubjectId = uniqueId,
                SubjectId = userId,
                Username = str,
                ProviderName = provider,
                ProviderSubjectId = userId,
                Claims = source1
            };
            Users.Add(testUser);
            return testUser;
        }
    }

    public class UserData
    {
        public ICollection<Claim> Claims { get; set; } = (ICollection<Claim>) new HashSet<Claim>(new ClaimComparer());

        public string SubjectId { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string ProviderName { get; set; }

        public string ProviderSubjectId { get; set; }
    }
}