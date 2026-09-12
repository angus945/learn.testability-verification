using TinyArena.Domain;

namespace TinyArena.Application;

public sealed class GetGameStateUseCase
{
    private readonly IGameSessionRepository _repository;

    public GetGameStateUseCase(IGameSessionRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public GameStateDto? Execute(GameSessionId sessionId)
    {
        GameSession? session = _repository.Get(sessionId);

        if (session is null)
        {
            return null;
        }

        ActorStateDto player = CreateActorState(session.Player);

        ActorStateDto[] enemies = session.Enemies
            .Select(CreateActorState)
            .ToArray();

        return new GameStateDto(
            session.Id,
            session.Width,
            session.Height,
            session.Status,
            player,
            enemies);
    }

    private static ActorStateDto CreateActorState(Actor actor)
    {
        return new ActorStateDto(
            actor.Id,
            actor.Position,
            actor.Health.Current,
            actor.Health.Maximum,
            actor.IsDead);
    }
}