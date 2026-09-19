using Module.Verification.Invariant;
using TinyArena.Application;
using TinyArena.Domain;

namespace TinyArena.Verification;

public sealed class ActorOverlapInvariant : IInvariant<GameSessionSnapshot>
{
    public string Code => "tinyarena.actor.overlap";

    public InvariantResult Evaluate(GameSessionSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        HashSet<Position> occupiedPositions = new HashSet<Position>();

        if (!snapshot.Player.IsDead)
        {
            occupiedPositions.Add(snapshot.Player.Position);
        }

        foreach (ActorSnapshot enemy in snapshot.Enemies)
        {
            if (enemy.IsDead)
            {
                continue;
            }

            if (!occupiedPositions.Add(enemy.Position))
            {
                string detail = $"Living actor {enemy.Id.Value} overlaps another living actor at ({enemy.Position.X}, {enemy.Position.Y}).";

                return InvariantResult.Violated(new InvariantViolation(Code, detail));
            }
        }

        return InvariantResult.Satisfied();
    }
}