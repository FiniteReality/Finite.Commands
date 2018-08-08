using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Wumpus.Commands
{
    /// <summary>
    /// A builder which creates instances of
    /// <see cref="CommandService&lt;TContext&gt;"/>.
    /// </summary>
    /// <typeparam name="TContext">
    /// The command context type to use.
    /// </typeparam>
    public class CommandServiceBuilder<TContext>
        where TContext : class, ICommandContext<TContext>
    {
        private readonly List<PipelineCallback> _pipelines;
        private readonly List<ModuleInfo> _modules;

        /// <summary>
        /// Creates a new CommandServiceBuilder for the given context
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
        /// <par>
        /// There should only ever be one command parser, or else conflicts
        /// may arise.
        /// </par>
        /// </summary>
        /// <typeparam name="TParser"></typeparam>
        /// <returns></returns>
        public CommandServiceBuilder<TContext> AddCommandParser<TParser>()
            where TParser : class, ICommandParser
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
                    await parser.ParseAsync<TContext>(ctx)
                        .ConfigureAwait(false);
                    
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
        {
            if (!ClassBuilder.IsValidModule<TModule, TContext>())
                throw new ArgumentException(
                    $"Cannot build {typeof(TModule).Name} " +
                    "as it is not a valid module.");

            var builtModule = ClassBuilder.Build<TModule, TContext>();
            _modules.Add(builtModule);

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
    }
}
