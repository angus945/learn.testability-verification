using Module.Verification.SystemFact.Observability;
using TinyArena.Application;

namespace TinyArena.Console;

internal sealed class PlayerActionConsoleObserver : ISystemFactObserver<PlayerActionCommitted>
{
    public void Observe(PlayerActionCommitted fact, FactObservationContext context)
    {
        global::System.Console.WriteLine(
            $"Fact #{context.Sequence}: Player action committed for session {fact.SessionId.Value}.");
    }
}