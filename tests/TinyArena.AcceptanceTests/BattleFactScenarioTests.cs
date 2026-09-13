using Module.Verification.SystemFact;
using Module.Verification.SystemFact.Observability;
using TinyArena.Application;
using TinyArena.Domain;
using TinyArena.Infrastructure;

namespace TinyArena.AcceptanceTests;

public sealed class BattleFactScenarioTests
{
    [Fact]
    public void AttackLastEnemy_ShouldPublishExpectedFactsAndFinalState()
    {
        InMemoryGameSessionRepository repository = new InMemoryGameSessionRepository();
        FixedRandomSource randomSource = new FixedRandomSource(4);
        RecordingSystemFactObserver recordingObserver = new RecordingSystemFactObserver();

        SystemFactHubBuilder factHubBuilder = new SystemFactHubBuilder();
        factHubBuilder.Register<ISystemFact>(recordingObserver);
        SystemFactHub factHub = factHubBuilder.Build();

        StartGameUseCase startGame = new StartGameUseCase(repository);
        SubmitPlayerActionUseCase submitAction = new SubmitPlayerActionUseCase(repository, randomSource, factHub);
        GetGameStateUseCase getGameState = new GetGameStateUseCase(repository);

        GameSessionId sessionId = new GameSessionId(1);
        ActorSetup player = new ActorSetup(new ActorId(1), new Position(1, 1), 10, 10);
        ActorSetup enemy = new ActorSetup(new ActorId(2), new Position(2, 1), 4, 4);
        ActorSetup[] enemies = { enemy };

        StartGameCommand startCommand = new StartGameCommand(sessionId, 5, 5, player, enemies);
        startGame.Execute(startCommand);

        PlayerActionCommand.Attack action = new PlayerActionCommand.Attack(Direction.Right);
        SubmitPlayerActionResult result = submitAction.Execute(sessionId, action);

        GameStateDto? state = getGameState.Execute(sessionId);

        Assert.Equal(SubmitPlayerActionOutcome.Executed, result.Outcome);
        Assert.NotNull(state);
        Assert.Equal(GameStatus.Won, state.Status);

        ActorStateDto enemyState = Assert.Single(state.Enemies);
        Assert.True(enemyState.IsDead);
        Assert.Equal(0, enemyState.CurrentHealth);

        Assert.Equal(4, recordingObserver.Facts.Count);

        DamageApplied damageApplied = Assert.IsType<DamageApplied>(recordingObserver.Facts[0].Fact);
        ActorDefeated actorDefeated = Assert.IsType<ActorDefeated>(recordingObserver.Facts[1].Fact);
        BattleEnded battleEnded = Assert.IsType<BattleEnded>(recordingObserver.Facts[2].Fact);
        PlayerActionCommitted actionCommitted = Assert.IsType<PlayerActionCommitted>(recordingObserver.Facts[3].Fact);

        Assert.Equal(new ActorId(1), damageApplied.SourceActorId);
        Assert.Equal(new ActorId(2), damageApplied.TargetActorId);
        Assert.Equal(4, damageApplied.Amount);
        Assert.Equal(4, damageApplied.PreviousHealth);
        Assert.Equal(0, damageApplied.CurrentHealth);

        Assert.Equal(new ActorId(2), actorDefeated.DefeatedActorId);
        Assert.Equal(new ActorId(1), actorDefeated.SourceActorId);

        Assert.Equal(GameStatus.Won, battleEnded.FinalStatus);

        Assert.Equal(sessionId, actionCommitted.SessionId);
        Assert.Equal(action, actionCommitted.Action);
    }

    [Fact]
    public void AttackEmptyCell_ShouldRejectWithoutChangingState()
    {
        InMemoryGameSessionRepository repository = new InMemoryGameSessionRepository();
        FixedRandomSource randomSource = new FixedRandomSource(4);
        RecordingSystemFactObserver recordingObserver = new RecordingSystemFactObserver();

        SystemFactHubBuilder factHubBuilder = new SystemFactHubBuilder();
        factHubBuilder.Register<ISystemFact>(recordingObserver);
        SystemFactHub factHub = factHubBuilder.Build();

        StartGameUseCase startGame = new StartGameUseCase(repository);
        SubmitPlayerActionUseCase submitAction = new SubmitPlayerActionUseCase(repository, randomSource, factHub);
        GetGameStateUseCase getGameState = new GetGameStateUseCase(repository);

        GameSessionId sessionId = new GameSessionId(1);
        ActorSetup player = new ActorSetup(new ActorId(1), new Position(1, 1), 10, 10);
        ActorSetup enemy = new ActorSetup(new ActorId(2), new Position(3, 3), 4, 4);
        StartGameCommand startCommand = new StartGameCommand(sessionId, 5, 5, player, new[] { enemy });

        startGame.Execute(startCommand);

        PlayerActionCommand.Attack action = new PlayerActionCommand.Attack(Direction.Right);
        SubmitPlayerActionResult result = submitAction.Execute(sessionId, action);

        GameStateDto? state = getGameState.Execute(sessionId);

        Assert.Equal(SubmitPlayerActionOutcome.Rejected, result.Outcome);
        Assert.Equal(PlayerActionRejectionReason.NoTarget, result.RejectionReason);

        Assert.NotNull(state);
        Assert.Equal(new Position(1, 1), state.Player.Position);
        Assert.Equal(10, state.Player.CurrentHealth);

        ActorStateDto enemyState = Assert.Single(state.Enemies);
        Assert.Equal(4, enemyState.CurrentHealth);

        PlayerActionRejected rejected = Assert.IsType<PlayerActionRejected>(Assert.Single(recordingObserver.Facts).Fact);

        Assert.Equal(PlayerActionRejectionReason.NoTarget, rejected.Reason);
    }
}