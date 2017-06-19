using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Test;
using Labs.Security.Domain.Features.Profiles;
using Labs.Security.Domain.Shared.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Security.Auth.Exchange.Profiles;

namespace Labs.Security.Auth.Quickstart.Account
{
    public class ProfileService : IProfileService
    {
        public ProfileService(ILogger<ProfileService> logger)
        {
            Logger = logger;
        }

        protected ILogger Logger { get; private set; }

        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subject = context.Subject.GetSubjectId();
            var client = context.Client.ClientName ?? context.Client.ClientId;
            var types = context.RequestedClaimTypes;
            var caller = context.Caller;

            Logger.LogDebug("Get profile called for subject {subject} from client {client} with claim types {claimTypes} via {caller}", subject, client, types, caller);

            if (context.RequestedClaimTypes.Any())
            {
                var claims = GetProfileClaims(context.Subject.GetSubjectId(), null);
                context.AddFilteredClaims(claims);
            }

            return Task.FromResult(result: 0);
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            // ToDo: Check if the user name exists [DanD]
            //context.IsActive = (user != null) && user.Active;
            context.IsActive = true;
            return Task.FromResult(result: 0);
        }

        private static IEnumerable<Claim> GetProfileClaims(string aliasName, string connectName)
        {
            // ToDo: Inject context factory for provider [DanD]
            var claims = new List<Claim>();

            var provider = new ExchangeProfileProvider();
            var handler = new FindProfileHandler(provider);
            var request = new FindProfileRequest
            {
                AliasNames = new[]
                {
                    aliasName,
                    connectName,
                },
            };
            var response = handler.Execute(request).Result;

            var identity = response.Profiles.SingleOrDefault(p => p.AliasName.Equals(aliasName, StringComparison.InvariantCultureIgnoreCase));
            if (identity == null)
            {
                // ToDo: Consider invalidating authentication when alias name is not found [DanD]
                // ToDo: Use AuthenticateResult with error message in theses case [DanD]

                claims.Add("userName", aliasName);
                claims.Add("userLabel", aliasName);

                claims.Add("aliasName", aliasName);
                claims.Add("firstName", aliasName);
                claims.Add("lastName", aliasName);
                claims.Add("fullName", aliasName);
            }
            else
            {
                var principal = response.Profiles.SingleOrDefault(p => p.AliasName.Equals(connectName, StringComparison.InvariantCultureIgnoreCase));
                if (principal == null)
                {
                    principal = identity;
                }

                claims.Add("userName", identity.AliasName);
                claims.Add("userLabel", identity.FullName ?? "unknown");

                claims.Add("aliasName", principal.AliasName);
                claims.Add("firstName", principal.FirstName ?? "unknown");
                claims.Add("lastName", principal.LastName ?? "unknown");
                claims.Add("fullName", principal.FullName ?? "unknown");
            }

            return claims;
        }
    }

    public class CustomUser
    {
        public CustomUser()
        {
        }

        public CustomUser(Guid subjectId)
        {
            SubjectId = subjectId.ToString();
        }

        public string SubjectId { get; set; }

        public string AliasName { get; set; }

        public string ProviderName { get; set; }

        public List<Claim> Claims { get; set; }
    }
}