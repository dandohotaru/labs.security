using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Labs.Security.Domain.Features.Profiles.Providers;
using Labs.Security.Domain.Shared.Extensions;
using Labs.Security.Domain.Shared.Messages;

namespace Labs.Security.Domain.Features.Profiles
{
    public class FindClaimsHandler : IHandler<FindClaimsRequest, FindClaimsResponse>
    {
        public FindClaimsHandler(IIdentityProvider provider)
        {
            Provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        protected IIdentityProvider Provider { get; private set; }

        public async Task<FindClaimsResponse> Execute(FindClaimsRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            if (request.Aliases == null)
                throw new ArgumentNullException(nameof(request.Aliases));

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var criterion = new AliasesCriterion {Aliases = request.Aliases};
            var profiles = await Provider.Search(criterion);

            var build = new Func<IdentityData, Claim[]>(p =>
            {
                var claims = new List<Claim>();
                claims.Add("userName", p.AliasName);
                claims.Add("userLabel", p.FullName);
                claims.Add("aliasName", p.AliasName);
                claims.Add("firstName", p.FirstName);
                claims.Add("lastName", p.LastName);
                claims.Add("fullName", p.FullName);

                return claims.ToArray();
            });
            var results = from profile in profiles
                          select new ResultModel
                          {
                              Alias = profile.AliasName,
                              Claims = build(profile),
                          };

            stopwatch.Stop();

            return new FindClaimsResponse
            {
                Meta = new
                {
                    Duration = stopwatch.ToDuration()
                },
                Results = results.ToArray()
            };
        }
    }
}