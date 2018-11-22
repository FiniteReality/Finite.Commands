using System;

namespace Finite.Commands
{
    public interface ITypeReaderFactory
    {
        bool TryGetTypeReader<T>(out ITypeReader<T> reader);

        bool TryGetTypeReader(Type valueType, out ITypeReader reader);
    }
}
