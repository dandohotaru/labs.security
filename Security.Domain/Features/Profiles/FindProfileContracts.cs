using Labs.Security.Domain.Features.Profiles.Models;
using Labs.Security.Domain.Shared.Messages;

namespace Labs.Security.Domain.Features.Profiles
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