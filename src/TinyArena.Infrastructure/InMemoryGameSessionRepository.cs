using TinyArena.Application;
using TinyArena.Domain;

namespace TinyArena.Infrastructure;

public sealed class InMemoryGameSessionRepository : IGameSessionRepository
{
    private readonly Dictionary<GameSessionId, GameSession> _sessions = new Dictionary<GameSessionId, GameSession>();

    public void Add(GameSession session)
    {
        ArgumentNullException.ThrowIfNull(session);

        if (!_sessions.TryAdd(session.Id, session))
        {
            throw new InvalidOperationException($"Game session {session.Id.Value} already exists.");
        }
    }

    public GameSession? Get(GameSessionId id)
    {
        _sessions.TryGetValue(id, out GameSession? session);

        return session;
    }

    public void Update(GameSession session)
    {
        ArgumentNullException.ThrowIfNull(session);

        if (!_sessions.ContainsKey(session.Id))
        {
            throw new InvalidOperationException($"Game session {session.Id.Value} does not exist.");
        }

        _sessions[session.Id] = session;
    }
}