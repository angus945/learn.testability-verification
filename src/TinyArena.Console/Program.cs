using Module.Verification.SystemFact;
using Module.Verification.SystemFact.Observability;
using TinyArena.AcceptanceTests;
using TinyArena.Application;
using TinyArena.Console;
using TinyArena.Domain;
using TinyArena.Infrastructure;

InMemoryGameSessionRepository repository = new InMemoryGameSessionRepository();
SystemRandomSource randomSource = new SystemRandomSource();

PlayerActionConsoleObserver playerActionObserver = new PlayerActionConsoleObserver();

SystemFactHubBuilder factHubBuilder = new SystemFactHubBuilder();
DamageConsoleObserver damageObserver = new DamageConsoleObserver();
PlayerActionRejectedConsoleObserver rejectedObserver = new PlayerActionRejectedConsoleObserver();
ActorDefeatedConsoleObserver actorDefeatedObserver = new ActorDefeatedConsoleObserver();
BattleEndedConsoleObserver battleEndedObserver = new BattleEndedConsoleObserver();
factHubBuilder.Register<PlayerActionCommitted>(playerActionObserver);
factHubBuilder.Register<DamageApplied>(damageObserver);
factHubBuilder.Register<PlayerActionRejected>(rejectedObserver);
factHubBuilder.Register<ActorDefeated>(actorDefeatedObserver);
factHubBuilder.Register<BattleEnded>(battleEndedObserver);

RecordingSystemFactObserver recordingObserver = new RecordingSystemFactObserver();
factHubBuilder.Register<ISystemFact>(recordingObserver);

SystemFactHub factHub = factHubBuilder.Build();

StartGameUseCase startGame = new StartGameUseCase(repository);
SubmitPlayerActionUseCase submitPlayerAction = new SubmitPlayerActionUseCase(repository, randomSource, factHub);
GetGameStateUseCase getGameState = new GetGameStateUseCase(repository);
GameSessionId sessionId = new GameSessionId(1);

ActorSetup player = new ActorSetup(new ActorId(1), new Position(2, 2), 10, 10);
ActorSetup firstEnemy = new ActorSetup(new ActorId(2), new Position(0, 2), 6, 6);
ActorSetup secondEnemy = new ActorSetup(new ActorId(3), new Position(4, 2), 6, 6);
ActorSetup[] enemies = { firstEnemy, secondEnemy };

StartGameCommand startCommand = new StartGameCommand(sessionId, 5, 5, player, enemies);

startGame.Execute(startCommand);

submitPlayerAction.Execute(sessionId, new PlayerActionCommand.Move(Direction.Right));

GameStateDto? state = getGameState.Execute(sessionId);

if (state is not null)
{
    Console.WriteLine($"Status: {state.Status}");
    Console.WriteLine($"Player: ({state.Player.Position.X}, {state.Player.Position.Y}) HP {state.Player.CurrentHealth}/{state.Player.MaximumHealth}");
}