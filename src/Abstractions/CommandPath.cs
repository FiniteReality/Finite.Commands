using System;

namespace Finite.Commands
{
    /// <summary>
    /// Utilities for using <see cref="CommandString"/> instances as paths in a
    /// command string.
    /// </summary>
    public static class CommandPath
    {
        /// <summary>
        /// Gets the delimiter used to separate path values
        /// </summary>
        public static readonly string Delimiter = " ";

        /// <summary>
        /// Combines two <see cref="CommandString"/> values into one.
        /// </summary>
        /// <remarks>
        /// The values are separated using <see cref="Delimiter"/>.
        /// </remarks>
        /// <param name="first">
        /// The first value to combine.
        /// </param>
        /// <param name="second">
        /// The second value to combine.
        /// </param>
        /// <returns>
        /// The combined <see cref="CommandString"/>.
        /// </returns>
        public static CommandString Combine(CommandString first,
            CommandString second)
        {
            var token = string.Create(
                first.Value.Length +
                Delimiter.Length +
                second.Value.Length,
                (first, second),
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
        /// Extracts the path corresponding to the parent path of the passed
        /// <see cref="CommandString"/>.
        /// </summary>
        /// <param name="path">
        /// The path to compute the parent path of.
        /// </param>
        /// <returns>
        /// The passed <see cref="CommandString"/>, with the last path element
        /// removed.
        /// <see cref="CommandString.Empty"/> if this command path refers to a
        /// top-level command.
        /// </returns>
        public static CommandString GetParentPath(CommandString path)
        {
            if (path.Value.IsEmpty)
                return CommandString.Empty;

            var index = path.Value.LastIndexOf(Delimiter,
                StringComparison.OrdinalIgnoreCase);

            if (index < 0)
                return CommandString.Empty;

            var (start, _) = path.Portion.GetOffsetAndLength(
                path.RawValue.Length);

            var portion = start .. (start + index);
            return new CommandString(path.RawValue, portion);
        }
    }
}
