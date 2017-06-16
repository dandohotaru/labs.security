using Infsys.Security.Auth.Core.Features.Profiles.Models;
using Infsys.Security.Auth.Core.Shared.Messages;

namespace Infsys.Security.Auth.Core.Features.Profiles
{
    public class FindProfileRequest : IQuery<FindProfileResponse>
    {
        public string[] AliasNames { get; set; }
    }

    public class FindProfileResponse : IResult
    {
        public dynamic Meta { get; set; }

        public ProfileModel[] Profiles { get; set; }
    }
}