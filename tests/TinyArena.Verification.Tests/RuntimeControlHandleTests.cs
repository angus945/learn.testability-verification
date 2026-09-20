// using Module.Verification.RuntimeControl;
// using Module.Verification.StateSnapshot;
// using Module.Verification.SystemFact.Observability;
// using TinyArena.Application;
// using TinyArena.Domain;
// using TinyArena.Infrastructure;

// namespace TinyArena.Verification.Tests;

// public sealed class RuntimeControlHandleTests
// {
//     [Fact]
//     public void SubmitPlayerAction_ShouldReturnHandleAndAllowResultLookup()
//     {
//         InMemoryGameSessionRepository repository = new InMemoryGameSessionRepository();
//         FixedRandomSource randomSource = new FixedRandomSource(4);

//         SystemFactHubBuilder factHubBuilder = new SystemFactHubBuilder();
//         SystemFactHub factHub = factHubBuilder.Build();

//         StateSnapshotChannel<GameSessionSnapshot> snapshotChannel =
//             new StateSnapshotChannel<GameSessionSnapshot>(16);

//         GameSessionSnapshotCapturer snapshotCapturer =
//             new GameSessionSnapshotCapturer();

//         GameSessionSnapshotRecorder snapshotRecorder =
//             new GameSessionSnapshotRecorder(
//                 snapshotCapturer,
//                 snapshotChannel.PublisherPort);

//         GameSessionCommitter committer =
//             new GameSessionCommitter(
//                 repository,
//                 snapshotRecorder,
//                 factHub);

//         StartGameUseCase startGame =
//             new StartGameUseCase(committer);

//         SubmitPlayerActionUseCase submitPlayerAction =
//             new SubmitPlayerActionUseCase(
//                 repository,
//                 randomSource,
//                 factHub,
//                 committer);

//         OperationRegistry<SubmitPlayerActionResult> operationRegistry =
//             new OperationRegistry<SubmitPlayerActionResult>(
//                 "tinyarena",
//                 0);

//         TinyArenaRuntimeAdapter runtimeAdapter =
//             new TinyArenaRuntimeAdapter(
//                 submitPlayerAction,
//                 operationRegistry);

//         GameSessionId sessionId = new GameSessionId(1);

//         ActorSetup player =
//             new ActorSetup(
//                 new ActorId(1),
//                 new Position(1, 1),
//                 10,
//                 10);

//         ActorSetup enemy =
//             new ActorSetup(
//                 new ActorId(2),
//                 new Position(3, 3),
//                 4,
//                 4);

//         StartGameCommand startCommand =
//             new StartGameCommand(
//                 sessionId,
//                 5,
//                 5,
//                 player,
//                 new[] { enemy });

//         startGame.Execute(startCommand);

//         PlayerActionCommand.Move command =
//             new PlayerActionCommand.Move(Direction.Left);

//         OperationHandle handle =
//             runtimeAdapter.SubmitPlayerAction(
//                 sessionId,
//                 command);

//         OperationRead<SubmitPlayerActionResult> read =
//             runtimeAdapter.Read(handle);

//         Assert.Equal(OperationReadState.Found, read.ReadState);
//         Assert.Equal(OperationState.Succeeded, read.State);

//         Assert.NotNull(read.Completion);
//         Assert.Equal("tinyarena.action.executed", read.Completion.Code);

//         SubmitPlayerActionResult actionResult =
//             read.Completion.Result;

//         Assert.Equal(
//             SubmitPlayerActionOutcome.Executed,
//             actionResult.Outcome);
//     }
// }