using Module.Verification.Invariant;
using Module.Verification.RuntimeControl;
using Module.Verification.StateSnapshot;
using Module.Verification.SystemFact.Observability;
using TinyArena.Application;
using TinyArena.Domain;
using TinyArena.Infrastructure;

namespace TinyArena.Verification.Tests;

public sealed class TinyArenaRuntimeAdapterTests
{
    // [Fact]
    // public void SubmitPlayerAction_WhenApplicationThrows_ShouldCompleteOperationAsFailed()
    // {
    //     ThrowingGameSessionRepository repository = new ThrowingGameSessionRepository();
    //     FixedRandomSource randomSource = new FixedRandomSource(4);

    //     SystemFactHubBuilder factHubBuilder = new SystemFactHubBuilder();
    //     SystemFactHub factHub = factHubBuilder.Build();

    //     StateSnapshotChannel<GameSessionSnapshot> snapshotChannel = new StateSnapshotChannel<GameSessionSnapshot>(16);
    //     IStateSnapshotPublisher<GameSessionSnapshot> snapshotPublisher = snapshotChannel.PublisherPort;

    //     GameSessionSnapshotCapturer snapshotCapturer = new GameSessionSnapshotCapturer();
    //     GameSessionSnapshotRecorder snapshotRecorder = new GameSessionSnapshotRecorder(snapshotCapturer, snapshotPublisher);
    //     GameSessionCommitter committer = new GameSessionCommitter(repository, snapshotRecorder, factHub);

    //     SubmitPlayerActionUseCase submitPlayerAction = new SubmitPlayerActionUseCase(repository, randomSource, factHub, committer);

    //     OperationRegistry<SubmitPlayerActionResult> operationRegistry = new OperationRegistry<SubmitPlayerActionResult>("tinyarena", 0);

    //     TinyArenaRuntimeAdapter runtimeAdapter = new TinyArenaRuntimeAdapter(submitPlayerAction, operationRegistry);

    //     GameSessionId sessionId = new GameSessionId(1);
    //     PlayerActionCommand.Move command = new PlayerActionCommand.Move(Direction.Left);

    //     OperationHandle handle = runtimeAdapter.SubmitPlayerAction(sessionId, command);

    //     OperationRead<SubmitPlayerActionResult> read = runtimeAdapter.Read(handle);

    //     Assert.Equal(OperationReadState.Found, read.ReadState);
    //     Assert.Equal(OperationState.Failed, read.State);

    //     Assert.NotNull(read.Completion);
    //     Assert.Equal(OperationState.Failed, read.Completion.State);
    //     Assert.Equal("tinyarena.action.execution-failed", read.Completion.Code);
    // }

    // [Fact]
    // public void SubmitSameIdempotentOperationTwice_ShouldExecuteGameplayOnlyOnce()
    // {
    //     InMemoryGameSessionRepository repository = new InMemoryGameSessionRepository();
    //     FixedRandomSource randomSource = new FixedRandomSource(4);

    //     SystemFactHubBuilder factHubBuilder = new SystemFactHubBuilder();
    //     SystemFactHub factHub = factHubBuilder.Build();

    //     StateSnapshotChannel<GameSessionSnapshot> snapshotChannel = new StateSnapshotChannel<GameSessionSnapshot>(16);

    //     GameSessionSnapshotCapturer snapshotCapturer = new GameSessionSnapshotCapturer();
    //     GameSessionSnapshotRecorder snapshotRecorder = new GameSessionSnapshotRecorder(snapshotCapturer, snapshotChannel.PublisherPort);
    //     GameSessionCommitter committer = new GameSessionCommitter(repository, snapshotRecorder, factHub);

    //     StartGameUseCase startGame = new StartGameUseCase(committer);
    //     SubmitPlayerActionUseCase submitPlayerAction = new SubmitPlayerActionUseCase(repository, randomSource, factHub, committer);
    //     GetGameStateUseCase getGameState = new GetGameStateUseCase(repository);

