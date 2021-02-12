using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Finite.Commands.Parsing
{
    /// <summary>
    /// Defines extension methods for adding command parsing infrastructure to
    /// an <see cref="ICommandsBuilder"/>.
    /// </summary>
    public static class CommandParserCommandsBuilderExtensions
    {
        /// <summary>
        /// Adds command parsing infrastructure to the given
        /// <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">
        /// The <see cref="ICommandsBuilder"/> to add parsing infrastructure
        /// to.
        /// </param>
        /// <returns>
        /// The <paramref name="builder"/>.
        /// </returns>
        public static ICommandsBuilder AddPositionalCommandParser(
            this ICommandsBuilder builder)
        {
            builder.Services.TryAddSingleton<ICommandParser, PositionalCommandParser>();

            return builder;
        }
    }
}
