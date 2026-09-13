using Module.Verification.StateSnapshot;
using Module.Verification.SystemFact;
using Module.Verification.SystemFact.Observability;
using TinyArena.Domain;

namespace TinyArena.Application;

public sealed class GameSessionCommitter
{
    private readonly IGameSessionRepository _repository;
    private readonly GameSessionSnapshotRecorder _snapshotRecorder;
    private readonly ISystemFactSink _factSink;

    public GameSessionCommitter(IGameSessionRepository repository, GameSessionSnapshotRecorder snapshotRecorder, ISystemFactSink factSink)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _snapshotRecorder = snapshotRecorder ?? throw new ArgumentNullException(nameof(snapshotRecorder));
        _factSink = factSink ?? throw new ArgumentNullException(nameof(factSink));
    }

    public void CommitNew(GameSession session)
    {
        ArgumentNullException.ThrowIfNull(session);

        _repository.Add(session);

        PublishCommittedObservations(session);
    }

    public void CommitChanges(GameSession session)
    {
        ArgumentNullException.ThrowIfNull(session);

        _repository.Update(session);

        PublishCommittedObservations(session);
    }

    private void PublishCommittedObservations(GameSession session)
    {
        _snapshotRecorder.Record(session);

        IReadOnlyCollection<ISystemFact> facts = session.ReleaseFacts();

        foreach (ISystemFact fact in facts)
        {
            _factSink.Publish(fact);
        }
    }
}