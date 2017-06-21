﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Labs.Security.Domain.Features.Profiles.Providers;
using Labs.Security.Domain.Shared.Extensions;

namespace Labs.Security.Domain.Features.Users
{
    public class UserStore
    {
        public UserStore(ICollection<UserData> users, IClaimMapper mapper, IIdentityProvider provider)
        {
            Cache = users;
            Mapper = mapper;
            Provider = provider;
        }

        protected ICollection<UserData> Cache { get; set; }

        protected IClaimMapper Mapper { get; set; }

        public IIdentityProvider Provider { get; set; }

        public bool ValidateCredentials(string username, string password)
        {
            var query = from user in Cache
                        where user.Username != null && user.Password != null
                            && user.Username.Equals(username, StringComparison.OrdinalIgnoreCase)
                            && user.Password.Equals(password)
                        select user;

            return query.Any();
        }

        public UserData FindBySubjectId(string subjectId)
        {
            var query = from user in Cache
                        where user.SubjectId == subjectId
                        select user;

            return query.FirstOrDefault();
        }

        public UserData FindByUsername(string username)
        {
            var query = from user in Cache
                        where user.Username.Equals(username, StringComparison.OrdinalIgnoreCase)
                        select user;

            return query.FirstOrDefault();
        }

        public UserData FindByProvider(string provider, string subjectId, string connectId)
        {
            var query = from user in Cache
                        where user.ProviderName == provider
                            && user.ProviderSubjectId == subjectId
                            && user.ConnectId == connectId
                        select user;

            return query.FirstOrDefault();
        }

        public UserData ProvisionUser(string provider, string userId, string connectId, List<Claim> claims)
        {
            // ToDo: consider using async await [DanD]
            var criterion = new AliasesCriterion
            {
                Aliases = new []
                {
                    userId,
                    connectId,
                }
            };
            var profiles = Provider.Search(criterion).Result;
            if (profiles.Any())
            {
                var userClaims = new List<Claim>();

                var identity = profiles.SingleOrDefault(p => p.AliasName.Equals(userId, StringComparison.InvariantCultureIgnoreCase));
                if (identity == null)
                {
                    // ToDo: Consider invalidating authentication when alias name is not found [DanD]
                    // ToDo: Use AuthenticateResult with error message in theses case [DanD]

                    userClaims.Add("userName", userId);
                    userClaims.Add("userLabel", userId);
                    userClaims.Add("aliasName", userId);
                    userClaims.Add("firstName", userId);
                    userClaims.Add("lastName", userId);
                    userClaims.Add("fullName", userId);
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

                var user = new UserData
                {
                    SubjectId = userId,
                    ConnectId = connectId,
                    Username = userId,
                    ProviderName = provider,
                    ProviderSubjectId = userId,
                    Claims = userClaims
                };

                Cache.Add(user);
                return user;
            }
            else
            {
                var source1 = new List<Claim>();
                foreach (var claim in claims)
                {
                    if (claim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")
                        source1.Add(new Claim("name", claim.Value));
                    else if (Mapper.Contains(claim.Type))
                        source1.Add(new Claim(Mapper.Fetch(claim.Type), claim.Value));
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

                var uniqueId = Guid.NewGuid().ToString();
                var claim3 = source1.FirstOrDefault(c => c.Type == "name");
                var str = (claim3 != null ? claim3.Value : null) ?? uniqueId;

                var user = new UserData
                {
                    SubjectId = userId,
                    Username = str,
                    ProviderName = provider,
                    ProviderSubjectId = userId,
                    Claims = source1
                };

                Cache.Add(user);
                return user;
            }
        }
    }
}