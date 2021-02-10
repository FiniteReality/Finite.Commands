using System;
using Finite.Commands.Parsing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ConsoleCommands
{
    public static class Program
    {
        public static void Main(string[] args)
            => CreateHostBuilder(args).Build().Run();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices(ConfigureServices);
        private static void ConfigureServices(HostBuilderContext context,
            IServiceCollection services)
        {
            // Workaround for: https://github.com/dotnet/runtime/issues/36059
            // (PR was not merged for .NET 5.0)
            _ = services.Configure<HostOptions>(x => x.ShutdownTimeout = TimeSpan.Zero);

            _ = services.AddCommands()
                .AddPositionalCommandParser();

            _ = services.AddHostedService<LineReaderService>();
        }
    }
}
