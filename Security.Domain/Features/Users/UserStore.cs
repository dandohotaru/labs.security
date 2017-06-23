using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Labs.Security.Domain.Features.Profiles.Providers;
using Labs.Security.Domain.Shared.Extensions;

namespace Labs.Security.Domain.Features.Users
{
    public class UserStore : IUserStore
    {
        public UserStore(IIdentityProvider provider)
        {
            Cache = new List<UserData>();
            Provider = provider;
        }

        protected ICollection<UserData> Cache { get; set; }

        protected IIdentityProvider Provider { get; set; }

        public Task<bool> ValidateCredentials(string username, string password)
        {
            var query = from user in Cache
                        where user.Username != null && user.Password != null
                              && user.Username.Equals(username, StringComparison.OrdinalIgnoreCase)
                              && user.Password.Equals(password)
                        select user;

            return Task.FromResult(query.Any());
        }

        public Task<UserData> FindBySubjectId(string subjectId)
        {
            var query = from user in Cache
                        where user.SubjectId == subjectId
                        select user;

            return Task.FromResult(query.FirstOrDefault());
        }

        public Task<UserData> FindByUsername(string username)
        {
            var query = from user in Cache
                        where user.Username.Equals(username, StringComparison.OrdinalIgnoreCase)
                        select user;

            return Task.FromResult(query.FirstOrDefault());
        }

        public Task<UserData> FindByProvider(string provider, string subjectId, string connectId)
        {
            var query = from user in Cache
                        where user.Provider == provider
                              && user.ExternalId == subjectId
                              && user.ConnectId == connectId
                        select user;

            return Task.FromResult(query.FirstOrDefault());
        }

        public async Task<UserData> ProvisionUser(string provider, string userId, string connectId, List<Claim> claims)
        {
            var criterion = new AliasesCriterion
            {
                Aliases = new[]
                {
                    userId,
                    connectId,
                }
            };
            var profiles = await Provider.Search(criterion);
            if (profiles.Any())
            {
                var userClaims = new List<Claim>();

                var identity = profiles.SingleOrDefault(p => p.AliasName.Equals(userId, StringComparison.InvariantCultureIgnoreCase));
                if (identity == null)
                {
                    userClaims.Add("userName", userId);
                    userClaims.Add("userLabel", "anonymous");
                    userClaims.Add("aliasName", connectId);
                    userClaims.Add("firstName", "anonymous");
                    userClaims.Add("lastName", "anonymous");
                    userClaims.Add("fullName", "anonymous");
                }
                else
                {
                    var principal = profiles.SingleOrDefault(p => p.AliasName.Equals(connectId, StringComparison.InvariantCultureIgnoreCase));
                    if (principal == null)
                        principal = identity;

                    userClaims.Add("userName", identity.AliasName);
                    userClaims.Add("userLabel", identity.FullName);

                    userClaims.Add("aliasName", principal.AliasName);
                    userClaims.Add("firstName", principal.FirstName);
                    userClaims.Add("lastName", principal.LastName);
                    userClaims.Add("fullName", principal.FullName);
                }

                var query = from entry in Cache
                            where entry.Provider == provider
                                  && entry.ExternalId == userId
                            select entry;

                var user = query.FirstOrDefault();
                if (user == null)
                {
                    user = new UserData
                    {
                        SubjectId = userId,
                        ConnectId = connectId,
                        Username = userId,
                        Provider = provider,
                        ExternalId = userId,
                        Claims = userClaims,
                    };
                }
                else
                {
                    user.ConnectId = connectId;
                    user.Claims = userClaims;
                }

                Cache.Add(user);
                return user;
            }
            else
            {
                var nameSchema = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
                var nameClaim = claims.Find(p => p.Type == "name" || p.Type == nameSchema);
                var name = nameClaim == null
                    ? userId
                    : nameClaim.Value;

                var user = new UserData
                {
                    SubjectId = userId,
                    Username = name,
                    Provider = provider,
                    ExternalId = userId,
                    Claims = claims,
                };

                Cache.Add(user);
                return user;
            }
        }
    }
}