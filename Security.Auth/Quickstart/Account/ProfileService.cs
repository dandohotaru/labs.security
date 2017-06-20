using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Labs.Security.Domain.Features.Profiles;
using Labs.Security.Domain.Features.Profiles.Providers;
using Labs.Security.Domain.Features.Users;
using Microsoft.Extensions.Logging;

namespace Labs.Security.Auth.Quickstart.Account
{
    public class ProfileService : IProfileService
    {
        public ProfileService(ILogger<ProfileService> logger, IIdentityProvider provider)
        {
            Logger = logger;
            Provider = provider;
        }

        protected ILogger Logger { get; private set; }

        protected IIdentityProvider Provider { get; private set; }

        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subject = context.Subject.GetSubjectId();
            var client = context.Client.ClientName ?? context.Client.ClientId;
            var types = context.RequestedClaimTypes;
            var caller = context.Caller;

            Logger.LogDebug("Get profile called for subject {subject} from client {client} with claim types {claimTypes} via {caller}", subject, client, types, caller);

            if (context.RequestedClaimTypes.Any())
            {
                var alias = context.Subject.GetSubjectId();
                var handler = new FindClaimsHandler(Provider);
                var request = new FindClaimsRequest(alias);
                var response = handler.Execute(request).Result;

                var claims = response.Results
                    .Where(p => p.Alias == alias)
                    .SelectMany(p => p.Claims);
                context.AddFilteredClaims(claims);
            }

            return Task.FromResult(result: 0);
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            // ToDo: Check if the user identity exists [DanD]
            context.IsActive = true;
            return Task.FromResult(result: 0);
        }
    }

    public class SimpleProfileService : IProfileService
    {
        public SimpleProfileService(ILogger<DefaultProfileService> logger)
        {
            Logger = logger;
        }

        protected ILogger<DefaultProfileService> Logger { get; }

        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subject = context.Subject.GetSubjectId();
            var client = context.Client.ClientName ?? context.Client.ClientId;
            var types = context.RequestedClaimTypes;
            var caller = context.Caller;

            Logger.LogDebug("Get profile called for subject {subject} from client {client} with claim types {claimTypes} via {caller}", subject, client, types, caller);

            if (context.RequestedClaimTypes.Any())
                context.AddFilteredClaims(context.Subject.Claims);
            return Task.FromResult(result: 0);
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;
            return Task.FromResult(result: 0);
        }
    }

    public class CustomProfileService : IProfileService
    {
        public CustomProfileService(ILogger<DefaultProfileService> logger, UserStore store)
        {
            Logger = logger;
            Store = store;
        }

        protected ILogger<DefaultProfileService> Logger { get; }

        protected UserStore Store { get; }

        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subject = context.Subject.GetSubjectId();
            var client = context.Client.ClientName ?? context.Client.ClientId;
            var types = context.RequestedClaimTypes;
            var caller = context.Caller;

            Logger.LogDebug("Get profile called for subject {subject} from client {client} with claim types {claimTypes} via {caller}", subject, client, types, caller);

            if (context.RequestedClaimTypes.Any())
            {
                var user = Store.FindBySubjectId(context.Subject.GetSubjectId());
                if (user != null)
                    context.AddFilteredClaims(user.Claims);
            }

            return Task.FromResult(result: 0);
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            var user = Store.FindBySubjectId(context.Subject.GetSubjectId());
            context.IsActive = user != null;
            return Task.FromResult(result: 0);
        }
    }
}