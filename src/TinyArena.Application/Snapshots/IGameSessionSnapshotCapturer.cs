using TinyArena.Domain;

namespace TinyArena.Application;

public interface IGameSessionSnapshotCapturer
{
    GameSessionSnapshot Capture(GameSession session);
}
