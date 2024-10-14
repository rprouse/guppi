using System;

namespace Guppi.Core.Exceptions;

public class WarningException(string message) : Exception(message)
{
}
