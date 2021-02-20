using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Finite.Commands.Binders
{
    /// <summary>
    /// An implementation of <see cref="IParameterBinder{T}"/> for binding
    /// parameters of type <see cref="Guid"/>.
    /// </summary>
    public sealed class GuidBinder : IParameterBinder<Guid>
    {
        /// <summary>
        /// Gets an object which can be used as a key in
        /// <see cref="IParameter.Data"/> to specify a specific format to use
        /// instead of the default.
        /// </summary>
        public static object UseFormat { get; } = new object();

        private readonly char _format;

        /// <summary>
        /// Creates a new <see cref="GuidBinder"/>.
        /// </summary>
        /// <remarks>
        /// This constructor uses the 'B' format by default, which includes
        /// Guids such as
        /// <c>{d85b1407-351d-4694-9392-03acc5870eb1}</c>.
        /// </remarks>
        public GuidBinder()
            : this('B')
        { }

        /// <summary>
        /// Creates a new <see cref="GuidBinder"/> with the given format.
        /// </summary>
        /// <param name="format">
        /// The format to parse as. This is passed directly to
        /// <see cref="Guid.TryParseExact(ReadOnlySpan{char}, ReadOnlySpan{char}, out Guid)"/>.
        /// </param>
        public GuidBinder(char format)
        {
            _format = format;
        }

        /// <inheritdoc/>
        public Guid Bind(IParameter parameter, ReadOnlySpan<char> text,
            out bool success)
        {
            if (!parameter.TryGetData(UseFormat, out char format))
                format = _format;

            var formatSpan = MemoryMarshal.CreateReadOnlySpan(ref format, 1);
            success = Guid.TryParseExact(text, formatSpan, out var value);

            return value;
        }
    }
}
