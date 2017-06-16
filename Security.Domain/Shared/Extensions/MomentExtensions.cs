using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Labs.Security.Domain.Shared.Extensions
{
    public static class MomentExtensions
    {
        public static string ToDuration(this Stopwatch instance)
        {
            return instance.Elapsed.TotalSeconds < 1 
                ? string.Format("{0} miliseconds", instance.Elapsed.TotalMilliseconds)
                : string.Format("{0} seconds", instance.Elapsed.TotalSeconds);
        }

        public static bool Same(this DateTime? instance, DateTime? other)
        {
            if (instance == null && other == null)
                return true;

            if (instance == null || other == null)
                return false;

            return instance.Value.Date == other.Value.Date;
        }

        public static IEnumerable<DateTime> Range(this DateTime? instance, DateTime? other)
        {
            if (instance == null || other == null)
                return null;

            var results = new List<DateTime>();
            while (instance.Value <= other.Value)
            {
                results.Add(instance.Value);
                instance = instance.Value.AddDays(1);
            }
            return results;
        }
    }
}