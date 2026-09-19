using Module.Verification.SystemFact;
using Module.Verification.SystemFact.Observability;
using Module.Verification.StateSnapshot;
using TinyArena.AcceptanceTests;
using TinyArena.Application;
using TinyArena.Console;
using TinyArena.Domain;
using TinyArena.Infrastructure;
using Module.Verification.Invariant;
using TinyArena.Verification;

InMemoryGameSessionRepository repository = new InMemoryGameSessionRepository();
SystemRandomSource randomSource = new SystemRandomSource();

PlayerActionConsoleObserver playerActionObserver = new PlayerActionConsoleObserver();

SystemFactHubBuilder factHubBuilder = new SystemFactHubBuilder();
DamageConsoleObserver damageObserver = new DamageConsoleObserver();
PlayerActionRejectedConsoleObserver rejectedObserver = new PlayerActionRejectedConsoleObserver();
ActorDefeatedConsoleObserver actorDefeatedObserver = new ActorDefeatedConsoleObserver();
BattleEndedConsoleObserver battleEndedObserver = new BattleEndedConsoleObserver();
RecordingSystemFactObserver recordingObserver = new RecordingSystemFactObserver();
factHubBuilder.Register<PlayerActionCommitted>(playerActionObserver);
factHubBuilder.Register<DamageApplied>(damageObserver);
factHubBuilder.Register<PlayerActionRejected>(rejectedObserver);
factHubBuilder.Register<ActorDefeated>(actorDefeatedObserver);
factHubBuilder.Register<BattleEnded>(battleEndedObserver);
factHubBuilder.Register<ISystemFact>(recordingObserver);
SystemFactHub factHub = factHubBuilder.Build();

InvariantRegistry<GameSessionSnapshot> registry = new InvariantRegistry<GameSessionSnapshot>();
registry.Register(new HealthRangeInvariant());
registry.Register(new ActorPositionInvariant());
registry.Register(new ActorOverlapInvariant());
registry.Register(new WonStateInvariant());
registry.Register(new LostStateInvariant());
registry.Seal();

GameSessionInvariantEvaluator invariantEvaluator = new GameSessionInvariantEvaluator(registry);

StateSnapshotChannel<GameSessionSnapshot> snapshotChannel = new StateSnapshotChannel<GameSessionSnapshot>(16);
IStateSnapshotPublisher<GameSessionSnapshot> snapshotPublisher = snapshotChannel.PublisherPort;
IStateSnapshotReader<GameSessionSnapshot> snapshotReader = snapshotChannel.ReaderPort;

GameSessionSnapshotCapturer snapshotCapturer = new GameSessionSnapshotCapturer();
GameSessionSnapshotRecorder snapshotRecorder = new GameSessionSnapshotRecorder(snapshotCapturer, snapshotPublisher);
GameSessionCommitter committer = new GameSessionCommitter(repository, snapshotRecorder, factHub); StartGameUseCase startGame = new StartGameUseCase(committer);
SubmitPlayerActionUseCase submitPlayerAction = new SubmitPlayerActionUseCase(repository, randomSource, factHub, committer);
GetGameStateUseCase getGameState = new GetGameStateUseCase(repository);
GameSessionId sessionId = new GameSessionId(1);

ActorSetup player = new ActorSetup(new ActorId(1), new Position(2, 2), 10, 10);
ActorSetup firstEnemy = new ActorSetup(new ActorId(2), new Position(0, 2), 6, 6);
ActorSetup secondEnemy = new ActorSetup(new ActorId(3), new Position(4, 2), 6, 6);
ActorSetup[] enemies = { firstEnemy, secondEnemy };

StartGameCommand startCommand = new StartGameCommand(sessionId, 5, 5, player, enemies);

startGame.Execute(startCommand);
StateSnapshotRead<GameSessionSnapshot> firstRead = snapshotReader.ReadLatest();
Console.WriteLine($"first snapshot capture ID: {firstRead.Reference.CaptureId}");
// StateSnapshotRead<GameSessionSnapshot> secondRead = snapshotReader.ReadLatest();
// Console.WriteLine($"second snapshot capture ID: {secondRead.Reference.CaptureId}");
if (firstRead.State == StateSnapshotReadState.Available)
{

    InvariantEvaluation evaluation = invariantEvaluator.Evaluate(firstRead.Reference, firstRead.Snapshot);
    Console.WriteLine($"Invariant evaluation for first snapshot: {(evaluation.IsSatisfied ? "Satisfied" : "Violated")}");
}

submitPlayerAction.Execute(sessionId, new PlayerActionCommand.Attack(Direction.Up));
StateSnapshotRead<GameSessionSnapshot> afterRejected = snapshotReader.ReadLatest();
Console.WriteLine($"after rejected snapshot capture ID: {afterRejected.Reference.CaptureId}");

submitPlayerAction.Execute(sessionId, new PlayerActionCommand.Move(Direction.Right));
StateSnapshotRead<GameSessionSnapshot> afterMove = snapshotReader.ReadLatest();
Console.WriteLine($"after move snapshot capture ID: {afterMove.Reference.CaptureId}");

GameStateDto? state = getGameState.Execute(sessionId);

if (state is not null)
{
    Console.WriteLine($"Status: {state.Status}");
    Console.WriteLine($"Player: ({state.Player.Position.X}, {state.Player.Position.Y}) HP {state.Player.CurrentHealth}/{state.Player.MaximumHealth}");
}