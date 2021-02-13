using System;

namespace Finite.Commands
{
    /// <summary>
    /// Represents a command path.
    /// </summary>
    public readonly struct CommandPath : IEquatable<CommandPath>
    {
        private const string Delimiter = " ";

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
            RawValue = value ?? string.Empty;

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

            RawValue = value;
            Portion = portion;
        }

        /// <summary>
        /// Gets the unescaped path value.
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
        /// Combines two <see cref="CommandPath"/> values into one.
        /// </summary>
        /// <param name="left">
        /// The first value to combine.
        /// </param>
        /// <param name="right">
        /// The second value to combine.
        /// </param>
        /// <returns>
        /// The combined <see cref="CommandPath"/>.
        /// </returns>
        public static CommandPath Combine(CommandPath left, CommandPath right)
        {
            var path = string.Create(
                left.Value.Length +
                right.Value.Length +
                Delimiter.Length,
                (left, right),
                CreatePath);

            return new CommandPath(path);

            static void CreatePath(Span<char> span,
                (CommandPath, CommandPath) state)
            {
                var (left, right) = state;

                left.Value.CopyTo(span);
                Delimiter.AsSpan().CopyTo(span[left.Value.Length..]);
                right.Value.CopyTo(
                    span[(left.Value.Length + Delimiter.Length)..]);
            }
        }

        /// <summary>
        /// Extracts the command path corresponding to the parent node for this
        /// path.
        /// </summary>
        /// <returns>
        /// The original command path, without the last segment found in it.
        /// <see cref="Empty"/> if this command path refers to a top-level
        /// command path.
        /// </returns>
        public CommandPath GetParentPath()
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
            return new CommandPath(RawValue, portion);
        }

        /// <summary>
        /// Determines whether the beginning of this <see cref="CommandPath"/>
        /// matches the specified <see cref="CommandPath"/>.
        /// </summary>
        /// <param name="parent">
        /// The <see cref="CommandPath"/> to compare.
        /// </param>
        /// <returns>
        /// <code>true</code> if the value matches the beginning of this path;
        /// otherwise, <code>false</code>.
        /// </returns>
        public bool StartsWith(CommandPath parent)
            => Value.StartsWith(parent.Value,
                StringComparison.OrdinalIgnoreCase);

        /// <inheritdoc/>
        public override string ToString()
            => HasValue ? new string(Value) : string.Empty;

        /// <inheritdoc/>
        public bool Equals(CommandPath other)
            => Equals(other, StringComparison.OrdinalIgnoreCase);

        /// <inheritdoc/>
        public bool Equals(CommandPath other, StringComparison comparisonType)
            => !(HasValue || other.HasValue)
                || MemoryExtensions.Equals(Value, other.Value,
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
                ? string.GetHashCode(Value,
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
