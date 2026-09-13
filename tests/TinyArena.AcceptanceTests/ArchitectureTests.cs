using Module.Verification.StateSnapshot;
using Module.Verification.SystemFact.Observability;
using TinyArena.Application;
using TinyArena.Domain;
using TinyArena.Infrastructure;

namespace TinyArena.AcceptanceTests;

public class ArchitectureTests
{

    [Fact]
    public void SnapshotCaptureFailure_ShouldNotChangeCommittedGameplayOutcome()
    {
        InMemoryGameSessionRepository repository = new InMemoryGameSessionRepository();
        FixedRandomSource randomSource = new FixedRandomSource(4);

        SystemFactHubBuilder factHubBuilder = new SystemFactHubBuilder();
        SystemFactHub factHub = factHubBuilder.Build();

        StateSnapshotChannel<GameSessionSnapshot> snapshotChannel = new StateSnapshotChannel<GameSessionSnapshot>(16);
        IStateSnapshotPublisher<GameSessionSnapshot> snapshotPublisher = snapshotChannel.PublisherPort;
        IStateSnapshotReader<GameSessionSnapshot> snapshotReader = snapshotChannel.ReaderPort;

        GameSessionSnapshotCapturer normalCapturer = new GameSessionSnapshotCapturer();
        GameSessionSnapshotRecorder normalRecorder = new GameSessionSnapshotRecorder(normalCapturer, snapshotPublisher);
        GameSessionCommitter normalCommitter = new GameSessionCommitter(repository, normalRecorder, factHub);

        StartGameUseCase startGame = new StartGameUseCase(normalCommitter);

        GameSessionId sessionId = new GameSessionId(1);
        ActorSetup player = new ActorSetup(new ActorId(1), new Position(1, 1), 10, 10);
        ActorSetup enemy = new ActorSetup(new ActorId(2), new Position(3, 3), 4, 4);
        StartGameCommand startCommand = new StartGameCommand(sessionId, 5, 5, player, new[] { enemy });

        startGame.Execute(startCommand);

        ThrowingGameSessionSnapshotCapturer failingCapturer = new ThrowingGameSessionSnapshotCapturer();
        GameSessionSnapshotRecorder failingRecorder = new GameSessionSnapshotRecorder(failingCapturer, snapshotPublisher);
        GameSessionCommitter failingCommitter = new GameSessionCommitter(repository, failingRecorder, factHub);

        SubmitPlayerActionUseCase submitAction = new SubmitPlayerActionUseCase(repository, randomSource, factHub, failingCommitter);
        GetGameStateUseCase getGameState = new GetGameStateUseCase(repository);

        PlayerActionCommand.Move action = new PlayerActionCommand.Move(Direction.Left);

        SubmitPlayerActionResult result = submitAction.Execute(sessionId, action);

        Assert.Equal(SubmitPlayerActionOutcome.Executed, result.Outcome);

        GameStateDto? state = getGameState.Execute(sessionId);

        Assert.NotNull(state);
        Assert.Equal(new Position(0, 1), state.Player.Position);

        StateSnapshotRead<GameSessionSnapshot> snapshotRead = snapshotReader.ReadLatest();

        Assert.Equal(StateSnapshotReadState.CaptureFailed, snapshotRead.State);
        Assert.False(snapshotRead.HasSnapshot);
        Assert.NotNull(snapshotRead.Failure);
        Assert.Equal("tinyarena.game-session.capture-failed", snapshotRead.Failure.Code);
    }
}