using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Finite.Commands
{
    internal sealed class DefaultCommandContext : CommandContext
    {
        public override CommandString Path { get; set; } = CommandString.Empty;
        public override IDictionary<object, object?> Items { get; set; }
            = new Dictionary<object, object?>();
        public override IDictionary<string, object?> Parameters { get; set; }
            = new Dictionary<string, object?>();
        public override ICommand Command { get; set; } = null!;

        public override IServiceProvider Services
            => ServiceScope.ServiceProvider;

        internal IServiceScope ServiceScope { get; set; } = null!;

        internal void Reset()
        {
            Path = CommandString.Empty;
            Items.Clear();
            Parameters.Clear();
            ServiceScope.Dispose();
        }
    }
}
