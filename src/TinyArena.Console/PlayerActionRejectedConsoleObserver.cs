using Module.Verification.SystemFact.Observability;
using TinyArena.Application;

namespace TinyArena.Console;

internal sealed class PlayerActionRejectedConsoleObserver : ISystemFactObserver<PlayerActionRejected>
{
    public void Observe(PlayerActionRejected fact, FactObservationContext context)
    {
        global::System.Console.WriteLine(
            $"Fact #{context.Sequence}: Player action rejected in session {fact.SessionId.Value}. Reason: {fact.Reason}");
    }
}