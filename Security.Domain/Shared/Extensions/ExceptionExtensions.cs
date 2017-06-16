using System;
using System.Collections.Generic;
using System.Text;

namespace Labs.Security.Domain.Shared.Extensions
{
    public static class ExceptionExtensions
    {
        public static string ToText(this Exception exception)
        {
            var type = exception.GetType().Name;
            var message = exception.Message;

            var builder = new StringBuilder()
                .AppendFormat("{0}: {1} ", type, message);

            var next = exception.InnerException;
            if (next != null)
                builder.AppendFormat("{0}", next.ToText());

            return builder.ToString();
        }

        public static string ToDetails(this Exception exception, string action)
        {
            var builder = new StringBuilder()
                .AppendFormat("Context: {0}. ", action)
                .AppendLine()
                .AppendFormat("{0}", exception.ToDetails());

            return builder.ToString();
        }

        public static string ToDetails(this Exception exception)
        {
            var type = exception.GetType().Name;
            var message = exception.Message;
            var stack = exception.StackTrace;

            var builder = new StringBuilder()
                .AppendFormat("{0}: {1}", type, message)
                .AppendLine()
                .AppendFormat("{0}", stack)
                .AppendLine();

            var next = exception.InnerException;
            if (next != null)
                builder.AppendFormat("{0}", next.ToDetails());

            return builder.ToString();
        }

        public static string ToHtml(this Exception exception, string action)
        {
            var builder = new StringBuilder()
                .AppendFormat("<b>Context</b>: {0}. ", action)
                .AppendLine()
                .AppendFormat("{0}", exception.ToHtml());

            return builder.ToString();
        }

        public static string ToHtml(this Exception exception)
        {
            var type = exception.GetType().Name;
            var message = exception.Message;
            var stack = exception.StackTrace;

            var builder = new StringBuilder()
                .AppendFormat("<font color='red'>{0}</font>: <i>{1}</i>", type, message)
                .AppendLine()
                .AppendFormat("<pre>{0}</pre>", stack)
                .AppendLine();

            var next = exception.InnerException;
            if (next != null)
                builder.AppendFormat("{0}", next.ToHtml());

            return builder.ToString();
        }

        public static IEnumerable<Exception> Iterate(this Exception exception)
        {
            if (exception == null)
                yield break;

            yield return exception;

            while (exception.InnerException != null)
            {
                exception = exception.InnerException;
                yield return exception;
            }
        }
    }
}