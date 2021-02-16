using System;
using System.Collections.Generic;

namespace Finite.Commands
{
    /// <summary>
    /// Context object for execution of commands or middleware.
    /// </summary>
    public abstract class CommandContext
    {
        /// <summary>
        /// Gets or sets the command path.
        /// </summary>
        public abstract CommandPath Path { get; set; }

        /// <summary>
        /// Gets or sets a key/value collection that can be used to share data
        /// within the scope of this command.
        /// </summary>
        public abstract IDictionary<object, object?> Items { get; set; }

        /// <summary>
        /// Gets or sets a key/value collection that can be used to store
        /// parameters within the scope of this command.
        /// </summary>
        public abstract IDictionary<string, object?> Parameters { get; set; }

        /// <summary>
        /// Gets or sets the command to execute.
        /// </summary>
        public abstract ICommand Command { get; set; }

        /// <summary>
        /// Gets the services to use during this command.
        /// </summary>
        public abstract IServiceProvider Services { get; }
    }
}
