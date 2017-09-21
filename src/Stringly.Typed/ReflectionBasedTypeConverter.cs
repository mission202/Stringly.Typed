using System;
using System.Reflection;

namespace StringlyTyped
{
    internal class ReflectionBasedTypeConverter<T> : IStringlyTypeConverter<T>
    {
        private readonly Type _type;

        public ReflectionBasedTypeConverter()
        {
            _type = typeof(T);
        }

        public bool TryParse(string value, out T result)
        {
            try
            {
                result = default(T);

                var method = _type.GetTypeInfo().GetMethod(
                "TryParse",
                new[] { typeof(string), typeof(T).MakeByRefType() });

                if (method != null)
                {
                    var parameters = new object[] { value, null };
                    var success = (bool)method.Invoke(null, parameters);

                    if (!success) return false;

                    result = (T)parameters[1];
                    return true;
                }

                return false; // No Method "TryParse" Defined
            }
            catch (Exception)
            {
                result = default(T);
                return false;
            }
        }
    }
}