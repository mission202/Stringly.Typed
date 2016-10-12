using System;
using System.Text.RegularExpressions;

namespace StringlyTyped
{
    public class RegexMismatchException : Exception
    {
        public RegexMismatchException(string message, Regex regex) : base(message) { }
        public RegexMismatchException(string message, Regex regex, Exception innerException) : base(message, innerException) { }
    }
}