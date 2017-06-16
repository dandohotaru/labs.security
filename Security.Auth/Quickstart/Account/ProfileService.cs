using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Test;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Labs.Security.Auth.Quickstart.Account
{
    public class ProfileService : IProfileService
    {
        public ProfileService(TestUserStore users, ILogger<ProfileService> logger)
        {
            Logger = logger;
            Users = users;
        }

        protected ILogger Logger { get; set; }

        public TestUserStore Users { get; set; }

        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subject = context.Subject.GetSubjectId();
            var client = context.Client.ClientName ?? context.Client.ClientId;
            var types = context.RequestedClaimTypes;
            var caller = context.Caller;

            Logger.LogDebug("Get profile called for subject {subject} from client {client} with claim types {claimTypes} via {caller}", subject, client, types, caller);

            if (context.RequestedClaimTypes.Any())
            {
                var bySubjectId = Users.FindBySubjectId(context.Subject.GetSubjectId());
                context.AddFilteredClaims(bySubjectId.Claims);
            }

            return Task.FromResult(result: 0);
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;
            return Task.FromResult(result: 0);
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

    public static class IdentityServerOptionExtensions
    {
        public static IIdentityServerBuilder AddTestingUsers(this IIdentityServerBuilder builder, List<TestUser> users)
        {
            builder.Services.AddSingleton<TestUserStore>(new TestUserStore(users));
            builder.AddProfileService<ProfileService>();
            builder.AddResourceOwnerValidator<TestUserResourceOwnerPasswordValidator>();
            return builder;
        }
    }
}