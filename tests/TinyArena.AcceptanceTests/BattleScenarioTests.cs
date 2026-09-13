// using TinyArena.Application;
// using TinyArena.Domain;
// using TinyArena.Infrastructure;

// namespace TinyArena.AcceptanceTests;

// public sealed class BattleScenarioTests
// {
//     [Fact]
//     public void AttackEnemy_WithControlledDamage_ShouldProduceExpectedGameState()
//     {
//         InMemoryGameSessionRepository repository = new InMemoryGameSessionRepository();
//         FixedRandomSource randomSource = new FixedRandomSource(4);

//         StartGameUseCase startGame = new StartGameUseCase(repository);
//         SubmitPlayerActionUseCase submitAction = new SubmitPlayerActionUseCase(repository, randomSource);
//         GetGameStateUseCase getGameState = new GetGameStateUseCase(repository);

//         GameSessionId sessionId = new GameSessionId(1);

//         ActorSetup player = new ActorSetup(
//             new ActorId(1),
//             new Position(1, 1),
//             10,
//             10);

//         ActorSetup enemy = new ActorSetup(
//             new ActorId(2),
//             new Position(2, 1),
//             6,
//             6);

//         StartGameCommand startCommand = new StartGameCommand(
//             sessionId,
//             5,
//             5,
//             player,
//             new[] { enemy });

//         startGame.Execute(startCommand);

//         SubmitPlayerActionResult actionResult = submitAction.Execute(
//             sessionId,
//             new PlayerActionCommand.Attack(Direction.Right));

//         GameStateDto? state = getGameState.Execute(sessionId);

//         Assert.Equal(SubmitPlayerActionOutcome.Executed, actionResult.Outcome);
//         Assert.NotNull(state);

//         ActorStateDto enemyState = Assert.Single(state.Enemies);

//         Assert.Equal(2, enemyState.CurrentHealth);
//         Assert.False(enemyState.IsDead);
//         Assert.Equal(GameStatus.Running, state.Status);
//     }
// }