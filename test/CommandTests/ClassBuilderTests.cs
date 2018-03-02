using System.Linq;
using System.Threading.Tasks;
using Wumpus.Commands;
using Xunit;

namespace Wumpus.Commands.Tests
{
    public class ClassBuilderTests
    {
        [Fact]
        public void IsValidModule()
        {
            Assert.True(ClassBuilder
                .IsValidModule<ValidTestModule, TestContext>());

            Assert.False(ClassBuilder
                .IsValidModule<InvalidTestModule, TestContext>());
        }

        [Fact]
        public void BuildModule()
        {
            var module = ClassBuilder
                .Build<ValidTestModule, TestContext>();

            Assert.Empty(module.Aliases);
            Assert.Empty(module.Attributes);
            Assert.Empty(module.Submodules);

            var cmd = Assert.Single(module.Commands);

            Assert.Equal("derp", Assert.Single(cmd.Aliases));
            Assert.Empty(cmd.Attributes);
            Assert.Equal(module, cmd.Module);
            Assert.NotNull(cmd.Callback);

            var result = cmd.Callback(null, null, new object[]{})
                .GetAwaiter().GetResult();

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public void CommandCallbacks()
        {
            var module = ClassBuilder
                .Build<ModuleBuildTestModule, TestContext>();

            foreach (var cmd in module.Commands)
            {
                var result = cmd.Callback(null, null, new object[]{})
                    .GetAwaiter().GetResult();

                switch (cmd.Aliases.First())
                {
                    case "void":
                    case "Task":
                        Assert.NotNull(result);
                        Assert.True(result.IsSuccess);
                        break;
                    case "Task<ICommandResult>":
                        Assert.Null(result);
                        break;
                    case "Task<TestResult>":
                        Assert.NotNull(result);
                        Assert.IsType(typeof(TestResult), result);
                        Assert.False(result.IsSuccess);
                        break;
                }
            }
        }
    }

    internal class TestContext : ICommandContext
    {
        public string Message { get; set; }
        public string Author { get; set; }
    }

    internal class TestResult : ICommandResult
    {
        public bool IsSuccess
            => false;
    }

    internal class ModuleBuildTestModule : ModuleBase<TestContext>
    {
        [Command("void")]
        public void TestCommandReturningVoid()
        { }

        [Command("Task")]
        public Task TestCommandReturningTask()
            => Task.CompletedTask;

        [Command("Task<ICommandResult>")]
        public Task<ICommandResult> TestCommandReturningTaskICommandResult()
            => Task.FromResult<ICommandResult>(null);

        [Command("Task<TestResult>")]
        public Task<TestResult>
            TestCommandReturningTaskTestResult()
            => Task.FromResult<TestResult>(new TestResult());
    }

    internal class ValidTestModule : ModuleBase<TestContext>
    {
        [Command("derp")]
        public void TestCommand()
        { }
    }

    internal class InvalidTestModule : ModuleBase<TestContext>
    {

    }
}
