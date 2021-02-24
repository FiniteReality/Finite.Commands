using System;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace Finite.Commands
{
    /// <summary>
    /// Defines an interface which provides instances of <see cref="ICommand"/>.
    /// </summary>
    public interface ICommandProvider
    {
        /// <summary>
        /// Gets all of the commands registered by this provider.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> containing all commands registered
        /// by this provider.
        /// /// </returns>
        IEnumerable<ICommand> GetCommands();

        /// <summary>
        /// Gets a change token which can be used to signal when the set of
        /// commands managed by this provider changes.
        /// </summary>
        /// <returns>
        /// A <see cref="IChangeToken"/> which can be used to monitor for
        /// changes.
        /// </returns>
        IChangeToken GetChangeToken();
    }
}
