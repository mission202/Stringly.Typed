using System;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace StringlyTyped
{
    internal static class ExceptionFactory
    {
        internal static ArgumentOutOfRangeException IncorrectStringFormat<T>(string value, Exception innerException = null)
        {
            return new ArgumentOutOfRangeException(
                $"Unable to create type '{typeof(T).Name}' from value '{value}'.", innerException);
        }

        internal static RegexMismatchException RegexMismatch(string value, Regex regex)
        {
            return new RegexMismatchException($"Value '{value}' does not match regular expression '{regex}'.", regex);
        }
    }
}