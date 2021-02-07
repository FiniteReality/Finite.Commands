using System;

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
    }
}
