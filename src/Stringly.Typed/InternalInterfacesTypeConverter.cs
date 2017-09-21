using System;
using System.Reflection;

namespace StringlyTyped
{
    internal abstract class InternalInterfacesTypeConverter<T> : IStringlyTypeConverter<T>
    {
        private readonly Type _targetType = typeof(T);

        public bool TryParse(string value, out T result)
        {
            var ctor = _targetType.GetTypeInfo().GetConstructor(Type.EmptyTypes);

            if (ctor == null)
                throw new ArgumentException($"Type '{_targetType.Name}' must have a default paramless constructor.");

            result = default(T);

            var instance = (T)ctor.Invoke(null);

            OnInstantiation(instance);

            var success = TryParseInternal(value, out result);

            if (!success) return false;

            OnSuccessfulParse(value);

            result = instance;
            return true;
        }

        protected virtual void OnInstantiation(T instance) { }

        protected abstract bool TryParseInternal(string value, out T result);

        protected virtual void OnSuccessfulParse(string value) { }
    }
}