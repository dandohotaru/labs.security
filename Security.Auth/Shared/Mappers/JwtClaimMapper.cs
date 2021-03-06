using System.IdentityModel.Tokens.Jwt;

namespace Labs.Security.Auth.Shared.Mappers
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