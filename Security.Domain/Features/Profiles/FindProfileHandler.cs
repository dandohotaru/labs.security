using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Infsys.Security.Auth.Core.Features.Profiles.Providers;
using Infsys.Security.Auth.Core.Shared.Extensions;
using Infsys.Security.Auth.Core.Shared.Messages;

namespace Infsys.Security.Auth.Core.Features.Profiles
{
    public class FindProfileHandler : IHandler<FindProfileRequest, FindProfileResponse>
    {
        public FindProfileHandler(IProfileProvider provider)
        {
            Provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        protected IProfileProvider Provider { get; private set; }

        public async Task<FindProfileResponse> Execute(FindProfileRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            if (request.AliasNames == null)
                throw new ArgumentNullException(nameof(request.AliasNames));

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var criterion = new AliasesCriterion {Names = request.AliasNames};
            var profiles = await Provider.Search(criterion);

            stopwatch.Stop();

            return new FindProfileResponse
            {
                Meta = new
                {
                    Duration = stopwatch.ToDuration()
                },
                Profiles = profiles
            };
        }
    }
}