using Module.Verification.SystemFact;
using Module.Verification.SystemFact.Observability;

namespace TinyArena.AcceptanceTests;

internal sealed class RecordingSystemFactObserver : ISystemFactObserver<ISystemFact>
{
    private readonly List<ObservedFact> _facts = new List<ObservedFact>();

    public IReadOnlyList<ObservedFact> Facts => _facts;

    public void Observe(ISystemFact fact, FactObservationContext context)
    {
        ObservedFact observedFact = new ObservedFact(fact, context);
        _facts.Add(observedFact);
    }
}
