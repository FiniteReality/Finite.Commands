using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Finite.Commands;
using Microsoft.Extensions.Logging;

namespace ConsoleCommands.Authentication
{
    internal class PlatformUserMiddleware : ICommandMiddleware
    {
        private static readonly Random Rng = new Random();
        private readonly ILogger _logger;

        public PlatformUserMiddleware(ILogger<PlatformUserMiddleware> logger)
        {
            _logger = logger;
        }

        public ValueTask<ICommandResult> ExecuteAsync(CommandMiddleware next,
            CommandContext context, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (OperatingSystem.IsWindows())
            {
                var identity = WindowsIdentity.GetCurrent();

                context.User.AddIdentity(identity);
            }
            else
            {
                context.User.AddIdentity(
                    new ClaimsIdentity(
                        new[]
                        {
                            new Claim(ClaimTypes.Name, Environment.UserName),
                        }
                    ));
            }

            if (Rng.NextDouble() > 0.5)
            {
                _logger.LogDebug("Adding cool role");
                context.User.AddIdentity(
                    new ClaimsIdentity(
                        new[]
                        {
                            new Claim(ClaimTypes.Role, "Cool")
                        }
                    ));
            }

            var nameClaim = context.User
                .FindFirst(x => x.Type == ClaimTypes.Name);

            if (nameClaim != null)
                _logger.LogInformation("Loaded user identity as {name}",
                    nameClaim.Value);
            else
                _logger.LogWarning("Unable to load user identity");

            return next();
        }
    }
}
