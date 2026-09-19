using Module.Verification.Invariant;
using TinyArena.Application;

namespace TinyArena.Verification;

public sealed class HealthRangeInvariant : IInvariant<GameSessionSnapshot>
{
    public string Code => "tinyarena.health.range";

    public InvariantResult Evaluate(GameSessionSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        if (!IsHealthValid(snapshot.Player))
        {
            return CreateViolation(snapshot.Player);
        }

        foreach (ActorSnapshot enemy in snapshot.Enemies)
        {
            if (!IsHealthValid(enemy))
            {
                return CreateViolation(enemy);
            }
        }

        return InvariantResult.Satisfied();
    }

    private static bool IsHealthValid(ActorSnapshot actor)
    {
        return actor.MaximumHealth > 0 &&
               actor.CurrentHealth >= 0 &&
               actor.CurrentHealth <= actor.MaximumHealth;
    }

    private InvariantResult CreateViolation(ActorSnapshot actor)
    {
        string detail = $"Actor {actor.Id.Value} has invalid health: {actor.CurrentHealth}/{actor.MaximumHealth}.";

        return InvariantResult.Violated(new InvariantViolation(Code, detail));
    }
}