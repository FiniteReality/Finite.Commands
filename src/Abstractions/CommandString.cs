using System;

namespace Finite.Commands
{
    /// <summary>
    /// Represents a significant token in a command.
    /// </summary>
    public readonly struct CommandString : IEquatable<CommandString>
    {
        private const string Delimiter = " ";

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
        /// <paramref name="portion"/> of <paramref name="value"/>.
        /// </summary>
        /// <param name="value">
        /// The raw value.
        /// </param>
        /// <param name="portion">
        /// The token within <paramref name="value"/> to represent.
        /// </param>
        public CommandString(string value, Range portion)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));

            if (value == string.Empty)
                throw new ArgumentException(null, nameof(value));

            // N.B. this validates whether portion is valid based on the string.
            _ = portion.GetOffsetAndLength(value.Length);

            RawValue = value;
            Portion = portion;
        }

        /// <summary>
        /// Gets the unescaped token value.
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
        /// Combines two <see cref="CommandString"/> values into one.
        /// </summary>
        /// <param name="left">
        /// The first value to combine.
        /// </param>
        /// <param name="right">
        /// The second value to combine.
        /// </param>
        /// <returns>
        /// The combined <see cref="CommandString"/>.
        /// </returns>
        public static CommandString Combine(CommandString left,
            CommandString right)
        {
            var token = string.Create(
                left.Value.Length +
                right.Value.Length +
                Delimiter.Length,
                (left, right),
                CreatePath);

            return new CommandString(token);

            static void CreatePath(Span<char> span,
                (CommandString, CommandString) state)
            {
                var (left, right) = state;

                left.Value.CopyTo(span);
                Delimiter.AsSpan().CopyTo(span[left.Value.Length..]);
                right.Value.CopyTo(
                    span[(left.Value.Length + Delimiter.Length)..]);
            }
        }

        /// <summary>
        /// Extracts the command string corresponding to the previous token for
        /// this string.
        /// </summary>
        /// <returns>
        /// The command string, with the last token removed.
        /// <see cref="Empty"/> if this command string contains no more tokens.
        /// </returns>
        public CommandString GetPreviousToken()
        {
            if (Value.IsEmpty)
                return Empty;

            var index = Value.LastIndexOf(Delimiter,
                StringComparison.OrdinalIgnoreCase);

            if (index < 0)
                return Empty;

            var (start, _) = Portion.GetOffsetAndLength(
                RawValue.Length);

            var portion = start .. (start + index);
            return new CommandString(RawValue, portion);
        }

        /// <summary>
        /// Determines whether the beginning of this
        /// <see cref="CommandString"/> matches the specified
        /// <see cref="CommandString"/>.
        /// </summary>
        /// <param name="parent">
        /// The <see cref="CommandString"/> to compare.
        /// </param>
        /// <returns>
        /// <c>true</c> if the value matches the beginning of this command
        /// string; otherwise, <c>false</c>.
        /// </returns>
        public bool StartsWith(CommandString parent)
            => Value.StartsWith(parent.Value,
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
