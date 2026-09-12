using TinyArena.Domain;

namespace TinyArena.Application;

public sealed class StartGameUseCase
{
    private readonly IGameSessionRepository _repository;

    public StartGameUseCase(IGameSessionRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public GameSessionId Execute(StartGameCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(command.Player);
        ArgumentNullException.ThrowIfNull(command.Enemies);

        Actor player = CreateActor(command.Player);
        Actor[] enemies = command.Enemies.Select(CreateActor).ToArray();

        GameSession session = new GameSession(command.SessionId, command.Width, command.Height, player, enemies);

        _repository.Add(session);

        return session.Id;
    }

    private static Actor CreateActor(ActorSetup setup)
    {
        Health health = new Health(setup.CurrentHealth, setup.MaximumHealth);
        return new Actor(setup.Id, setup.Position, health);
    }
}