using System;

namespace Guppi.Application.Exceptions
{
    public class WarningException : Exception
    {
        public WarningException(string message) : base(message)
        {
        }
    }
}
