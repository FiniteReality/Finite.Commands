using System;
using System.Runtime.InteropServices;

namespace Finite.Commands.Binders
{
    internal sealed class BoolBinder : IParameterBinder<bool>
    {
        public bool Bind(IParameter parameter, ReadOnlySpan<char> text,
            out bool success)
        {
            success = bool.TryParse(text, out var value);

            return value;
        }
    }
}
