using System;

namespace Guppi.Core.Exceptions;

public class ErrorException(string message) : Exception(message)
{
}
