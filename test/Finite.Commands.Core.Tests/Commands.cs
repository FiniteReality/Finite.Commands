using System.Threading.Tasks;

namespace Finite.Commands.Tests
{
    internal class TestContext : ICommandContext
    {
        public string Message { get; set; }
        public string Author { get; set; }
    }

    internal class TestResultUnsuccessful : IResult
    {
        public bool IsSuccess
            => false;
    }

    internal class TestResultSuccessful : IResult
    {
        public bool IsSuccess
            => true;
    }

    internal class ValidTestModule : ModuleBase<TestContext>
    {
        [Command("derp")]
        public Task DoCoolThings()
            => Task.CompletedTask;

        protected override void OnExecuting(CommandInfo command)
        {
            base.OnExecuting(command);
        }

        [OnBuilding]
        public static void OnBuilding(ModuleBuilder builder)
        { }
    }

    internal class InvalidTestModuleNoCommands
        : ModuleBase<TestContext>
    { }

    internal class InvalidTestModuleBadCommandSignature
        : ModuleBase<TestContext>
    {
        [Command("derp")]
        public int BadReturnType()
            => 1;
    }

    internal class InvalidTestModuleBadBuildingCallbackSignature
        : ModuleBase<TestContext>
    {
        [OnBuilding]
        public static int BadBuilding(int value)
            => 0;
    }
}
