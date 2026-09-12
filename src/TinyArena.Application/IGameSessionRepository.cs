using TinyArena.Domain;

namespace TinyArena.Application;

public interface IGameSessionRepository
{
    void Add(GameSession session);

    GameSession? Get(GameSessionId id);

    void Update(GameSession session);
}