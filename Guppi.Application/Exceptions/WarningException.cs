using System;

namespace Guppi.Application.Exceptions;

public class WarningException(string message) : Exception(message)
{
}
