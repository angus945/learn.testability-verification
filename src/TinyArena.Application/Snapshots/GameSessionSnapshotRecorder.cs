using Module.Verification.StateSnapshot;
using TinyArena.Domain;

namespace TinyArena.Application;

public sealed class GameSessionSnapshotRecorder
{
    private readonly IGameSessionSnapshotCapturer _capturer;
    private readonly IStateSnapshotPublisher<GameSessionSnapshot> _publisher;

    public GameSessionSnapshotRecorder(IGameSessionSnapshotCapturer capturer, IStateSnapshotPublisher<GameSessionSnapshot> publisher)
    {
        _capturer = capturer ?? throw new ArgumentNullException(nameof(capturer));
        _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
    }

    public void Record(GameSession session)
    {
        ArgumentNullException.ThrowIfNull(session);

        GameSessionSnapshot snapshot;

        try
        {
            snapshot = _capturer.Capture(session);
        }
        catch (Exception exception)
        {
            ReportCaptureFailure(exception);
            return;
        }

        TryPublish(snapshot);
    }

    private void ReportCaptureFailure(Exception exception)
    {
        StateSnapshotCaptureFailure failure = new StateSnapshotCaptureFailure("tinyarena.game-session.capture-failed", exception.Message);

        try
        {
            _publisher.ReportStateSnapshotCaptureFailure(failure);
        }
        catch (Exception)
        {
        }
    }

    private void TryPublish(GameSessionSnapshot snapshot)
    {
        try
        {
            _publisher.Publish(snapshot);
        }
        catch (Exception)
        {
        }
    }
}