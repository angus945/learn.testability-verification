using Module.Verification.SystemFact.Observability;
using TinyArena.Domain;

namespace TinyArena.Console;

internal sealed class BattleEndedConsoleObserver : ISystemFactObserver<BattleEnded>
{
    public void Observe(BattleEnded fact, FactObservationContext context)
    {
        global::System.Console.WriteLine($"Fact #{context.Sequence}: Battle ended with status {fact.FinalStatus}.");
    }
}