namespace TinyArena.Domain;

public sealed class GameSession
{
    private readonly List<Actor> _enemies;

    public GameSessionId Id { get; }

    public int Width { get; }

    public int Height { get; }

    public Actor Player { get; }

    public IReadOnlyList<Actor> Enemies => _enemies;

    public GameStatus Status { get; private set; }

    private const int PlayerAttackDamage = 3;
    private const int EnemyAttackDamage = 2;

    public GameSession(GameSessionId id, int width, int height, Actor player, IEnumerable<Actor> enemies)
    {
        if (width <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(width));
        }

        if (height <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(height));
        }

        ArgumentNullException.ThrowIfNull(player);
        ArgumentNullException.ThrowIfNull(enemies);

        List<Actor> enemyList = enemies.ToList();

        Id = id;
        Width = width;
        Height = height;

        ValidateInitialState(player, enemyList);

        Player = player;
        _enemies = enemyList.OrderBy(enemy => enemy.Id.Value).ToList();
        Status = DetermineStatus(player, _enemies);
    }
    private void ValidateInitialState(Actor player, IReadOnlyList<Actor> enemies)
    {
        ValidateActorInsideBoard(player);

        foreach (Actor enemy in enemies)
        {
            ValidateActorInsideBoard(enemy);
        }

        ValidateUniqueActorIds(player, enemies);
        ValidateNoLivingActorOverlap(player, enemies);
    }
    private void ValidateActorInsideBoard(Actor actor)
    {
        if (!IsInsideBoard(actor.Position))
        {
            throw new ArgumentException($"Actor {actor.Id.Value} is outside the board.");
        }
    }
    private static void ValidateUniqueActorIds(Actor player, IReadOnlyList<Actor> enemies)
    {
        HashSet<ActorId> actorIds = new HashSet<ActorId>();

        if (!actorIds.Add(player.Id))
        {
            throw new ArgumentException($"Duplicate actor id: {player.Id.Value}.");
        }

        foreach (Actor enemy in enemies)
        {
            if (!actorIds.Add(enemy.Id))
            {
                throw new ArgumentException($"Duplicate actor id: {enemy.Id.Value}.");
            }
        }
    }
    private static void ValidateNoLivingActorOverlap(Actor player, IReadOnlyList<Actor> enemies)
    {
        HashSet<Position> occupiedPositions = new HashSet<Position>();

        if (!player.IsDead)
        {
            occupiedPositions.Add(player.Position);
        }

        foreach (Actor enemy in enemies)
        {
            if (enemy.IsDead)
            {
                continue;
            }

            if (!occupiedPositions.Add(enemy.Position))
            {
                throw new ArgumentException(
                    $"Multiple living actors occupy position ({enemy.Position.X}, {enemy.Position.Y}).");
            }
        }
    }

    private static GameStatus DetermineStatus(Actor player, IReadOnlyList<Actor> enemies)
    {
        if (player.IsDead)
        {
            return GameStatus.Lost;
        }

        bool hasLivingEnemy = enemies.Any(enemy => !enemy.IsDead);

        if (!hasLivingEnemy)
        {
            return GameStatus.Won;
        }

        return GameStatus.Running;
    }

    public PlayerMoveResult MovePlayer(Direction direction)
    {
        if (Status != GameStatus.Running)
        {
            return PlayerMoveResult.GameAlreadyEnded;
        }

        Position targetPosition = GetTargetPosition(Player.Position, direction);

        if (!IsInsideBoard(targetPosition))
        {
            return PlayerMoveResult.OutOfBounds;
        }

        if (IsOccupiedByLivingActor(targetPosition, Player))
        {
            return PlayerMoveResult.Occupied;
        }

        Player.MoveTo(targetPosition);

        CompletePlayerTurn();

        return PlayerMoveResult.Moved;
    }
    public PlayerAttackResult PlayerAttack(Direction direction, IRandomSource randomSource)
    {
        ArgumentNullException.ThrowIfNull(randomSource);

        if (Status != GameStatus.Running)
        {
            return PlayerAttackResult.GameAlreadyEnded;
        }

        Position targetPosition = GetTargetPosition(Player.Position, direction);
        Actor? target = FindLivingEnemyAt(targetPosition);

        if (target is null)
        {
            return PlayerAttackResult.NoTarget;
        }

        int damage = randomSource.Next(1, 5);

        target.ReceiveDamage(damage);

        CompletePlayerTurn();

        return PlayerAttackResult.Attacked;
    }
    public PlayerWaitResult WaitPlayer()
    {
        if (Status != GameStatus.Running)
        {
            return PlayerWaitResult.GameAlreadyEnded;
        }

        if (Player.IsDead)
        {
            return PlayerWaitResult.PlayerDead;
        }

        CompletePlayerTurn();

        return PlayerWaitResult.Waited;
    }
    private void CompletePlayerTurn()
    {
        UpdateStatus();

        if (Status != GameStatus.Running)
        {
            return;
        }

        ExecuteEnemyTurn();

        UpdateStatus();
    }
    private Actor? FindLivingEnemyAt(Position position)
    {
        foreach (Actor enemy in _enemies)
        {
            if (!enemy.IsDead && enemy.Position == position)
            {
                return enemy;
            }
        }

        return null;
    }
    private void ExecuteEnemyTurn()
    {
        foreach (Actor enemy in _enemies)
        {
            if (enemy.IsDead)
            {
                continue;
            }

            if (IsAdjacent(enemy.Position, Player.Position))
            {
                Player.ReceiveDamage(EnemyAttackDamage);
                UpdateStatus();

                if (Status != GameStatus.Running)
                {
                    break;
                }

                continue;
            }

            Position targetPosition = GetEnemyMoveTarget(enemy);

            if (IsOccupiedByLivingActor(targetPosition, enemy))
            {
                continue;
            }

            enemy.MoveTo(targetPosition);
        }
    }
    private Position GetEnemyMoveTarget(Actor enemy)
    {
        Position current = enemy.Position;
        Position playerPosition = Player.Position;

        if (current.X < playerPosition.X)
        {
            return new Position(current.X + 1, current.Y);
        }

        if (current.X > playerPosition.X)
        {
            return new Position(current.X - 1, current.Y);
        }

        if (current.Y < playerPosition.Y)
        {
            return new Position(current.X, current.Y + 1);
        }

        if (current.Y > playerPosition.Y)
        {
            return new Position(current.X, current.Y - 1);
        }

        return current;
    }
    private static bool IsAdjacent(Position first, Position second)
    {
        int xDistance = Math.Abs(first.X - second.X);
        int yDistance = Math.Abs(first.Y - second.Y);

        return xDistance + yDistance == 1;
    }
    private bool IsOccupiedByLivingActor(Position position, Actor? ignoredActor = null)
    {
        if (!Player.IsDead && Player.Position == position && !ReferenceEquals(Player, ignoredActor))
        {
            return true;
        }

        foreach (Actor enemy in _enemies)
        {
            if (ReferenceEquals(enemy, ignoredActor))
            {
                continue;
            }

            if (!enemy.IsDead && enemy.Position == position)
            {
                return true;
            }
        }

        return false;
    }

    private void UpdateStatus()
    {
        Status = DetermineStatus(Player, _enemies);
    }

    private Position GetTargetPosition(Position current, Direction direction)
    {
        return direction switch
        {
            Direction.Up => new Position(current.X, current.Y + 1),
            Direction.Down => new Position(current.X, current.Y - 1),
            Direction.Left => new Position(current.X - 1, current.Y),
            Direction.Right => new Position(current.X + 1, current.Y),
            _ => throw new ArgumentOutOfRangeException(nameof(direction))
        };
    }
    private bool IsInsideBoard(Position position)
    {
        return position.X >= 0 &&
               position.X < Width &&
               position.Y >= 0 &&
               position.Y < Height;
    }


}