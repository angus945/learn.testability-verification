using Module.Verification.SystemFact.Observability;
using TinyArena.Domain;

namespace TinyArena.Console;

internal sealed class DamageConsoleObserver : ISystemFactObserver<DamageApplied>
{
    public void Observe(DamageApplied fact, FactObservationContext context)
    {
        global::System.Console.WriteLine(
            $"Fact #{context.Sequence}: Actor {fact.SourceActorId.Value} dealt {fact.Amount} damage to Actor {fact.TargetActorId.Value}. HP {fact.PreviousHealth} -> {fact.CurrentHealth}");
    }
}