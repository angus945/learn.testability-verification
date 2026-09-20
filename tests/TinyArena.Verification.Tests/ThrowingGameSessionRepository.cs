using TinyArena.Application;
using TinyArena.Domain;

namespace TinyArena.Verification.Tests;

internal sealed class ThrowingGameSessionRepository : IGameSessionRepository
{
    public void Add(GameSession session)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public GameSession? Get(GameSessionId id)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }

    public void Update(GameSession session)
    {
        throw new InvalidOperationException("Simulated repository failure.");
    }
}