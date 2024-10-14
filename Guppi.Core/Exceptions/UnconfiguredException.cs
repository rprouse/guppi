using System;

namespace Guppi.Core.Exceptions
{
    public class UnconfiguredException : Exception
    {
        public UnconfiguredException(string message) : base(message)
        {
        }
    }
}
