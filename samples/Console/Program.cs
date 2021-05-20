using System;
using System.Threading;
using System.Threading.Tasks;
using ConsoleCommands.Authentication;
using Finite.Commands;
using Finite.Commands.Parsing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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

            _ = services.AddSingleton<PlatformUserMiddleware>();

            _ = services.AddCommands()
                .AddPositionalCommandParser()
                .AddAttributedCommands(x => x.Assemblies.Add(
                    typeof(Program).Assembly.Location))
                .Use(TestMiddlewareAsync)
                .Use<PlatformUserMiddleware>();

            _ = services.AddHostedService<LineReaderService>();
        }

        private static async ValueTask<ICommandResult> TestMiddlewareAsync(
            CommandMiddleware next, CommandContext context,
            CancellationToken cancellationToken)
        {
            var logger = context.Services
                .GetRequiredService<ILoggerFactory>()
                .CreateLogger(typeof(Program).Name);

            logger.LogInformation("Hello world from middleware!");

            return await next();
        }
    }
}
