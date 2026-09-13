// using Module.Verification.StateSnapshot;
// using Module.Verification.SystemFact.Observability;
// using TinyArena.Application;
// using TinyArena.Domain;
// using TinyArena.Infrastructure;

// namespace TinyArena.AcceptanceTests;

// public sealed class StateSnapshotScenarioTests
// {
//     [Fact]
//     public void Snapshot_ShouldOnlyAdvanceOnCommittedStateAndKeepHistoricalStateImmutable()
//     {
//         InMemoryGameSessionRepository repository = new InMemoryGameSessionRepository();
//         FixedRandomSource randomSource = new FixedRandomSource(4);

//         SystemFactHubBuilder factHubBuilder = new SystemFactHubBuilder();
//         SystemFactHub factHub = factHubBuilder.Build();

//         StateSnapshotChannel<GameSessionSnapshot> snapshotChannel = new StateSnapshotChannel<GameSessionSnapshot>(16);
//         IStateSnapshotPublisher<GameSessionSnapshot> snapshotPublisher = snapshotChannel.PublisherPort;
//         IStateSnapshotReader<GameSessionSnapshot> snapshotReader = snapshotChannel.ReaderPort;

//         StartGameUseCase startGame = new StartGameUseCase(repository, snapshotPublisher);
//         SubmitPlayerActionUseCase submitAction = new SubmitPlayerActionUseCase(repository, randomSource, factHub, snapshotPublisher);

//         GameSessionId sessionId = new GameSessionId(1);
//         ActorSetup player = new ActorSetup(new ActorId(1), new Position(1, 1), 10, 10);
//         ActorSetup enemy = new ActorSetup(new ActorId(2), new Position(3, 3), 4, 4);
//         StartGameCommand startCommand = new StartGameCommand(sessionId, 5, 5, player, new[] { enemy });

//         startGame.Execute(startCommand);

//         StateSnapshotRead<GameSessionSnapshot> initialRead = snapshotReader.ReadLatest();

//         Assert.Equal(StateSnapshotReadState.Available, initialRead.State);
//         Assert.True(initialRead.HasSnapshot);

//         StateSnapshotReference initialReference = initialRead.Reference;

//         Assert.Equal(new Position(1, 1), initialRead.Snapshot.Player.Position);
//         Assert.Equal(new Position(3, 3), Assert.Single(initialRead.Snapshot.Enemies).Position);

//         StateSnapshotRead<GameSessionSnapshot> repeatedRead = snapshotReader.ReadLatest();

//         Assert.Equal(initialReference, repeatedRead.Reference);

//         PlayerActionCommand.Attack rejectedAction = new PlayerActionCommand.Attack(Direction.Up);
//         SubmitPlayerActionResult rejectedResult = submitAction.Execute(sessionId, rejectedAction);

//         Assert.Equal(SubmitPlayerActionOutcome.Rejected, rejectedResult.Outcome);
//         Assert.Equal(PlayerActionRejectionReason.NoTarget, rejectedResult.RejectionReason);

//         StateSnapshotRead<GameSessionSnapshot> afterRejectedRead = snapshotReader.ReadLatest();

//         Assert.Equal(initialReference, afterRejectedRead.Reference);

//         PlayerActionCommand.Move committedAction = new PlayerActionCommand.Move(Direction.Left);
//         SubmitPlayerActionResult committedResult = submitAction.Execute(sessionId, committedAction);

//         Assert.Equal(SubmitPlayerActionOutcome.Executed, committedResult.Outcome);

//         StateSnapshotRead<GameSessionSnapshot> latestRead = snapshotReader.ReadLatest();

//         Assert.Equal(StateSnapshotReadState.Available, latestRead.State);
//         Assert.NotEqual(initialReference, latestRead.Reference);
//         Assert.Equal(new Position(0, 1), latestRead.Snapshot.Player.Position);

//         StateSnapshotRead<GameSessionSnapshot> historicalRead = snapshotReader.Read(initialReference);

//         Assert.Equal(StateSnapshotReadState.Available, historicalRead.State);
//         Assert.Equal(new Position(1, 1), historicalRead.Snapshot.Player.Position);
//         Assert.Equal(new Position(3, 3), Assert.Single(historicalRead.Snapshot.Enemies).Position);
//     }
// }