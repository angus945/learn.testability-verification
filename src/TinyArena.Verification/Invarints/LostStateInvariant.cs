using Module.Verification.Invariant;
using TinyArena.Application;
using TinyArena.Domain;

namespace TinyArena.Verification;

public sealed class LostStateInvariant : IInvariant<GameSessionSnapshot>
{
    public string Code => "tinyarena.status.lost";

    public InvariantResult Evaluate(GameSessionSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        bool shouldBeLost = snapshot.Player.IsDead;

        if (snapshot.Status == GameStatus.Lost && !shouldBeLost)
        {
            return InvariantResult.Violated(new InvariantViolation(Code, "Game status is Lost while the player is still alive."));
        }

        if (snapshot.Status != GameStatus.Lost && shouldBeLost)
        {
            return InvariantResult.Violated(new InvariantViolation(Code, "Player is dead but game status is not Lost."));
        }

        return InvariantResult.Satisfied();
    }
}