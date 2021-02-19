using System;

namespace Finite.Commands
{
    internal sealed class StringBinder : IParameterBinder<string>
    {
        public string Bind(IParameter parameter, ReadOnlySpan<char> text,
            out bool success)
        {
            success = true;

            return text.ToString();
        }
    }
}
