using System.Linq;
using System.Threading.Tasks;
using Finite.Commands;
using Xunit;

namespace Finite.Commands.Tests
{
    public class ClassBuilderTests
    {
        private static TestContext Context = new TestContext();

        [Fact]
        public void IsValidModule()
        {
            Assert.True(ClassBuilder
                .IsValidModule<ValidTestModule, TestContext>());

            Assert.False(ClassBuilder
                .IsValidModule<InvalidTestModuleNoCommands, TestContext>());

            Assert.False(ClassBuilder
                .IsValidModule<InvalidTestModuleBadCommandSignature,
                    TestContext>());

            Assert.False(ClassBuilder
                .IsValidModule<InvalidTestModuleBadBuildingCallbackSignature,
                    TestContext>());
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

            var result = cmd.ExecuteAsync(Context, null, new object[]{})
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
                var result = cmd.ExecuteAsync(Context, null, new object[]{})
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
                    case "Task<TestResultUnsuccessful>":
                        Assert.NotNull(result);
                        Assert.IsType(typeof(TestResultUnsuccessful), result);
                        Assert.False(result.IsSuccess);
                        break;
                    case "Task<TestResultSuccessful>":
                        Assert.NotNull(result);
                        Assert.IsType(typeof(TestResultSuccessful), result);
                        Assert.True(result.IsSuccess);
                        break;
                }
            }
        }

        private class ModuleBuildTestModule : ModuleBase<TestContext>
        {
            [Command("Task")]
            public Task TestCommandReturningTask()
                => Task.CompletedTask;

            [Command("Task<ICommandResult>")]
            public Task<IResult> TestCommandReturningTaskICommandResult()
                => Task.FromResult<IResult>(null);

            [Command("Task<TestResultUnsuccessful>")]
            public Task<TestResultUnsuccessful>
                TestCommandReturningTaskTestResultUnsuccessful()
                => Task.FromResult<TestResultUnsuccessful>(
                    new TestResultUnsuccessful());

            [Command("Task<TestResultSuccessful>")]
            public Task<TestResultSuccessful>
                TestCommandReturningTaskTestResultSuccessful()
                => Task.FromResult<TestResultSuccessful>(
                    new TestResultSuccessful());
        }
    }
}
