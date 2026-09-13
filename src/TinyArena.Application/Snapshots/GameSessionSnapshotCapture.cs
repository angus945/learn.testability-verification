using TinyArena.Domain;

namespace TinyArena.Application;

public sealed class GameSessionSnapshotCapturer : IGameSessionSnapshotCapturer
{
    public GameSessionSnapshot Capture(GameSession session)
    {
        ArgumentNullException.ThrowIfNull(session);

        ActorSnapshot player = CreateActorSnapshot(session.Player);
        ActorSnapshot[] enemies = session.Enemies.Select(CreateActorSnapshot).ToArray();

        return new GameSessionSnapshot(session.Id, session.Width, session.Height, session.Status, player, enemies);
    }

    private static ActorSnapshot CreateActorSnapshot(Actor actor)
    {
        return new ActorSnapshot(actor.Id, actor.Position, actor.Health.Current, actor.Health.Maximum, actor.IsDead);
    }
}