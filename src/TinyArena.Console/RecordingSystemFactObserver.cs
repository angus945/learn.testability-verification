using System.Collections.Generic;
using Module.Verification.SystemFact;
using Module.Verification.SystemFact.Observability;

namespace TinyArena.AcceptanceTests;

internal sealed class RecordingSystemFactObserver : ISystemFactObserver<ISystemFact>
{
    private readonly List<ObservedFact> _facts = new List<ObservedFact>();

    public IReadOnlyList<ObservedFact> Facts => _facts;

    public void Observe(ISystemFact fact, FactObservationContext context)
    {
        _facts.Add(new ObservedFact(fact, context));
    }
}