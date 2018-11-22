using System;

namespace Finite.Commands
{
    public interface ITypeReader
    {
        Type SupportedType { get; }
        bool TryRead(string value, out object result);
    }
}
