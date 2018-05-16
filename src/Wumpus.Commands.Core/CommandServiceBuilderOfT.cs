using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Wumpus.Commands
{
    public class CommandServiceBuilder<TContext>
        where TContext : class, ICommandContext<TContext>
    {
        private readonly List<PipelineCallback> _pipelines;
        private readonly List<ModuleInfo> _modules;

        public CommandServiceBuilder()
        {
            _pipelines = new List<PipelineCallback>();
            _modules = new List<ModuleInfo>();
        }

        public CommandServiceBuilder<TContext> AddPipeline(
            PipelineCallback pipeline)
        {
            _pipelines.Add(pipeline);
            return this;
        }

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

        public CommandService<TContext> BuildCommandService()
        {
            return new CommandService<TContext>(_pipelines, _modules);
        }
    }
}
