using System.Collections.Generic;
using IdentityServer4.Models;

namespace Labs.Security.Auth
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new[]
            {
                new IdentityResources.OpenId(),
                //new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResources.Address(),
                new IdentityResources.Phone(),
                new IdentityResource
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
                    }
                }
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new[]
            {
                new ApiResource("api1", "My API #1"),
                new ApiResource("api2", "My API #2"),
                new ApiResource
                {
                    Name = "permissions",
                    DisplayName = "Permissions",
                    Description = "Encapsulates information about the permissions claims",
                    Enabled = true,

                    //Claims = new List<ScopeClaim>
                    //{
                    //    new ScopeClaim("userId"),
                    //    new ScopeClaim("userName"),
                    //    new ScopeClaim("userLabel"),
                    //    new ScopeClaim("personId"),
                    //    new ScopeClaim("firstName"),
                    //    new ScopeClaim("lastName"),
                    //    new ScopeClaim("fullName"),
                    //    new ScopeClaim("aliasName"),
                    //    new ScopeClaim("email"),
                    //    new ScopeClaim("role"),
                    //    new ScopeClaim("grant"),
                    //    new ScopeClaim("commission"),
                    //}
                },
                new ApiResource
                {
                    Name = "roles",
                    DisplayName = "Roles",
                    Description = "Encapsulates information about the roles claims",
                    Enabled = true,

                    //Claims = new List<ScopeClaim>
                    //{
                    //    new ScopeClaim("role"),
                    //    new ScopeClaim("grant"),
                    //}
                }
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new[]
            {
                // client credentials flow client
                new Client
                {
                    ClientId = "client",
                    ClientName = "Client Credentials Client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = {new Secret("373f4671-0c18-48d6-9da3-962b1c81299a".Sha256())},
                    AllowedScopes = {"api1"}
                },

                // MVC client using hybrid flow
                new Client
                {
                    ClientId = "mvc",
                    ClientName = "MVC Client",
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    ClientSecrets = {new Secret("373f4671-0c18-48d6-9da3-962b1c81299a".Sha256())},
                    RedirectUris = {"http://localhost:5001/signin-oidc"},
                    LogoutUri = "http://localhost:5001/signout-oidc",
                    PostLogoutRedirectUris = {"http://localhost:5001/signout-callback-oidc"},
                    AllowOfflineAccess = true,
                    AllowedScopes = {"openid", "profile", "api1"}
                },

                // SPA client using implicit flow
                new Client
                {
                    ClientId = "spa",
                    ClientName = "SPA Client",
                    ClientUri = "http://identityserver.io",
                    RequireConsent = false,
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    AllowedScopes = new[]
                    {
                        "openid",
                        "profile",
                        "api1"
                    },
                    RedirectUris = new[]
                    {
                        "http://localhost:5002/index.html",
                        "http://localhost:5002/callback.html",
                        "http://localhost:5002/silent.html",
                        "http://localhost:5002/popup.html",
                        "http://localhost/samples.web/app/basic/index.html",
                    },
                    PostLogoutRedirectUris = new[]
                    {
                        "http://localhost:5002/index.html",
                        "http://localhost/samples.web/app/basic/index.html",
                    },
                    AllowedCorsOrigins = new[]
                    {
                        "http://localhost:5002",
                        "http://localhost",
                    },
                },
            };
        }
    }
}