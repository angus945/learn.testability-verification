using TinyArena.Application;
using TinyArena.Domain;

namespace TinyArena.AcceptanceTests;

internal sealed class ThrowingGameSessionSnapshotCapturer : IGameSessionSnapshotCapturer
{
    public GameSessionSnapshot Capture(GameSession session)
    {
        throw new InvalidOperationException("Simulated snapshot capture failure.");
    }
}