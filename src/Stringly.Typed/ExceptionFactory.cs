using System;

namespace StringlyTyped
{
    internal static class ExceptionFactory
    {
        internal static ArgumentOutOfRangeException IncorrectStringFormat<T>(string value, Exception innerException = null)
        {
            return new ArgumentOutOfRangeException(
                $"Unable to create type '{typeof(T).Name}' from value '{value}'.", innerException);
        }
    }
}