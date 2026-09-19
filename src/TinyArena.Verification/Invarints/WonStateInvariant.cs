using Module.Verification.Invariant;
using TinyArena.Application;
using TinyArena.Domain;

namespace TinyArena.Verification;

public sealed class WonStateInvariant : IInvariant<GameSessionSnapshot>
{
    public string Code => "tinyarena.status.won";

    public InvariantResult Evaluate(GameSessionSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        bool hasLivingEnemy = snapshot.Enemies.Any(enemy => !enemy.IsDead);
        bool shouldBeWon = !snapshot.Player.IsDead && !hasLivingEnemy;

        if (snapshot.Status == GameStatus.Won && !shouldBeWon)
        {
            return InvariantResult.Violated(new InvariantViolation(Code, "Game status is Won but the win condition is not satisfied."));
        }

        if (snapshot.Status != GameStatus.Won && shouldBeWon)
        {
            return InvariantResult.Violated(new InvariantViolation(Code, "Win condition is satisfied but game status is not Won."));
        }

        return InvariantResult.Satisfied();
    }
}