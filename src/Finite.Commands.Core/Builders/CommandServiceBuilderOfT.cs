using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Finite.Commands
{
    /// <inheritdoc/>
    public sealed class CommandServiceBuilder<TContext>
        : ICommandServiceBuilder<TContext>
        where TContext : class, ICommandContext<TContext>
    {
        private readonly List<PipelineCallback> _pipelines;

        internal CommandServiceBuilder<TContext> AddModules()
        {
            throw new NotImplementedException();
        }

        private readonly List<ModuleInfo> _modules;

        /// <summary>
        /// Creates a new <see cref="CommandServiceBuilder&lt;TContext&gt;"/>
        /// for the given context.
        /// </summary>
        public CommandServiceBuilder()
        {
            _pipelines = new List<PipelineCallback>();
            _modules = new List<ModuleInfo>();
        }

        /// <summary>
        /// Adds a pipeline to the created command service.
        /// </summary>
        /// <param name="pipeline">
        /// The pipeline to add.
        /// </param>
        /// <returns>
        /// <code>this</code>
        /// </returns>
        public CommandServiceBuilder<TContext> AddPipeline(
            PipelineCallback pipeline)
        {
            _pipelines.Add(pipeline);
            return this;
        }

        /// <summary>
        /// Adds a typed pipeline to the created command service.
        /// </summary>
        /// <typeparam name="TPipeline">
        /// The type of the pipeline to add.
        /// </typeparam>
        /// <returns>
        /// <code>this</code>
        /// </returns>
        public CommandServiceBuilder<TContext> AddPipeline<TPipeline>()
            where TPipeline : class, IPipeline
        {
            var factory = ActivatorUtilities.CreateFactory(
                typeof(TPipeline),
                Array.Empty<Type>());

            _pipelines.Add(async (ctx, next) =>
            {
                var pipeline = factory(ctx.ServiceProvider,
                    Array.Empty<object>()) as TPipeline;

                try
                {
                    return await pipeline.ExecuteAsync(ctx, next)
                        .ConfigureAwait(false);
                }
                finally
                {
                    (pipeline as IDisposable)?.Dispose();
                }
            });

            return this;
        }

        /// <summary>
        /// Adds a command parser to the command service.
        /// </summary>
        /// <typeparam name="TParser">
        /// The parser to add.
        /// </typeparam>
        /// <returns>
        /// Returns a <see cref="ICommandServiceBuilder&lt;TContext&gt;"/> for
        /// chaining calls.
        /// </returns>
        public ICommandServiceBuilder<TContext> AddCommandParser<TParser>()
            where TParser : class, ICommandParser<TContext>
        {
            var factory = ActivatorUtilities.CreateFactory(
                typeof(TParser),
                Array.Empty<Type>());

            _pipelines.Add(async (ctx, next) =>
            {
                var parser = factory(ctx.ServiceProvider,
                    Array.Empty<object>()) as TParser;

                try
                {
                    var result = parser.Parse(ctx);

                    if (!result.IsSuccess)
                        return result;

                    return await next().ConfigureAwait(false);
                }
                finally
                {
                    (parser as IDisposable)?.Dispose();
                }
            });

            return this;
        }

        /// <summary>
        /// Adds a typed module to the command service. The module must use the
        /// same context type as the current builder.
        /// </summary>
        /// <typeparam name="TModule">
        /// The type of the module to add.
        /// </typeparam>
        /// <returns>
        /// <code>this</code>
        /// </returns>
        public CommandServiceBuilder<TContext> AddModule<TModule>()
            where TModule : ModuleBase<TContext>
            => AddModule(typeof(TModule));

        /// <summary>
        /// Adds a typed module to the command service. The module must use the
        /// same context type as the current builder.
        /// </summary>
        /// <param name="moduleType">
        /// The type of the module to add.
        /// </param>
        /// <returns>
        /// <code>this</code>
        /// </returns>
        public CommandServiceBuilder<TContext> AddModule(Type moduleType)
        {
            var builtModule = ClassBuilder.Build<TContext>(moduleType);
            _modules.Add(builtModule);

            return this;
        }

        /// <summary>
        /// Adds an existing module to the command service. The module must use
        /// the same context type as the current builder.
        /// </summary>
        /// <param name="module">
        /// The module to add.
        /// </param>
        /// <returns>
        /// <code>this</code>
        /// </returns>
        public CommandServiceBuilder<TContext> AddModule(ModuleInfo module)
        {
            if (!module.ContextType.IsAssignableFrom(typeof(TContext)))
                throw new ArgumentException(
                    $"{typeof(TContext).FullName} does not inherit or " +
                    $"implement {module.ContextType.FullName}, so cannot be " +
                    "added to the built command service.",
                    nameof(module));

            _modules.Add(module);

            return this;
        }

        /// <summary>
        /// Builds the command service with the given pipelines and modules.
        /// </summary>
        /// <returns>
        /// The built <see cref="CommandService&lt;TContext&gt;"/>.
        /// </returns>
        public CommandService<TContext> BuildCommandService()
        {
            return new CommandService<TContext>(_pipelines, _modules);
        }

        /// <inheritdoc/>
        ICommandServiceBuilder<TContext> ICommandServiceBuilder<TContext>
            .AddPipeline(PipelineCallback pipeline)
            => AddPipeline(pipeline);

        /// <inheritdoc/>
        ICommandServiceBuilder<TContext> ICommandServiceBuilder<TContext>
            .AddPipeline<TPipeline>()
            => AddPipeline<TPipeline>();

        /// <inheritdoc/>
        ICommandServiceBuilder<TContext> ICommandServiceBuilder<TContext>
            .AddModule<TModule>()
            => AddModule<TModule>();

        /// <inheritdoc/>
        ICommandServiceBuilder<TContext> ICommandServiceBuilder<TContext>
            .AddModule(Type moduleType)
            => AddModule(moduleType);

        /// <inheritdoc/>
        ICommandServiceBuilder<TContext> ICommandServiceBuilder<TContext>
            .AddModule(ModuleInfo module)
            => AddModule(module);

        /// <inheritdoc/>
        CommandService<TContext> ICommandServiceBuilder<TContext>
            .BuildCommandService()
            => BuildCommandService();
    }
}
