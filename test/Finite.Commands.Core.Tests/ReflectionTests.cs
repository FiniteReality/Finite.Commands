using Finite.Commands.Extensions;
using Xunit;
using System.Reflection;
using System.Threading.Tasks;
using Finite.Commands.Tests;
using System;

namespace Finite.Commands.Core.Tests
{
    public class ReflectionTests
    {
        [Fact]
        void FindCommands()
        {
            CommandService<TestContext> cs = new CommandServiceBuilder<TestContext>()
                 .AddModules(Assembly.GetExecutingAssembly())
                 .AddCommandParser<DefaultCommandParser<TestContext>>()
                 .AddTypeReaderFactory<NullTypeReaderFactory>()
                 .BuildCommandService();

            Assert.Equal(1, cs.Modules.Count);
        }
    }
}