    //     OperationRegistry<SubmitPlayerActionResult> operationRegistry = new OperationRegistry<SubmitPlayerActionResult>("tinyarena", 0);
    //     TinyArenaRuntimeAdapter runtimeAdapter = new TinyArenaRuntimeAdapter(submitPlayerAction, operationRegistry);

    //     GameSessionId sessionId = new GameSessionId(1);

    //     ActorSetup player = new ActorSetup(new ActorId(1), new Position(2, 1), 10, 10);
    //     ActorSetup enemy = new ActorSetup(new ActorId(2), new Position(4, 4), 4, 4);

    //     StartGameCommand startCommand = new StartGameCommand(sessionId, 5, 5, player, new[] { enemy });

    //     startGame.Execute(startCommand);

    //     PlayerActionCommand.Move command = new PlayerActionCommand.Move(Direction.Left);

    //     OperationHandle firstHandle = runtimeAdapter.SubmitPlayerAction(sessionId, command, "request-001", "scenario-001");

    //     OperationHandle secondHandle = runtimeAdapter.SubmitPlayerAction(sessionId, command, "request-001", "scenario-001");

    //     Assert.Equal(firstHandle, secondHandle);

    //     GameStateDto? state = getGameState.Execute(sessionId);

    //     Assert.NotNull(state);
    //     Assert.Equal(new Position(1, 1), state.Player.Position);

    //     OperationRead<SubmitPlayerActionResult> read = runtimeAdapter.Read(firstHandle);

    //     Assert.Equal(OperationReadState.Found, read.ReadState);
    //     Assert.Equal(OperationState.Succeeded, read.State);
    // }

    // [Fact]
    // public void SubmitConflictingIdempotencyRequest_ShouldReturnInvalidAdmissionWithoutExecutingGameplayAgain()
    // {
    //     InMemoryGameSessionRepository repository = new InMemoryGameSessionRepository();
    //     FixedRandomSource randomSource = new FixedRandomSource(4);

    //     SystemFactHubBuilder factHubBuilder = new SystemFactHubBuilder();
    //     SystemFactHub factHub = factHubBuilder.Build();

    //     StateSnapshotChannel<GameSessionSnapshot> snapshotChannel = new StateSnapshotChannel<GameSessionSnapshot>(16);

    //     GameSessionSnapshotCapturer snapshotCapturer = new GameSessionSnapshotCapturer();
    //     GameSessionSnapshotRecorder snapshotRecorder = new GameSessionSnapshotRecorder(snapshotCapturer, snapshotChannel.PublisherPort);
    //     GameSessionCommitter committer = new GameSessionCommitter(repository, snapshotRecorder, factHub);

    //     StartGameUseCase startGame = new StartGameUseCase(committer);
    //     SubmitPlayerActionUseCase submitPlayerAction = new SubmitPlayerActionUseCase(repository, randomSource, factHub, committer);
    //     GetGameStateUseCase getGameState = new GetGameStateUseCase(repository);

    //     OperationRegistry<SubmitPlayerActionResult> operationRegistry = new OperationRegistry<SubmitPlayerActionResult>("tinyarena", 0);
    //     TinyArenaRuntimeAdapter runtimeAdapter = new TinyArenaRuntimeAdapter(submitPlayerAction, operationRegistry);

    //     GameSessionId sessionId = new GameSessionId(1);

    //     ActorSetup player = new ActorSetup(new ActorId(1), new Position(2, 1), 10, 10);
    //     ActorSetup enemy = new ActorSetup(new ActorId(2), new Position(4, 4), 4, 4);

    //     StartGameCommand startCommand = new StartGameCommand(sessionId, 5, 5, player, new[] { enemy });

    //     startGame.Execute(startCommand);

    //     PlayerActionCommand.Move command = new PlayerActionCommand.Move(Direction.Left);

