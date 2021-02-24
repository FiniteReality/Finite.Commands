using System;
using Finite.Commands.AttributedModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Finite.Commands
{
    /// <summary>
    /// Defines extension methods for adding commands using the attributed
    /// model to an <see cref="ICommandsBuilder"/>.
    /// </summary>
    public static class AttributedModelCommandsBuilderExtensions
    {
        /// <summary>
        /// Adds commands using the attributed model to the given
        /// <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">
        /// The <see cref="ICommandsBuilder"/> to add attributed model commands
        /// to.
        /// </param>
        /// <returns>
        /// The <paramref name="builder"/>.
        /// </returns>
        public static ICommandsBuilder AddAttributedCommands(
            this ICommandsBuilder builder)
            => AddAttributedCommands(builder, static (_) => {});

        /// <summary>
        /// Adds commands using the attributed model to the given
        /// <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">
        /// The <see cref="ICommandsBuilder"/> to add attributed model commands
        /// to.
        /// </param>
        /// <param name="configure">
        /// A delegate to configure the attributed model.
        /// </param>
        /// <returns>
        /// The <paramref name="builder"/>.
        /// </returns>
        public static ICommandsBuilder AddAttributedCommands(
            this ICommandsBuilder builder,
            Action<AttributedCommandOptions> configure)
        {
            _ = builder.Services.AddSingleton<ICommandProvider, CommandBuilder>();
            _ = builder.Services.Configure(configure);

            return builder;
        }
    }
}
