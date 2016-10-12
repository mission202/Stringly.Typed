using System;
using System.Text.RegularExpressions;

namespace StringlyTyped
{
    public abstract class StringlyPattern<T> : IStringlyTypeConverter<T>
    {
        protected abstract Regex Regex { get; }

        protected abstract T ParseFromRegex(Match match);

        public bool TryParse(string value, out T result)
        {
            var match = Regex.Match(value);

            if (match.Success)
            {
                result = ParseFromRegex(match);
                return true;
            }

            result = default(T);
            return false;
        }
    }
}