using Module.Verification.Invariant;
using Module.Verification.StateSnapshot;
using Module.Verification.SystemFact.Observability;
using TinyArena.Application;
using TinyArena.Domain;
using TinyArena.Infrastructure;
using TinyArena.Verification;

namespace TinyArena.Verification.Tests;

public sealed class HealthRangeInvariantTests
{
    [Fact]
    public void ValidHealth_ShouldBeSatisfied()
    {
        ActorSnapshot player = new ActorSnapshot(new ActorId(1), new Position(1, 1), 10, 10, false);
        ActorSnapshot enemy = new ActorSnapshot(new ActorId(2), new Position(3, 3), 4, 4, false);
        GameSessionSnapshot snapshot = new GameSessionSnapshot(new GameSessionId(1), 5, 5, GameStatus.Running, player, new[] { enemy });
        HealthRangeInvariant invariant = new HealthRangeInvariant();

        InvariantResult result = invariant.Evaluate(snapshot);

        Assert.True(result.IsSatisfied);
    }

    [Fact]
    public void NegativeHealth_ShouldBeViolated()
    {
        ActorSnapshot player = new ActorSnapshot(new ActorId(1), new Position(1, 1), 10, 10, false);
        ActorSnapshot enemy = new ActorSnapshot(new ActorId(2), new Position(3, 3), -1, 4, false);
        GameSessionSnapshot snapshot = new GameSessionSnapshot(new GameSessionId(1), 5, 5, GameStatus.Running, player, new[] { enemy });
        HealthRangeInvariant invariant = new HealthRangeInvariant();

        InvariantResult result = invariant.Evaluate(snapshot);

        Assert.False(result.IsSatisfied);
        Assert.Equal("tinyarena.health.range", result.Violation.Code);
    }

    [Fact]
    public void ActorOutsideBoard_ShouldBeViolated()
    {
        ActorSnapshot player = new ActorSnapshot(new ActorId(1), new Position(-1, 1), 10, 10, false);
        ActorSnapshot enemy = new ActorSnapshot(new ActorId(2), new Position(3, 3), 4, 4, false);
        GameSessionSnapshot snapshot = new GameSessionSnapshot(new GameSessionId(1), 5, 5, GameStatus.Running, player, new[] { enemy });
        ActorPositionInvariant invariant = new ActorPositionInvariant();

        InvariantResult result = invariant.Evaluate(snapshot);

        Assert.False(result.IsSatisfied);
        Assert.Equal("tinyarena.actor.position", result.Violation.Code);
    }

    [Fact]
    public void Evaluate_ShouldCollectAllInvariantViolations()
    {
        ActorSnapshot player = new ActorSnapshot(new ActorId(1), new Position(-1, 1), 10, 10, false);
        ActorSnapshot enemy = new ActorSnapshot(new ActorId(2), new Position(3, 3), -1, 4, false);
        GameSessionSnapshot snapshot = new GameSessionSnapshot(new GameSessionId(1), 5, 5, GameStatus.Running, player, new[] { enemy });

        InvariantRegistry<GameSessionSnapshot> registry = new InvariantRegistry<GameSessionSnapshot>();
        registry.Register(new HealthRangeInvariant());
        registry.Register(new ActorPositionInvariant());
        registry.Seal();

        IReadOnlyList<InvariantViolation> violations = registry.Evaluate(snapshot);

        Assert.Equal(2, violations.Count);
    }

    [Fact]
    public void LivingActorsAtSamePosition_ShouldBeViolated()
    {
        Position sharedPosition = new Position(2, 2);

        ActorSnapshot player = new ActorSnapshot(new ActorId(1), sharedPosition, 10, 10, false);
        ActorSnapshot enemy = new ActorSnapshot(new ActorId(2), sharedPosition, 4, 4, false);
        GameSessionSnapshot snapshot = new GameSessionSnapshot(new GameSessionId(1), 5, 5, GameStatus.Running, player, new[] { enemy });

        ActorOverlapInvariant invariant = new ActorOverlapInvariant();

        InvariantResult result = invariant.Evaluate(snapshot);

        Assert.False(result.IsSatisfied);
        Assert.Equal("tinyarena.actor.overlap", result.Violation.Code);
    }

    [Fact]
    public void DeadActorAtSamePosition_ShouldBeSatisfied()
    {
        Position sharedPosition = new Position(2, 2);

        ActorSnapshot player = new ActorSnapshot(new ActorId(1), sharedPosition, 10, 10, false);
        ActorSnapshot enemy = new ActorSnapshot(new ActorId(2), sharedPosition, 0, 4, true);
        GameSessionSnapshot snapshot = new GameSessionSnapshot(new GameSessionId(1), 5, 5, GameStatus.Running, player, new[] { enemy });

        ActorOverlapInvariant invariant = new ActorOverlapInvariant();

        InvariantResult result = invariant.Evaluate(snapshot);

        Assert.True(result.IsSatisfied);
    }

