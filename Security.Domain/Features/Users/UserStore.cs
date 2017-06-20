﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Labs.Security.Domain.Features.Users
{
    public class UserStore
    {
        public UserStore(ICollection<UserData> users, IClaimMapper mapper)
        {
            Cache = users;
            Mapper = mapper;
        }

        protected ICollection<UserData> Cache { get; set; }

        protected IClaimMapper Mapper { get; set; }

        public bool ValidateCredentials(string username, string password)
        {
            var query = from user in Cache
                        where user.Username.Equals(username, StringComparison.OrdinalIgnoreCase)
                        select user;
            var found = query.FirstOrDefault();
            if (found == null)
                return false;

            return found.Password.Equals(password);
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

        public UserData FindByProvider(string provider, string userId)
        {
            var query = from user in Cache
                        where user.ProviderName == provider
                            && user.ProviderSubjectId == userId
                        select user;

            return query.FirstOrDefault();
        }

        public UserData ProvisionUser(string provider, string userId, List<Claim> claims)
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
            var testUser = new UserData
            {
                //SubjectId = uniqueId,
                SubjectId = userId,
                Username = str,
                ProviderName = provider,
                ProviderSubjectId = userId,
                Claims = source1
            };

            Cache.Add(testUser);
            return testUser;
        }
    }
}