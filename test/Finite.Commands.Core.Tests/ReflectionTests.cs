using Finite.Commands.Extensions;
using Xunit;
using System.Reflection;
using System.Threading.Tasks;
using Finite.Commands.Tests;
using System;

namespace Finite.Commands.Core.Tests
{
    public class TestContext : ICommandContext<TestContext>
    {
        public CommandService<TestContext> Commands { get; set; }

        public string Message { get; set; }

        public string Author => Environment.UserDomainName;
    }

    public class ReflectionTests
    { 

        [Fact]
        void FindCommands()
        {
            CommandServiceBuilder<TestContext> builder = new CommandServiceBuilder<TestContext>().AddModules(Assembly.GetExecutingAssembly());
            var cs = builder.BuildCommandService();

            Assert.Equal(1, cs.Modules.Count);
        }
    }

    public class Commands : ModuleBase<TestContext>
    {
        [Command("ping", "bing")]
        public Task Ping()
        {
            Console.WriteLine("Pong");
            return Task.CompletedTask;
        }

        [Command("say")]
        public Task Say([Alias("toSay")] string args)
        {
            Console.WriteLine(args);
            return Task.CompletedTask;
        }
    }
}
