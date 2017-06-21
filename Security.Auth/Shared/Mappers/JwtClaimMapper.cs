using System.IdentityModel.Tokens.Jwt;
using Labs.Security.Domain.Features.Users;

namespace Labs.Security.Auth.Quickstart.Shared.Mappers
{
    public class JwtClaimMapper : IClaimMapper
    {
        public bool Contains(string type)
        {
            return JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.ContainsKey(type);
        }

        public string Fetch(string type)
        {
            return JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap[type];
        }
    }
}