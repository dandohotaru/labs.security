using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Labs.Security.Domain.Features.Users;
using Microsoft.Extensions.Logging;

namespace Labs.Security.Auth.Quickstart.Account
{
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
        public CustomProfileService(ILogger<DefaultProfileService> logger, IUserStore store)
        {
            Logger = logger;
            Store = store;
        }

        protected ILogger<DefaultProfileService> Logger { get; }

        protected IUserStore Store { get; }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subject = context.Subject.GetSubjectId();
            var client = context.Client.ClientName ?? context.Client.ClientId;
            var types = context.RequestedClaimTypes;
            var caller = context.Caller;

            Logger.LogDebug("Get profile called for subject {subject} from client {client} with claim types {claimTypes} via {caller}", subject, client, types, caller);

            if (context.RequestedClaimTypes.Any())
            {
                var user = await Store.FindBySubjectId(context.Subject.GetSubjectId());
                if (user != null)
                    context.AddFilteredClaims(user.Claims);
            }
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var user = await Store.FindBySubjectId(context.Subject.GetSubjectId());
            context.IsActive = user != null;
        }
    }
}