    //     OperationAdmission firstAdmission = runtimeAdapter.SubmitPlayerAction(sessionId, command, "request-001", "scenario-A");

    //     Assert.Equal(OperationAdmissionStatus.Admitted, firstAdmission.Status);

    //     OperationAdmission secondAdmission = runtimeAdapter.SubmitPlayerAction(sessionId, command, "request-001", "scenario-B");

    //     Assert.Equal(OperationAdmissionStatus.Invalid, secondAdmission.Status);
    //     Assert.False(secondAdmission.IsAdmitted);
    //     Assert.Equal("operation.idempotency_conflict", secondAdmission.Code);

    //     GameStateDto? state = getGameState.Execute(sessionId);

    //     Assert.NotNull(state);
    //     Assert.Equal(new Position(1, 1), state.Player.Position);
    // }

    // [Fact]
    // public void RuntimeControl_ShouldStartGameAndSubmitPlayerActionThroughNormalApplicationEntryPoints()
    // {
    //     InMemoryGameSessionRepository repository = new InMemoryGameSessionRepository();
    //     FixedRandomSource randomSource = new FixedRandomSource(4);

    //     SystemFactHubBuilder factHubBuilder = new SystemFactHubBuilder();
    //     SystemFactHub factHub = factHubBuilder.Build();

    //     StateSnapshotChannel<GameSessionSnapshot> snapshotChannel = new StateSnapshotChannel<GameSessionSnapshot>(16);

    //     GameSessionSnapshotCapturer snapshotCapturer = new GameSessionSnapshotCapturer();
    //     GameSessionSnapshotRecorder snapshotRecorder = new GameSessionSnapshotRecorder(snapshotCapturer, snapshotChannel.PublisherPort);
    //     GameSessionCommitter committer = new GameSessionCommitter(repository, snapshotRecorder, factHub);

    //     StartGameUseCase startGame = new StartGameUseCase(committer);
    //     SubmitPlayerActionUseCase submitPlayerAction = new SubmitPlayerActionUseCase(repository, randomSource, factHub, committer);

    //     OperationRegistry<GameSessionId> startGameOperations = new OperationRegistry<GameSessionId>("tinyarena.start-game", 0);
    //     OperationRegistry<SubmitPlayerActionResult> playerActionOperations = new OperationRegistry<SubmitPlayerActionResult>("tinyarena.player-action", 0);

    //     TinyArenaRuntimeAdapter runtimeAdapter = new TinyArenaRuntimeAdapter(startGame, submitPlayerAction, startGameOperations, playerActionOperations);

    //     GameSessionId sessionId = new GameSessionId(1);

    //     ActorSetup player = new ActorSetup(new ActorId(1), new Position(2, 1), 10, 10);
    //     ActorSetup enemy = new ActorSetup(new ActorId(2), new Position(4, 4), 4, 4);
    //     StartGameCommand startCommand = new StartGameCommand(sessionId, 5, 5, player, new[] { enemy });

    //     OperationAdmission startAdmission = runtimeAdapter.StartGame(startCommand, "request-start-001", "scenario-001");

    //     Assert.Equal(OperationAdmissionStatus.Admitted, startAdmission.Status);

    //     OperationRead<GameSessionId> startRead = runtimeAdapter.ReadStartGame(startAdmission.Handle);

    //     Assert.Equal(OperationReadState.Found, startRead.ReadState);
    //     Assert.Equal(OperationState.Succeeded, startRead.State);
    //     Assert.Equal(sessionId, startRead.Completion.Result);

    //     PlayerActionCommand.Move moveCommand = new PlayerActionCommand.Move(Direction.Left);

    //     OperationAdmission moveAdmission = runtimeAdapter.SubmitPlayerAction(sessionId, moveCommand, "request-move-001", "scenario-001");

    //     Assert.Equal(OperationAdmissionStatus.Admitted, moveAdmission.Status);

    //     OperationRead<SubmitPlayerActionResult> moveRead = runtimeAdapter.ReadPlayerAction(moveAdmission.Handle);

