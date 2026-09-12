namespace TinyArena.Domain;

public enum PlayerAttackResult
{
    Attacked,
    GameAlreadyEnded,
    PlayerDead,
    NoTarget
}