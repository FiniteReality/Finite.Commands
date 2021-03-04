using System;
using Finite.Commands;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for setting up commands services in an
    /// <see cref="IServiceCollection"/>.
    /// </summary>
    public static class CommandsServiceCollectionExtensions
    {
        /// <summary>
        /// Adds commands services to the specified
        /// <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">
        /// The <see cref="IServiceCollection"/> to add services to.
        /// </param>
        /// <returns>
        /// An <see cref="ICommandsBuilder"/> that can be used to further
        /// configure the commands services.
        /// </returns>
        public static ICommandsBuilder AddCommands(
            this IServiceCollection services)
        {
            if (services is null)
                throw new ArgumentNullException(nameof(services));

            services.TryAddSingleton<CommandHostedService>();
            services.TryAddSingleton<ICommandContextFactory, DefaultCommandContextFactory>();

            services.TryAddScoped<CommandExecutor>();
            services.TryAddScoped<ICommandStore, DefaultCommandStore>();
            services.TryAddScoped<ICommandResultExecutorFactory, DefaultCommandResultExecutorFactory>();
            services.TryAddScoped<IParameterBinderFactory, DefaultParameterBinderFactory>();

            services.TryAddTransient(typeof(CommandResultExecutorWrapper<>));
            services.TryAddTransient<ICommandResultExecutor<NoContentCommandResult>, NoContentCommandResultExecutor>();

            BinderUtility.AddAllBinders(services);

            _ = services.AddTransient(typeof(ParameterBinderWrapper<>));

            _ = services.AddHostedService(
                x => x.GetRequiredService<CommandHostedService>());
            services.TryAddSingleton<ICommandExecutor>(
                x => x.GetRequiredService<CommandHostedService>());

            var middlewareProvider = new CommandMiddlewareProvider();
            services.TryAddSingleton(middlewareProvider);

            return new CommandsBuilder(middlewareProvider, services);
        }
    }
}
