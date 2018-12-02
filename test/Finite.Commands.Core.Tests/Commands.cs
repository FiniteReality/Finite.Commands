using System.Threading.Tasks;

namespace Finite.Commands.Tests
{
    public class TestContext : ICommandContext
    {
        public string Message { get; set; }
        public string Author { get; set; }
    }

    public class TestResultUnsuccessful : IResult
    {
        public bool IsSuccess
            => false;
    }

    public class TestResultSuccessful : IResult
    {
        public bool IsSuccess
            => true;
    }

    public class ValidTestModule : ModuleBase<TestContext>
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

    public class InvalidTestModuleNoCommands
        : ModuleBase<TestContext>
    { }

    public class InvalidTestModuleBadCommandSignature
        : ModuleBase<TestContext>
    {
        [Command("derp")]
        public int BadReturnType()
            => 1;
    }

    public class InvalidTestModuleBadBuildingCallbackSignature
        : ModuleBase<TestContext>
    {
        [OnBuilding]
        public static int BadBuilding(int value)
            => 0;
    }
}
