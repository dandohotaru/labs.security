using System.Linq;
using System.Security.Principal;

namespace Labs.Security.Domain.Shared.Extensions
{
    public static class PrincipalExtensions
    {
        public static string Alias(this IIdentity identity)
        {
            if (identity is WindowsIdentity)
            {
                var parts = identity.Name.Split('\\');
                return parts.LastOrDefault();
            }

            return identity.Name;
        }
    }
}