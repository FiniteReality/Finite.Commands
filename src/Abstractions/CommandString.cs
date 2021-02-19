using System;

namespace Finite.Commands
{
    /// <summary>
    /// Represents a significant portion of a <see cref="string"/> used in a
    /// command.
    /// </summary>
    public readonly struct CommandString : IEquatable<CommandString>
    {
        /// <summary>
        /// Represents the empty string. This field is read only.
        /// </summary>
        public static readonly CommandString Empty = new(string.Empty);

        /// <summary>
        /// Creates a new <see cref="CommandString"/> with the given
        /// <paramref name="value"/>.
        /// </summary>
        /// <param name="value">
        /// The raw value.
        /// </param>
        public CommandString(string? value)
        {
            RawValue = value ?? string.Empty;

            Portion = Range.All;
        }

        /// <summary>
        /// Creates a new <see cref="CommandString"/> with the given
        /// <paramref name="slice"/> of <paramref name="value"/>.
        /// </summary>
        /// <param name="value">
        /// The raw value.
        /// </param>
        /// <param name="slice">
        /// The slice of <paramref name="value"/> to represent.
        /// </param>
        public CommandString(string value, Range slice)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));

            if (value == string.Empty)
                throw new ArgumentException(null, nameof(value));

            // N.B. this validates whether portion is valid based on the string.
            _ = slice.GetOffsetAndLength(value.Length);

            RawValue = value;
            Portion = slice;
        }

        /// <summary>
        /// Gets the raw value of this <see cref="CommandString"/>.
        /// </summary>
        public string RawValue { get; }

        /// <summary>
        /// Gets the range representing the portion of <see cref="RawValue"/>
        /// which is significant.
        /// </summary>
        public Range Portion { get; }

        /// <summary>
        /// Gets the slice of <see cref="RawValue"/> which is significant.
        /// </summary>
        public ReadOnlySpan<char> Value
            => RawValue.AsSpan()[Portion];

        /// <summary>
        /// Gets whether <see cref="Value"/> has a meaningful value.
        /// </summary>
        public bool HasValue
            => !Value.IsEmpty;

        /// <summary>
        /// Determines whether the beginning of this
        /// <see cref="CommandString"/> matches the specified
        /// <see cref="CommandString"/>.
        /// </summary>
        /// <param name="value">
        /// The <see cref="CommandString"/> to compare.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="value"/> matches the beginning of
        /// this command string; otherwise, <c>false</c>.
        /// </returns>
        public bool StartsWith(CommandString value)
            => Value.StartsWith(value.Value,
                StringComparison.OrdinalIgnoreCase);

        /// <inheritdoc/>
        public override string ToString()
            => HasValue ? new string(Value) : string.Empty;

        /// <inheritdoc/>
        public bool Equals(CommandString other)
            => Equals(other, StringComparison.OrdinalIgnoreCase);

        /// <inheritdoc/>
        public bool Equals(CommandString other,
            StringComparison comparisonType)
            => !(HasValue || other.HasValue)
                || MemoryExtensions.Equals(Value, other.Value,
                    comparisonType);

        /// <inheritdoc/>
        public override bool Equals(object? obj)
            => obj switch
            {
                null => !HasValue,
                CommandString other => Equals(other),
                _ => false
            };

        /// <inheritdoc/>
        public override int GetHashCode()
            => HasValue
                ? string.GetHashCode(Value,
                    StringComparison.OrdinalIgnoreCase)
                : 0;

        /// <inheritdoc/>
        public static bool operator ==(CommandString left, CommandString right)
            => left.Equals(right);

        /// <inheritdoc/>
        public static bool operator !=(CommandString left, CommandString right)
            => !left.Equals(right);
    }
}
