using System.Collections.Generic;

namespace Labs.Security.Domain.Shared.Extensions
{
    public static class DictionaryExtensions
    {
        public static TValue Value<TKey, TValue>(this IDictionary<TKey, TValue> instance, TKey key)
        {
            TValue value;
            instance.TryGetValue(key, out value);
            return value;
        }
    }
}