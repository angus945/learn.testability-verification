using Module.Verification.Invariant;
using TinyArena.Application;

namespace TinyArena.Verification;

public sealed class ActorPositionInvariant : IInvariant<GameSessionSnapshot>
{
    public string Code => "tinyarena.actor.position";

    public InvariantResult Evaluate(GameSessionSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        if (!IsInsideBoard(snapshot.Player, snapshot))
        {
            return CreateViolation(snapshot.Player);
        }

        foreach (ActorSnapshot enemy in snapshot.Enemies)
        {
            if (!IsInsideBoard(enemy, snapshot))
            {
                return CreateViolation(enemy);
            }
        }

        return InvariantResult.Satisfied();
    }

    private static bool IsInsideBoard(ActorSnapshot actor, GameSessionSnapshot snapshot)
    {
        return actor.Position.X >= 0 &&
               actor.Position.X < snapshot.Width &&
               actor.Position.Y >= 0 &&
               actor.Position.Y < snapshot.Height;
    }

    private InvariantResult CreateViolation(ActorSnapshot actor)
    {
        string detail = $"Actor {actor.Id.Value} is outside the board at ({actor.Position.X}, {actor.Position.Y}).";

        return InvariantResult.Violated(new InvariantViolation(Code, detail));
    }
}