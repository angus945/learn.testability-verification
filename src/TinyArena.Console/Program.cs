using TinyArena.Application;
using TinyArena.Domain;
using TinyArena.Infrastructure;

InMemoryGameSessionRepository repository = new InMemoryGameSessionRepository();
SystemRandomSource randomSource = new SystemRandomSource();

StartGameUseCase startGame = new StartGameUseCase(repository);
SubmitPlayerActionUseCase submitPlayerAction = new SubmitPlayerActionUseCase(repository, randomSource);
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