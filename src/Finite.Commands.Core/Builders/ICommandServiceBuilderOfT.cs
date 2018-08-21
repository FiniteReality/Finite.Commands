using System;

namespace Finite.Commands
{
    /// <summary>
    /// A builder which creates instances of
    /// <see cref="CommandService&lt;TContext&gt;"/>.
    /// </summary>
    /// <typeparam name="TContext">
    /// The command context type to use.
    /// </typeparam>
    public interface ICommandServiceBuilder<TContext>
        where TContext : class, ICommandContext<TContext>
    {
        /// <summary>
        /// Adds a pipeline to the created command service.
        /// </summary>
        /// <param name="pipeline">
        /// The pipeline to add.
        /// </param>
        /// <returns>
        /// <code>this</code>
        /// </returns>
        ICommandServiceBuilder<TContext> AddPipeline(PipelineCallback pipeline);

        /// <summary>
        /// Adds a typed pipeline to the created command service.
        /// </summary>
        /// <typeparam name="TPipeline">
        /// The type of the pipeline to add.
        /// </typeparam>
        /// <returns>
        /// <code>this</code>
        /// </returns>
        ICommandServiceBuilder<TContext> AddPipeline<TPipeline>()
            where TPipeline : class, IPipeline;

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
        ICommandServiceBuilder<TContext> AddModule<TModule>()
            where TModule : ModuleBase<TContext>;

        /// <summary>
        /// Adds a typed module to the command service. The module must use the
        /// same context type as the current builder.
        /// </summary>
        /// <param name="moduleType">
        /// The type of the module to add.
        /// </typeparam>
        /// <returns>
        /// <code>this</code>
        /// </returns>
        ICommandServiceBuilder<TContext> AddModule(Type moduleType);

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
        ICommandServiceBuilder<TContext> AddModule(ModuleInfo module);

        /// <summary>
        /// Builds the command service with the given pipelines and modules.
        /// </summary>
        /// <returns>
        /// The built <see cref="CommandService&lt;TContext&gt;"/>.
        /// </returns>
        CommandService<TContext> BuildCommandService();
    }
}
