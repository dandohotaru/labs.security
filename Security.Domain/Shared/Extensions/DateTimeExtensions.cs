using System;
using System.Globalization;

namespace Infsys.Security.Auth.Core.Shared.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime StartOfDay(this DateTime instance)
        {
            return instance.Date;
        }

        public static DateTime EndOfDay(this DateTime instance)
        {
            return instance.Date.AddDays(1);
        }

        public static DateTime StartOfWeek(this DateTime instance, DayOfWeek weekStart)
        {
            var offset = instance.DayOfWeek - weekStart;
            if (offset < 0)
                offset += 7;

            return instance.AddDays(-1*offset).Date;
        }

        public static DateTime StartOfWeek(this DateTime instance)
        {
            var culture = CultureInfo.CurrentCulture;
            var weekStart = culture.DateTimeFormat.FirstDayOfWeek;
            return StartOfWeek(instance, weekStart);
        }

        public static DateTime EndOfWeek(this DateTime instance, DayOfWeek weekStart)
        {
            return instance
                .StartOfWeek(weekStart)
                .AddDays(5);
        }

        public static DateTime EndOfWeek(this DateTime instance)
        {
            var culture = CultureInfo.CurrentCulture;
            var weekStart = culture.DateTimeFormat.FirstDayOfWeek;
            return EndOfWeek(instance, weekStart);
        }

        public static DateTime? ToDateTime(this string instance)
        {
            if (instance == null)
                return default(DateTime?);

            DateTime result;
            var success = DateTime.TryParse(instance, out result);
            return success
                ? result
                : default(DateTime?);
        }
    }
}