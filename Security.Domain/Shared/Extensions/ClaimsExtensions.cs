using System.Collections.Generic;
using System.Security.Claims;

namespace Labs.Security.Domain.Shared.Extensions
{
    public static class ClaimsExtensions
    {
        public static void Attach(this ICollection<Claim> instance, string key, string value)
        {
            var claim = value == null
                ? new Claim(key, "unknown")
                : new Claim(key, value);

            instance.Add(claim);
        }

        public static void Attach(this ICollection<Claim> instance, string key, int? value)
        {
            var claim = value == null
                ? new Claim(key, "unknown")
                : new Claim(key, value.ToString());

            instance.Add(claim);
        }
    }
}