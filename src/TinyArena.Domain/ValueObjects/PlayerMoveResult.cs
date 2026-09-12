namespace TinyArena.Domain;

public enum PlayerMoveResult
{
    Moved,
    GameAlreadyEnded,
    OutOfBounds,
    Occupied
}