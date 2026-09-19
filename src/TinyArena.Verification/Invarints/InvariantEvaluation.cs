using Module.Verification.Invariant;
using Module.Verification.StateSnapshot;

namespace TinyArena.Verification;

public sealed class InvariantEvaluation
{
    public StateSnapshotReference SnapshotReference { get; }

    public IReadOnlyList<InvariantViolation> Violations { get; }

    public bool IsSatisfied => Violations.Count == 0;

    public InvariantEvaluation(StateSnapshotReference snapshotReference, IEnumerable<InvariantViolation> violations)
    {
        ArgumentNullException.ThrowIfNull(violations);

        SnapshotReference = snapshotReference;
        Violations = Array.AsReadOnly(violations.ToArray());
    }
}