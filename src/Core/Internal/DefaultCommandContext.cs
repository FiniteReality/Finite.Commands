using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Finite.Commands
{
    internal sealed class DefaultCommandContext : CommandContext
    {
        public override CommandPath Path { get; set; } = CommandPath.Empty;
        public override IDictionary<object, object?> Items { get; set; }
            = new Dictionary<object, object?>();
        public override IDictionary<string, object?> Parameters { get; set; }
            = new Dictionary<string, object?>();

        public override IServiceProvider Services
            => ServiceScope.ServiceProvider;

        internal IServiceScope ServiceScope { get; set; } = null!;

        internal void Reset()
        {
            Path = CommandPath.Empty;
            Items.Clear();
            Parameters.Clear();
            ServiceScope.Dispose();
        }
    }
}
