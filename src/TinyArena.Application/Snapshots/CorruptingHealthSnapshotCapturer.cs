using TinyArena.Domain;

namespace TinyArena.Application;

internal sealed class CorruptingHealthSnapshotCapturer : IGameSessionSnapshotCapturer
{
    private readonly GameSessionSnapshotCapturer _inner = new GameSessionSnapshotCapturer();

    public GameSessionSnapshot Capture(GameSession session)
    {
        GameSessionSnapshot snapshot = _inner.Capture(session);

        ActorSnapshot corruptedPlayer = new ActorSnapshot(snapshot.Player.Id, snapshot.Player.Position, -1, snapshot.Player.MaximumHealth, snapshot.Player.IsDead);

        return new GameSessionSnapshot(snapshot.SessionId, snapshot.Width, snapshot.Height, snapshot.Status, corruptedPlayer, snapshot.Enemies);
    }
}