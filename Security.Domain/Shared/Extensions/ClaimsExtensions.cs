using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Infsys.Security.Auth.Core.Shared.Extensions
{
    public static class ClaimsExtensions
    {
        public static void Add(this ICollection<Claim> instance, string key, string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value), "The claim value cannot be null");

            instance.Add(new Claim(key, value));
        }

        public static void Add(this ICollection<Claim> instance, string key, int? value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value), "The claim value cannot be null");

            instance.Add(new Claim(key, value.ToString()));
        }
    }
}