using System;

namespace Finite.Commands
{
    public sealed class StringBinder : IParameterBinder<string>
    {
        public string Bind(ReadOnlySpan<char> text, out bool success)
        {
            success = true;

            return text.ToString();
        }
    }
}