    [Fact]
    public void WonWithLivingEnemy_ShouldBeViolated()
    {
        ActorSnapshot player = new ActorSnapshot(new ActorId(1), new Position(1, 1), 10, 10, false);
        ActorSnapshot enemy = new ActorSnapshot(new ActorId(2), new Position(3, 3), 4, 4, false);

        GameSessionSnapshot snapshot = new GameSessionSnapshot(new GameSessionId(1), 5, 5, GameStatus.Won, player, new[] { enemy });

        WonStateInvariant invariant = new WonStateInvariant();

        InvariantResult result = invariant.Evaluate(snapshot);

        Assert.False(result.IsSatisfied);
        Assert.Equal("tinyarena.status.won", result.Violation.Code);
    }

    [Fact]
    public void PlayerAndAllEnemiesDead_WithLostStatus_ShouldBeSatisfied()
    {
        ActorSnapshot player = new ActorSnapshot(new ActorId(1), new Position(1, 1), 0, 10, true);
        ActorSnapshot enemy = new ActorSnapshot(new ActorId(2), new Position(3, 3), 0, 4, true);

        GameSessionSnapshot snapshot = new GameSessionSnapshot(new GameSessionId(1), 5, 5, GameStatus.Lost, player, new[] { enemy });

        WonStateInvariant invariant = new WonStateInvariant();

        InvariantResult result = invariant.Evaluate(snapshot);

        Assert.True(result.IsSatisfied);
    }

    [Fact]
    public void LostWithLivingPlayer_ShouldBeViolated()
    {
        ActorSnapshot player = new ActorSnapshot(new ActorId(1), new Position(1, 1), 10, 10, false);
        ActorSnapshot enemy = new ActorSnapshot(new ActorId(2), new Position(3, 3), 4, 4, false);

        GameSessionSnapshot snapshot = new GameSessionSnapshot(new GameSessionId(1), 5, 5, GameStatus.Lost, player, new[] { enemy });

        LostStateInvariant invariant = new LostStateInvariant();

        InvariantResult result = invariant.Evaluate(snapshot);

        Assert.False(result.IsSatisfied);
    }

    [Fact]
    public void CommittedGameplaySnapshot_ShouldSatisfyAllInvariants()
    {
        InMemoryGameSessionRepository repository = new InMemoryGameSessionRepository();
        FixedRandomSource randomSource = new FixedRandomSource(4);

        SystemFactHubBuilder factHubBuilder = new SystemFactHubBuilder();
        SystemFactHub factHub = factHubBuilder.Build();

        StateSnapshotChannel<GameSessionSnapshot> snapshotChannel = new StateSnapshotChannel<GameSessionSnapshot>(16);
        IStateSnapshotPublisher<GameSessionSnapshot> snapshotPublisher = snapshotChannel.PublisherPort;
        IStateSnapshotReader<GameSessionSnapshot> snapshotReader = snapshotChannel.ReaderPort;

        GameSessionSnapshotCapturer snapshotCapturer = new GameSessionSnapshotCapturer();
        GameSessionSnapshotRecorder snapshotRecorder = new GameSessionSnapshotRecorder(snapshotCapturer, snapshotPublisher);
        GameSessionCommitter committer = new GameSessionCommitter(repository, snapshotRecorder, factHub);

        StartGameUseCase startGame = new StartGameUseCase(committer);
        SubmitPlayerActionUseCase submitPlayerAction = new SubmitPlayerActionUseCase(repository, randomSource, factHub, committer);

        InvariantRegistry<GameSessionSnapshot> registry = new InvariantRegistry<GameSessionSnapshot>();
        registry.Register(new HealthRangeInvariant());
        registry.Register(new ActorPositionInvariant());
        registry.Register(new ActorOverlapInvariant());
        registry.Register(new WonStateInvariant());
        registry.Register(new LostStateInvariant());
        registry.Seal();

        GameSessionInvariantEvaluator evaluator = new GameSessionInvariantEvaluator(registry);

        GameSessionId sessionId = new GameSessionId(1);

        ActorSetup player = new ActorSetup(new ActorId(1), new Position(1, 1), 10, 10);
        ActorSetup enemy = new ActorSetup(new ActorId(2), new Position(3, 3), 4, 4);

        startGame.Execute(new StartGameCommand(sessionId, 5, 5, player, new[] { enemy }));

        SubmitPlayerActionResult operationResult = submitPlayerAction.Execute(sessionId, new PlayerActionCommand.Move(Direction.Left));

        StateSnapshotRead<GameSessionSnapshot> snapshotRead = snapshotReader.ReadLatest();

        Assert.Equal(SubmitPlayerActionOutcome.Executed, operationResult.Outcome);
        Assert.Equal(StateSnapshotReadState.Available, snapshotRead.State);

        InvariantEvaluation evaluation = evaluator.Evaluate(snapshotRead.Reference, snapshotRead.Snapshot);

        Assert.True(evaluation.IsSatisfied);
        Assert.Empty(evaluation.Violations);
    }
}
