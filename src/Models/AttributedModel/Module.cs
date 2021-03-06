using System.Threading;

namespace Finite.Commands.AttributedModel
{
    /// <summary>
    /// Defines a class which can be used to define command modules.
    /// </summary>
    public abstract class Module
    {
        /// <summary>
        /// Gets the context which this command is executed under.
        /// </summary>
        public CommandContext Context { get; private set; } = null!;

        /// <summary>
        /// Gets a cancellation token which can be used to monitor for command
        /// cancellation.
        /// </summary>
        public CancellationToken CommandCancellation { get; private set; }
            = default;

        /// <summary>
        /// This API supports the product infrastructure and is not intended to
        /// be used directly from your code.
        ///
        /// Sets the <see cref="CommandContext"/> for a given
        /// <see cref="Module"/>.
        /// </summary>
        /// <param name="module">
        /// The module to set the <see cref="Context"/> of.
        /// </param>
        /// <param name="context">
        /// The context to use.
        /// </param>
        public static void SetCommandContext(Module module,
            CommandContext context)
            => module.Context = context;

        /// <summary>
        /// This API supports the product infrastructure and is not intended to
        /// be used directly from your code.
        ///
        /// Sets the <see cref="CancellationToken"/> for a given
        /// <see cref="Module"/>.
        /// </summary>
        /// <param name="module">
        /// The module to set the <see cref="CommandCancellation"/> of.
        /// </param>
        /// <param name="cancellationToken">
        /// The context to use.
        /// </param>
        public static void SetCancellationToken(Module module,
            CancellationToken cancellationToken)
            => module.CommandCancellation = cancellationToken;
    }
}
