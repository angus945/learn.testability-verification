using TinyArena.Domain;

namespace TinyArena.Application;

public sealed record ActorStateDto(
    ActorId Id,
    Position Position,
    int CurrentHealth,
    int MaximumHealth,
    bool IsDead);

public sealed record GameStateDto(
    GameSessionId SessionId,
    int Width,
    int Height,
    GameStatus Status,
    ActorStateDto Player,
    IReadOnlyList<ActorStateDto> Enemies);