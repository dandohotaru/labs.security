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
                },
                new IdentityResource
                {
                    Name = "permissions",
                    DisplayName = "Permissions",
                    Description = "Encapsulates information about the permissions claims",
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
                },
                new IdentityResource
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
                }
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new[]
            {
                new ApiResource("api1", "Api #1"),
                new ApiResource("api2", "Api #2"),
                new ApiResource
                {
                    Name = "api3",
                    DisplayName = "Api #3",
                    Description = "Api #3",
                    Enabled = true,
                    UserClaims = new List<string>
                    {
                        "role",
                    }
                },
                new ApiResource
                {
                    Name = "api4",
                    DisplayName = "Api #4",
                    Description = "Api #4",
                    Enabled = true,
                    UserClaims = new List<string>
                    {
                        "role",
                        "grant",
                    }
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
                    ClientId = "samplesweb",
                    ClientName = "Spa Client",
                    ClientUri = "TBD",
                    Enabled = true,
                    RequireConsent = false,
                    IdentityTokenLifetime = 360,
                    AccessTokenLifetime = 3600,
                    AllowRememberConsent = false,
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    AllowedScopes = new[]
                    {
                        "openid",
                        "profile",
                        "permissions",
                        "roles",
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