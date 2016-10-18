using System.Text.RegularExpressions;

namespace StringlyTyped
{
    /// <summary>
    /// Base class to allow the simplest form of Stringly.Typed implementation - "just specify a Regex".
    /// </summary>
    public abstract class Stringly : StringlyPattern<string>
    {
        internal string Value;

        protected override string ParseFromRegex(Match match)
        {
            Value = match.Value;
            return Value;
        }

        public override string ToString()
        {
            return Value;
        }
    }

    public class Stringly<T>
    {
        public readonly string Value;
        public readonly T Result;

        public Stringly(string value)
        {
            Value = value;

            var result = default(T);

            var target = typeof(T);
            IStringlyTypeConverter<T> converter = null;

            if (typeof(Stringly).IsAssignableFrom(target))
                converter = new StringlyTypeConverter<T>();

            if (typeof(IStringlyTypeConverter<T>).IsAssignableFrom(target))
                converter = new ImplementedTypeConverter<T>();

            if (converter == null)
                converter = new ReflectionBasedTypeConverter<T>();

            var success = converter.TryParse(value, out result);

            Result = result;

            if (!success)
                throw ExceptionFactory.IncorrectStringFormat<T>(value);
        }

        private Stringly(T value)
        {
            Value = value.ToString();
            Result = value;
        }

        public static implicit operator Stringly<T>(string value)
        {
            return new Stringly<T>(value);
        }

        public static implicit operator Stringly<T>(T value)
        {
            return new Stringly<T>(value);
        }

        public static implicit operator T(Stringly<T> value)
        {
            return value.Result;
        }

        public static implicit operator string(Stringly<T> value)
        {
            return value.Value;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}