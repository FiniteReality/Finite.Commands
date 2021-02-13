using System;
using System.Globalization;

namespace Finite.Commands
{
    public sealed class IntBinder : IParameterBinder<int>
    {
        public int Bind(ReadOnlySpan<char> text, out bool success)
        {
            success = int.TryParse(text, NumberStyles.Integer,
                CultureInfo.InvariantCulture.NumberFormat, out var value);

            return value;
        }
    }
}
