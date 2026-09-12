namespace TinyArena.Application;

public enum SubmitPlayerActionOutcome
{
    Executed,
    Rejected,
    SessionNotFound
}

public enum PlayerActionRejectionReason
{
    GameAlreadyEnded,
    OutOfBounds,
    Occupied,
    NoTarget
}

public sealed record SubmitPlayerActionResult(SubmitPlayerActionOutcome Outcome, PlayerActionRejectionReason? RejectionReason = null);