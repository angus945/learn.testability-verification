
using Module.Verification.Invariant;
using Module.Verification.StateSnapshot;
using TinyArena.Application;

namespace TinyArena.Verification;

public sealed class GameSessionInvariantEvaluator
{
    private readonly InvariantRegistry<GameSessionSnapshot> _registry;

    public GameSessionInvariantEvaluator(InvariantRegistry<GameSessionSnapshot> registry)
    {
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
    }

    public InvariantEvaluation Evaluate(StateSnapshotReference snapshotReference, GameSessionSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        IReadOnlyList<InvariantViolation> violations = _registry.Evaluate(snapshot);

        return new InvariantEvaluation(snapshotReference, violations);
    }
}