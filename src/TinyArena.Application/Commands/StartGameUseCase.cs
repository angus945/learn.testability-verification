using Module.Verification.StateSnapshot;
using TinyArena.Domain;

namespace TinyArena.Application;

public sealed class StartGameUseCase
{
    private readonly GameSessionCommitter _committer;

    public StartGameUseCase(GameSessionCommitter committer)
    {
        _committer = committer ?? throw new ArgumentNullException(nameof(committer));
    }

    public GameSessionId Execute(StartGameCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(command.Player);
        ArgumentNullException.ThrowIfNull(command.Enemies);

        Actor player = CreateActor(command.Player);
        Actor[] enemies = command.Enemies.Select(CreateActor).ToArray();

        GameSession session = new GameSession(command.SessionId, command.Width, command.Height, player, enemies);

        _committer.CommitNew(session);

        return session.Id;
    }

    private static Actor CreateActor(ActorSetup setup)
    {
        Health health = new Health(setup.CurrentHealth, setup.MaximumHealth);
        return new Actor(setup.Id, setup.Position, health);
    }
}