using System.Collections.Generic;
using IdentityServer4.Models;

namespace Labs.Security.Auth.Data
{
    public class ClientsData
    {
        public IEnumerable<Client> Load()
        {
            yield return DemoGrant();
            yield return DemoMvc();
            yield return DemoSpa();
        }

        protected Client DemoGrant()
        {
            // client credentials flow client
            return new Client
            {
                ClientId = "client",
                ClientName = "Client Credentials Client",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets =
                {
                    new Secret("373f4671-0c18-48d6-9da3-962b1c81299a".Sha256())
                },
                AllowedScopes =
                {
                    "api1"
                }
            };
        }

        protected Client DemoMvc()
        {
            // MVC client using hybrid flow
            return new Client
            {
                ClientId = "mvc",
                ClientName = "MVC Client",
                AllowedGrantTypes = GrantTypes.Hybrid,
                AllowOfflineAccess = true,
                AllowedScopes =
                {
                    "openid",
                    "profile",
                    "api1",
                },
                ClientSecrets =
                {
                    new Secret("373f4671-0c18-48d6-9da3-962b1c81299a".Sha256())
                },
                RedirectUris =
                {
                    "http://localhost:5001/signin-oidc"
                },
                LogoutUri = "http://localhost:5001/signout-oidc",
                PostLogoutRedirectUris =
                {
                    "http://localhost:5001/signout-callback-oidc"
                },
            };
        }

        protected Client DemoSpa()
        {
            // SPA client using implicit flow
            return new Client
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
            };
        }
    }
}