    //     Assert.Equal(OperationReadState.Found, moveRead.ReadState);
    //     Assert.Equal(OperationState.Succeeded, moveRead.State);
    //     Assert.Equal(SubmitPlayerActionOutcome.Executed, moveRead.Completion.Result.Outcome);
    // }

    [Fact]
    public void StartGameCompletion_ShouldContainObservationBarrier()
    {
        InMemoryGameSessionRepository repository = new InMemoryGameSessionRepository();
        FixedRandomSource randomSource = new FixedRandomSource(4);

        SystemFactHubBuilder factHubBuilder = new SystemFactHubBuilder();
        SystemFactHub factHub = factHubBuilder.Build();

        StateSnapshotChannel<GameSessionSnapshot> snapshotChannel =
            new StateSnapshotChannel<GameSessionSnapshot>(16);

        GameSessionSnapshotCapturer snapshotCapturer =
            new GameSessionSnapshotCapturer();

        GameSessionSnapshotRecorder snapshotRecorder =
            new GameSessionSnapshotRecorder(
                snapshotCapturer,
                snapshotChannel.PublisherPort);

        GameSessionCommitter committer =
            new GameSessionCommitter(
                repository,
                snapshotRecorder,
                factHub);

        StartGameUseCase startGame =
            new StartGameUseCase(committer);

        SubmitPlayerActionUseCase submitPlayerAction =
            new SubmitPlayerActionUseCase(
                repository,
                randomSource,
                factHub,
                committer);

        OperationRegistry<GameSessionId> startGameOperations =
            new OperationRegistry<GameSessionId>(
                "tinyarena.start-game",
                0);

        OperationRegistry<SubmitPlayerActionResult> playerActionOperations =
            new OperationRegistry<SubmitPlayerActionResult>(
                "tinyarena.player-action",
                0);

        RuntimeOperationExecutor<GameSessionId> startGameExecutor =
            new RuntimeOperationExecutor<GameSessionId>(
                startGameOperations);

        RuntimeOperationExecutor<SubmitPlayerActionResult> playerActionExecutor =
            new RuntimeOperationExecutor<SubmitPlayerActionResult>(
                playerActionOperations);

        TinyArenaRuntimeAdapter runtimeAdapter =
            new TinyArenaRuntimeAdapter(
                startGame,
                submitPlayerAction,
                startGameExecutor,
                playerActionExecutor,
                snapshotChannel.ReaderPort);

        GameSessionId sessionId = new GameSessionId(1);

        ActorSetup player =
            new ActorSetup(
                new ActorId(1),
                new Position(1, 1),
                10,
                10);

        ActorSetup enemy =
            new ActorSetup(
                new ActorId(2),
                new Position(3, 3),
                4,
                4);

        StartGameCommand command =
            new StartGameCommand(
                sessionId,
                5,
                5,
                player,
                new[] { enemy });

        OperationAdmission admission =
            runtimeAdapter.StartGame(
                command,
                "start-001",
                "scenario-001");

        OperationRead<GameSessionId> operationRead =
            runtimeAdapter.ReadStartGame(
                admission.Handle);

        Assert.Equal(
            OperationState.Succeeded,
            operationRead.State);

        Assert.NotNull(
            operationRead.Completion);

        Assert.NotNull(
            operationRead.Completion.ObservationBarrier);

        StateSnapshotRead<GameSessionSnapshot> snapshotRead =
            snapshotChannel.ReaderPort.ReadLatest();

        string expectedBarrier =
            SnapshotObservationBarrier.Encode(
                snapshotRead.Reference);

        Assert.Equal(
            expectedBarrier,
            operationRead.Completion.ObservationBarrier);
    }

