using Module.Verification.SystemFact.Observability;
using TinyArena.Domain;

namespace TinyArena.Console;

internal sealed class ActorDefeatedConsoleObserver : ISystemFactObserver<ActorDefeated>
{
    public void Observe(ActorDefeated fact, FactObservationContext context)
    {
        global::System.Console.WriteLine($"Fact #{context.Sequence}: Actor {fact.DefeatedActorId.Value} was defeated by Actor {fact.SourceActorId.Value}.");
    }
}