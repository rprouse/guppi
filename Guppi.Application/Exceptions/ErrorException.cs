using System;

namespace Guppi.Application.Exceptions;

public class ErrorException(string message) : Exception(message)
{
}
