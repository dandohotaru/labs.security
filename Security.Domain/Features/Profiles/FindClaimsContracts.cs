using System.Runtime.Serialization;
using System.Security.Claims;
using Labs.Security.Domain.Shared.Messages;

namespace Labs.Security.Domain.Features.Profiles
{
    [DataContract]
    public class FindClaimsRequest : IQuery<FindClaimsResponse>
    {
        public FindClaimsRequest(string alias)
        {
            Aliases = new[] {alias};
        }

        public FindClaimsRequest(string[] aliases)
        {
            Aliases = aliases;
        }

        [DataMember(Name = "Aliases")]
        public string[] Aliases { get; set; }
    }

    [DataContract]
    public class FindClaimsResponse : IResult
    {
        [DataMember(Name = "Meta")]
        public dynamic Meta { get; set; }

        [DataMember(Name = "Results")]
        public ResultModel[] Results { get; set; }
    }

    [DataContract]
    public class ResultModel : IModel
    {
        [DataMember(Name = "Alias")]
        public string Alias { get; set; }

        [DataMember(Name = "Claims")]
        public Claim[] Claims { get; set; }
    }
}