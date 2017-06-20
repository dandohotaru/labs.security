using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;

namespace Labs.Security.Domain.Adfs.Shared.Extensions
{
    public static class SearchExtensions
    {
        public static T Parse<T>(this SearchResult result, string key, T fallback = default(T))
        {
            if (!result.Properties.Contains(key))
                return fallback;

            var values = result.Properties[key];
            if (values.Count == 0)
                return fallback;

            return values.OfType<T>().FirstOrDefault();
        }

        public static IEnumerable<T> Values<T>(this SearchResult result, string key)
        {
            if (!result.Properties.Contains(key))
                yield break;

            var values = result.Properties[key];
            foreach (var value in values)
            {
                var items = value as T[];
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        yield return item;
                    }
                }
                else
                {
                    var item = (T)value;
                    yield return item;
                }
            }
        }
    }
}