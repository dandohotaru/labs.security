using System.Collections.Generic;
using IdentityServer4.Models;

namespace Labs.Security.Auth.Data
{
    public class CarbonData
    {
        public IEnumerable<IdentityResource> Load()
        {
            yield return Openid();
            yield return Email();
            yield return Phone();
            yield return Profile();
            yield return Roles();
        }

        private static IdentityResource Openid()
        {
            return new IdentityResources.OpenId();
        }

        private static IdentityResource Email()
        {
            return new IdentityResources.Email();
        }

        private static IdentityResource Phone()
        {
            return new IdentityResources.Phone();
        }

        private static IdentityResource Profile()
        {
            return new IdentityResource
            {
                Name = "profile",
                DisplayName = "User profile",
                Description = "Your user profile information (first name, last name, etc.)",
                Enabled = true,
                Emphasize = true,
                UserClaims = new List<string>
                {
                    "userId",
                    "userName",
                    "userLabel",
                    "personId",
                    "firstName",
                    "lastName",
                    "fullName",
                    "aliasName",
                    "email",
                    "role",
                    "grant",
                },
            };
        }

        private static IdentityResource Roles()
        {
            return new IdentityResource
            {
                Name = "roles",
                DisplayName = "Roles",
                Description = "Encapsulates information about the roles claims",
                Enabled = true,
                Emphasize = true,
                UserClaims = new List<string>
                {
                    "role",
                    "grant",
                }
            };
        }
    }
}