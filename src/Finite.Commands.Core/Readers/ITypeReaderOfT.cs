namespace Finite.Commands
{
    public interface ITypeReader<T> : ITypeReader
    {
        bool TryRead(string value, out T result);
    }
}
