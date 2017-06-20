using System.Collections.Generic;
using System.Security.Claims;

namespace Labs.Security.Domain.Features.Users
{
    public class TestUsers
    {
        public static List<UserData> Users = new List<UserData>
        {
            new UserData
            {
                SubjectId = "88421113", Username = "bob", Password = "bob",
                Claims =
                {
                    new Claim("name", "Bob Smith"),
                    new Claim("given_name", "Bob"),
                    new Claim("family_name", "Smith"),
                    new Claim("email", "BobSmith@email.com"),
                    new Claim("email_verified", "true", ClaimValueTypes.Boolean),
                    new Claim("website", "http://bob.com"),
                    new Claim("address", @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", "json"),
                    new Claim("location", "somewhere"),
                }
            },
            new UserData
            {
                SubjectId = "818727", Username = "alice", Password = "alice",
                Claims =
                {
                    new Claim("name", "Alice Smith"),
                    new Claim("given_name", "Alice"),
                    new Claim("family_name", "Smith"),
                    new Claim("email", "AliceSmith@email.com"),
                    new Claim("email_verified", "true", ClaimValueTypes.Boolean),
                    new Claim("website", "http://alice.com"),
                    new Claim("address", @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", "json")
                }
            },
        };
    }
}