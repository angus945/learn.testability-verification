using TinyArena.Domain;

namespace TinyArena.Application;

public sealed record ActorSnapshot(ActorId Id, Position Position, int CurrentHealth, int MaximumHealth, bool IsDead);

public sealed class GameSessionSnapshot
{
    public GameSessionId SessionId { get; }
    public int Width { get; }
    public int Height { get; }
    public GameStatus Status { get; }
    public ActorSnapshot Player { get; }
    public IReadOnlyList<ActorSnapshot> Enemies { get; }

    public GameSessionSnapshot(GameSessionId sessionId, int width, int height, GameStatus status, ActorSnapshot player, IEnumerable<ActorSnapshot> enemies)
    {
        ArgumentNullException.ThrowIfNull(player);
        ArgumentNullException.ThrowIfNull(enemies);

        SessionId = sessionId;
        Width = width;
        Height = height;
        Status = status;
        Player = player;
        Enemies = Array.AsReadOnly(enemies.ToArray());
    }
}