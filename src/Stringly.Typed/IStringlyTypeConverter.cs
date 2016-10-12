namespace StringlyTyped
{
    public interface IStringlyTypeConverter<T>
    {
        bool TryParse(string value, out T result);
    }
}