    [Fact]
    public void OperationBarrier_ShouldResolveExactCommittedSnapshot()
    {
        InMemoryGameSessionRepository repository =
            new InMemoryGameSessionRepository();

        FixedRandomSource randomSource =
            new FixedRandomSource(4);

        SystemFactHubBuilder factHubBuilder =
            new SystemFactHubBuilder();

        SystemFactHub factHub =
            factHubBuilder.Build();

        StateSnapshotChannel<GameSessionSnapshot> snapshotChannel =
            new StateSnapshotChannel<GameSessionSnapshot>(16);

        IStateSnapshotReader<GameSessionSnapshot> snapshotReader =
            snapshotChannel.ReaderPort;

        GameSessionSnapshotCapturer snapshotCapturer =
            new GameSessionSnapshotCapturer();

        GameSessionSnapshotRecorder snapshotRecorder =
            new GameSessionSnapshotRecorder(
                snapshotCapturer,
                snapshotChannel.PublisherPort);

        GameSessionCommitter committer =
            new GameSessionCommitter(
                repository,
                snapshotRecorder,
                factHub);

        StartGameUseCase startGame =
            new StartGameUseCase(
                committer);

        SubmitPlayerActionUseCase submitPlayerAction =
            new SubmitPlayerActionUseCase(
                repository,
                randomSource,
                factHub,
                committer);

        OperationRegistry<GameSessionId> startGameOperations =
            new OperationRegistry<GameSessionId>(
                "tinyarena.start-game",
                0);

        OperationRegistry<SubmitPlayerActionResult> playerActionOperations =
            new OperationRegistry<SubmitPlayerActionResult>(
                "tinyarena.player-action",
                0);

        RuntimeOperationExecutor<GameSessionId> startGameExecutor =
            new RuntimeOperationExecutor<GameSessionId>(
                startGameOperations);

        RuntimeOperationExecutor<SubmitPlayerActionResult> playerActionExecutor =
            new RuntimeOperationExecutor<SubmitPlayerActionResult>(
                playerActionOperations);

        TinyArenaRuntimeAdapter runtimeAdapter =
            new TinyArenaRuntimeAdapter(
                startGame,
                submitPlayerAction,
                startGameExecutor,
                playerActionExecutor,
                snapshotReader);

        InvariantRegistry<GameSessionSnapshot> invariantRegistry =
            new InvariantRegistry<GameSessionSnapshot>();

        invariantRegistry.Register(
            new HealthRangeInvariant());

        invariantRegistry.Register(
            new ActorPositionInvariant());

        invariantRegistry.Register(
            new ActorOverlapInvariant());

        invariantRegistry.Register(
            new WonStateInvariant());

        invariantRegistry.Register(
            new LostStateInvariant());

        invariantRegistry.Seal();

        GameSessionInvariantEvaluator invariantEvaluator =
            new GameSessionInvariantEvaluator(
                invariantRegistry);

        GameSessionId sessionId =
            new GameSessionId(1);

        ActorSetup player =
            new ActorSetup(
                new ActorId(1),
                new Position(2, 1),
                10,
                10);

        ActorSetup enemy =
            new ActorSetup(
                new ActorId(2),
                new Position(4, 4),
                4,
                4);

        StartGameCommand startCommand =
            new StartGameCommand(
                sessionId,
                5,
                5,
                player,
                new[] { enemy });

        OperationAdmission startAdmission =
            runtimeAdapter.StartGame(
                startCommand,
                "request-start",
                "scenario-001");

        OperationRead<GameSessionId> startOperation =
            runtimeAdapter.ReadStartGame(
                startAdmission.Handle);

        PlayerActionCommand.Move moveCommand =
            new PlayerActionCommand.Move(
                Direction.Left);

        OperationAdmission moveAdmission =
            runtimeAdapter.SubmitPlayerAction(
                sessionId,
                moveCommand,
                "request-move",
                "scenario-001");

        OperationRead<SubmitPlayerActionResult> moveOperation =
            runtimeAdapter.ReadPlayerAction(
                moveAdmission.Handle);

        bool startBarrierDecoded =
            SnapshotObservationBarrier.TryDecode(
                startOperation.Completion.ObservationBarrier,
                out StateSnapshotReference startSnapshotReference);

        bool moveBarrierDecoded =
            SnapshotObservationBarrier.TryDecode(
                moveOperation.Completion.ObservationBarrier,
                out StateSnapshotReference moveSnapshotReference);

        Assert.True(startBarrierDecoded);
        Assert.True(moveBarrierDecoded);

        Assert.NotEqual(
            startSnapshotReference,
            moveSnapshotReference);

        StateSnapshotRead<GameSessionSnapshot> startSnapshotRead =
            snapshotReader.Read(
                startSnapshotReference);

        StateSnapshotRead<GameSessionSnapshot> moveSnapshotRead =
            snapshotReader.Read(
                moveSnapshotReference);

        Assert.Equal(
            StateSnapshotReadState.Available,
            startSnapshotRead.State);

        Assert.Equal(
            StateSnapshotReadState.Available,
            moveSnapshotRead.State);

        Assert.Equal(
            new Position(2, 1),
            startSnapshotRead.Snapshot.Player.Position);

        Assert.Equal(
            new Position(1, 1),
            moveSnapshotRead.Snapshot.Player.Position);

        InvariantEvaluation startEvaluation =
            invariantEvaluator.Evaluate(
                startSnapshotReference,
                startSnapshotRead.Snapshot);

        InvariantEvaluation moveEvaluation =
            invariantEvaluator.Evaluate(
                moveSnapshotReference,
                moveSnapshotRead.Snapshot);

        Assert.True(
            startEvaluation.IsSatisfied);

        Assert.True(
            moveEvaluation.IsSatisfied);
    }

