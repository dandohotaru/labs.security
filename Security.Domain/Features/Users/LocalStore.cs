using System.Collections.Generic;
using System.Security.Claims;
using Labs.Security.Domain.Features.Profiles.Providers;

namespace Labs.Security.Domain.Features.Users
{
    public class LocalStore : UserStore
    {
        public LocalStore(IIdentityProvider provider) : base(provider)
        {
            Cache = new List<UserData>
            {
                new UserData
                {
                    SubjectId = "bob",
                    Username = "bob",
                    Password = "bob",
                    Claims =
                    {
                        new Claim("userName", "bob"),
                        new Claim("labelName", "Bob Smith"),
                        new Claim("aliasName", "bob"),
                        new Claim("firstName", "Bob"),
                        new Claim("lastName", "Smith"),
                        new Claim("fullName", "Bob Smith"),
                        new Claim("email", "BobSmith@email.com"),
                        new Claim("email_verified", "true", ClaimValueTypes.Boolean),
                        new Claim("website", "http://bob.com"),
                        new Claim("address", @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", "json"),
                        new Claim("location", "somewhere"),
                    }
                },
                new UserData
                {
                    SubjectId = "alice",
                    Username = "alice",
                    Password = "alice",
                    Claims =
                    {
                        new Claim("userName", "alice"),
                        new Claim("labelName", "Alice Smith"),
                        new Claim("aliasName", "alice"),
                        new Claim("firstName", "Alice"),
                        new Claim("lastName", "Smith"),
                        new Claim("fullName", "Alice Smith"),
                        new Claim("email", "AliceSmith@email.com"),
                        new Claim("email_verified", "true", ClaimValueTypes.Boolean),
                        new Claim("website", "http://alice.com"),
                        new Claim("address", @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", "json")
                    }
                },
            };
        }
    }
}