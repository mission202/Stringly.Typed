using System;

namespace StringlyTyped
{
    internal class UriTryCreateConverter : IStringlyTypeConverter<Uri>
    {
        private readonly Type _type;

        public UriTryCreateConverter()
        {
            _type = typeof(Uri);
        }

        public bool TryParse(string value, out Uri result)
        {
            return Uri.TryCreate(value, UriKind.Absolute, out result);
        }
    }
}