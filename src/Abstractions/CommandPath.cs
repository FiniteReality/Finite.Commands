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
        /// The path
        /// </param>
        public CommandPath(string? value)
        {
            Value = value ?? string.Empty;
        }

        /// <summary>
        /// Gets the unescaped path value.
        /// </summary>
        public string? Value { get; }

        /// <summary>
        /// Gets a value indicating whether this path empty or not.
        /// </summary>
        [MemberNotNullWhen(true, nameof(Value))]
        public bool HasValue => !string.IsNullOrEmpty(Value);

        /// <inheritdoc/>
        public override string ToString() => Value ?? string.Empty;

        /// <inheritdoc/>
        public bool Equals(CommandPath other)
            => Equals(other, StringComparison.OrdinalIgnoreCase);

        /// <inheritdoc/>
        public bool Equals(CommandPath other, StringComparison comparisonType)
            => !(HasValue || other.HasValue)
                || string.Equals(Value, other.Value, comparisonType);

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
                ? StringComparer.OrdinalIgnoreCase.GetHashCode(Value)
                : 0;

        /// <inheritdoc/>
        public static bool operator ==(CommandPath left, CommandPath right)
            => left.Equals(right);

        /// <inheritdoc/>
        public static bool operator !=(CommandPath left, CommandPath right)
            => !left.Equals(right);
    }
}
