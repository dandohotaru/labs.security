using System;

namespace Infsys.Security.Auth.Core.Shared.Exceptions
{
    public class GuardException : Exception
    {
        public GuardException()
        {
        }

        public GuardException(string message)
            : base(message)
        {
        }

        public GuardException(string format, params object[] arguments)
            : base(string.Format(format, arguments))
        {
        }

        public GuardException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}