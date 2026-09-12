namespace TinyArena.Domain;

public enum PlayerMoveResult
{
    Moved,
    GameAlreadyEnded,
    PlayerDead,
    OutOfBounds,
    Occupied
}