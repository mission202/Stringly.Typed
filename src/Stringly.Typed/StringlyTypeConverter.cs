using System;

namespace StringlyTyped
{
    internal class StringlyTypeConverter<T> : InternalInterfacesTypeConverter<T>
    {
        private readonly Type _targetType = typeof(T);
        private T _instance;
        private Stringly _stringly;

        protected override void OnInstantiation(T instance)
        {
            _stringly = instance as Stringly;

            if (_stringly == null)
                throw new InvalidOperationException($"Type '{_targetType.Name}' doesn't inherit from '{typeof(Stringly).Name}'.");

            _instance = instance;
        }

        protected override bool TryParseInternal(string value, out T result)
        {
            result = default(T);

            string parsed;
            var success = _stringly.TryParse(value, out parsed);

            if (!success) return false;

            _stringly.Value = parsed;
            result = _instance;
            return true;
        }
    }
}