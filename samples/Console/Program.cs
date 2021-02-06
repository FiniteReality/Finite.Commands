using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
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

            services.AddCommands();

            _ = services.AddHostedService<LineReaderService>();
        }
    }
}
