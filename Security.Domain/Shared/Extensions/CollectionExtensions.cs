using System.Collections.Generic;
using System.Linq;

namespace Infsys.Security.Auth.Core.Shared.Extensions
{
    public static class CollectionExtensions
    {
        public static bool Empty<T>(this IEnumerable<T> instance)
        {
            return !instance.Any();
        }

        public static string Collate<T>(this IEnumerable<T> instance, string separator = "")
        {
            return string.Join(separator, instance);
        }
    }
}