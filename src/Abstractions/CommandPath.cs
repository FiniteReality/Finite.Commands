using System;
using System.Diagnostics.CodeAnalysis;

namespace Finite.Commands
{
    /// <summary>
    /// Represents a command path.
    /// </summary>
    public readonly struct CommandPath : IEquatable<CommandPath>
    {
        /// <summary>
        /// Represents the empty path. This field is read only.
        /// </summary>
        public static readonly CommandPath Empty = new(string.Empty);

        /// <summary>
        /// Creates a new <see cref="CommandPath"/> with the given
        /// <paramref name="value"/>.
        /// </summary>
        /// <param name="value">
        /// The path for the command.
        /// </param>
        public CommandPath(string? value)
        {
            Value = value ?? string.Empty;

            Portion = Range.All;
        }

        /// <summary>
        /// Creates a new <see cref="CommandPath"/> with the given
        /// <paramref name="portion"/> of <paramref name="value"/>.
        /// </summary>
        /// <param name="value">
        /// The entire path of the command.
        /// </param>
        /// <param name="portion">
        /// The portion of <paramref name="value"/> to use.
        /// </param>
        public CommandPath(string value, Range portion)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));

            if (value == string.Empty)
                throw new ArgumentException(null, nameof(value));

            // N.B. this validates whether portion is valid based on the string.
            _ = portion.GetOffsetAndLength(value.Length);

            Value = value;
            Portion = portion;
        }

        /// <summary>
        /// Gets the unescaped path value.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Gets the range representing the portion of <see cref="Value"/>
        /// which is significant.
        /// </summary>
        public Range Portion { get; }

        /// <summary>
        /// Gets the slice of <see cref="Value"/> which is significant.
        /// </summary>
        public ReadOnlySpan<char> ValueSpan
            => Value.AsSpan()[Portion];

        /// <summary>
        /// Gets a value indicating whether this path empty or not.
        /// </summary>
        [MemberNotNullWhen(true, nameof(Value))]
        public bool HasValue => !ValueSpan.IsEmpty;

        /// <inheritdoc/>
        public override string ToString()
            => HasValue ? new string(ValueSpan) : string.Empty;

        /// <inheritdoc/>
        public bool Equals(CommandPath other)
            => Equals(other, StringComparison.OrdinalIgnoreCase);

        /// <inheritdoc/>
        public bool Equals(CommandPath other, StringComparison comparisonType)
            => !(HasValue || other.HasValue)
                || MemoryExtensions.Equals(ValueSpan, other.ValueSpan,
                    comparisonType);

        /// <inheritdoc/>
        public override bool Equals(object? obj)
            => obj switch
            {
                null => !HasValue,
                CommandPath other => Equals(other),
                _ => false
            };

        /// <inheritdoc/>
        public override int GetHashCode()
            => HasValue
                ? string.GetHashCode(ValueSpan,
                    StringComparison.OrdinalIgnoreCase)
                : 0;

        /// <inheritdoc/>
        public static bool operator ==(CommandPath left, CommandPath right)
            => left.Equals(right);

        /// <inheritdoc/>
        public static bool operator !=(CommandPath left, CommandPath right)
            => !left.Equals(right);
    }
}
