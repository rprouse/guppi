using System;

namespace Guppi.Application.Exceptions
{
    public class UnconfiguredException : Exception
    {
        public UnconfiguredException(string message) : base(message)
        {
        }
    }
}