    [Fact]
    public void RuntimeControl_ShouldControlGameThroughProductionEntryPointsAndExposeCommittedObservation()
    {
        InMemoryGameSessionRepository repository = new InMemoryGameSessionRepository();
        FixedRandomSource randomSource = new FixedRandomSource(4);

        SystemFactHubBuilder factHubBuilder = new SystemFactHubBuilder();
        SystemFactHub factHub = factHubBuilder.Build();

        StateSnapshotChannel<GameSessionSnapshot> snapshotChannel = new StateSnapshotChannel<GameSessionSnapshot>(16);
        IStateSnapshotReader<GameSessionSnapshot> snapshotReader = snapshotChannel.ReaderPort;

        GameSessionSnapshotCapturer snapshotCapturer = new GameSessionSnapshotCapturer();
        GameSessionSnapshotRecorder snapshotRecorder = new GameSessionSnapshotRecorder(snapshotCapturer, snapshotChannel.PublisherPort);
        GameSessionCommitter committer = new GameSessionCommitter(repository, snapshotRecorder, factHub);

        StartGameUseCase startGame = new StartGameUseCase(committer);
        SubmitPlayerActionUseCase submitPlayerAction = new SubmitPlayerActionUseCase(repository, randomSource, factHub, committer);

        OperationRegistry<GameSessionId> startGameOperations = new OperationRegistry<GameSessionId>("tinyarena.start-game", 0);
        OperationRegistry<SubmitPlayerActionResult> playerActionOperations = new OperationRegistry<SubmitPlayerActionResult>("tinyarena.player-action", 0);

        RuntimeOperationExecutor<GameSessionId> startGameExecutor = new RuntimeOperationExecutor<GameSessionId>(startGameOperations);
        RuntimeOperationExecutor<SubmitPlayerActionResult> playerActionExecutor = new RuntimeOperationExecutor<SubmitPlayerActionResult>(playerActionOperations);

        TinyArenaRuntimeAdapter runtimeAdapter = new TinyArenaRuntimeAdapter(startGame, submitPlayerAction, startGameExecutor, playerActionExecutor, snapshotReader);

        InvariantRegistry<GameSessionSnapshot> invariantRegistry = new InvariantRegistry<GameSessionSnapshot>();
        invariantRegistry.Register(new HealthRangeInvariant());
        invariantRegistry.Register(new ActorPositionInvariant());
        invariantRegistry.Register(new ActorOverlapInvariant());
        invariantRegistry.Register(new WonStateInvariant());
        invariantRegistry.Register(new LostStateInvariant());
        invariantRegistry.Seal();

        GameSessionInvariantEvaluator invariantEvaluator = new GameSessionInvariantEvaluator(invariantRegistry);

        GameSessionId sessionId = new GameSessionId(1);
        ActorSetup player = new ActorSetup(new ActorId(1), new Position(2, 1), 10, 10);
        ActorSetup enemy = new ActorSetup(new ActorId(2), new Position(4, 4), 4, 4);
        StartGameCommand startCommand = new StartGameCommand(sessionId, 5, 5, player, new[] { enemy });

        OperationAdmission startAdmission = runtimeAdapter.StartGame(startCommand, "request-start", "scenario-001");

        Assert.Equal(OperationAdmissionStatus.Admitted, startAdmission.Status);

        OperationRead<GameSessionId> startOperation = runtimeAdapter.ReadStartGame(startAdmission.Handle);

        Assert.Equal(OperationReadState.Found, startOperation.ReadState);
        Assert.Equal(OperationState.Succeeded, startOperation.State);
        Assert.Equal(sessionId, startOperation.Completion.Result);

        bool startBarrierDecoded = SnapshotObservationBarrier.TryDecode(startOperation.Completion.ObservationBarrier, out StateSnapshotReference startReference);

        Assert.True(startBarrierDecoded);

        StateSnapshotRead<GameSessionSnapshot> startSnapshotRead = snapshotReader.Read(startReference);

        Assert.Equal(StateSnapshotReadState.Available, startSnapshotRead.State);
        Assert.Equal(new Position(2, 1), startSnapshotRead.Snapshot.Player.Position);

        InvariantEvaluation startEvaluation = invariantEvaluator.Evaluate(startReference, startSnapshotRead.Snapshot);

        Assert.True(startEvaluation.IsSatisfied);

        PlayerActionCommand.Move moveCommand = new PlayerActionCommand.Move(Direction.Left);

        OperationAdmission moveAdmission = runtimeAdapter.SubmitPlayerAction(sessionId, moveCommand, "request-move", "scenario-001");

        Assert.Equal(OperationAdmissionStatus.Admitted, moveAdmission.Status);

        OperationRead<SubmitPlayerActionResult> moveOperation = runtimeAdapter.ReadPlayerAction(moveAdmission.Handle);

        Assert.Equal(OperationReadState.Found, moveOperation.ReadState);
        Assert.Equal(OperationState.Succeeded, moveOperation.State);
        Assert.Equal(SubmitPlayerActionOutcome.Executed, moveOperation.Completion.Result.Outcome);

        bool moveBarrierDecoded = SnapshotObservationBarrier.TryDecode(moveOperation.Completion.ObservationBarrier, out StateSnapshotReference moveReference);

        Assert.True(moveBarrierDecoded);
        Assert.NotEqual(startReference, moveReference);

        StateSnapshotRead<GameSessionSnapshot> moveSnapshotRead = snapshotReader.Read(moveReference);

        Assert.Equal(StateSnapshotReadState.Available, moveSnapshotRead.State);
        Assert.Equal(new Position(1, 1), moveSnapshotRead.Snapshot.Player.Position);

        InvariantEvaluation moveEvaluation = invariantEvaluator.Evaluate(moveReference, moveSnapshotRead.Snapshot);

        Assert.True(moveEvaluation.IsSatisfied);

        StateSnapshotRead<GameSessionSnapshot> startSnapshotReadAgain = snapshotReader.Read(startReference);

        Assert.Equal(new Position(2, 1), startSnapshotReadAgain.Snapshot.Player.Position);
    }

}