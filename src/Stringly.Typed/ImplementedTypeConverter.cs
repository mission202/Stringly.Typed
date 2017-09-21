using System;
using System.Reflection;

namespace StringlyTyped
{
    internal class ImplementedTypeConverter<T> : IStringlyTypeConverter<T>
    {
        public bool TryParse(string value, out T result)
        {
            var ctor = typeof(T).GetTypeInfo().GetConstructor(Type.EmptyTypes);

            if (ctor == null)
                throw new ArgumentException("Type must have a default paramless constructor.");

            var instance = (IStringlyTypeConverter<T>)ctor.Invoke(null);
            var success = instance.TryParse(value, out result);

            if (!success) return false;

            return true;
        }
    }
}