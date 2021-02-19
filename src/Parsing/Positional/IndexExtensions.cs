using System;

namespace Finite.Commands
{
    internal static class IndexExtensions
    {
        /// <summary>
        /// Adds the offset <paramref name="offset"/> to the given
        /// <paramref name="index"/>, accounting for sign and
        /// <see cref="Index.IsFromEnd"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method respects the sign of <paramref name="offset"/> as well
        /// as whether the given <paramref name="index"/> is measured from the
        /// end or the beginning of a sequence.
        /// </para>
        /// <para>
        /// If <paramref name="index"/>'s <see cref="Index.IsFromEnd"/>
        /// property is <c>true</c>, <paramref name="offset"/> will be
        /// subtracted from <paramref name="index"/>'s
        /// <see cref="Index.Value"/>. If <paramref name="index"/>'s
        /// <see cref="Index.IsFromEnd"/> property is <c>false</c>,
        /// <paramref name="offset"/> will be added to the
        /// <paramref name="index"/>'s <see cref="Index.Value"/> instead.
        /// This ensures that an <paramref name="offset"/> of <c>-1</c> is
        /// always treated as "the previous value in the sequence" when read
        /// from start to end, and an <paramref name="offset"/> of <c>1</c> is
        /// always treated as "the next value in the sequence".
        /// </para>
        /// </remarks>
        /// <param name="index">
        /// The <see cref="Index"/> to add an offset to.
        /// </param>
        /// <param name="offset">
        /// The offset to add.
        /// </param>
        /// <returns>
        /// An <see cref="Index"/> with its <see cref="Index.Value"/> property
        /// updated to include the passed <paramref name="offset"/>.
        /// </returns>
        public static Index AddOffset(this Index index, int offset)
            => (offset, index.IsFromEnd) switch {
                (0, _) => index,
                (_, true) => new(index.Value - offset, true),
                (_, false) => new(index.Value + offset, false),
            };
    }
}
