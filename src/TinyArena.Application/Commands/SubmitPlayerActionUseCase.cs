using TinyArena.Domain;

namespace TinyArena.Application;

public sealed class SubmitPlayerActionUseCase
{
    private readonly IGameSessionRepository _repository;
    private readonly IRandomSource _randomSource;

    public SubmitPlayerActionUseCase(IGameSessionRepository repository, IRandomSource randomSource)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _randomSource = randomSource ?? throw new ArgumentNullException(nameof(randomSource));
    }
    public SubmitPlayerActionResult Execute(GameSessionId sessionId, PlayerActionCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        GameSession? session = _repository.Get(sessionId);

        if (session is null)
        {
            return new SubmitPlayerActionResult(SubmitPlayerActionOutcome.SessionNotFound);
        }

        SubmitPlayerActionResult result = command switch
        {
            PlayerActionCommand.Move move => ExecuteMove(session, move),
            PlayerActionCommand.Attack attack => ExecuteAttack(session, attack),
            PlayerActionCommand.Wait => ExecuteWait(session),
            _ => throw new ArgumentOutOfRangeException(nameof(command))
        };

        if (result.Outcome == SubmitPlayerActionOutcome.Executed)
        {
            _repository.Update(session);
        }

        return result;
    }

    private SubmitPlayerActionResult ExecuteMove(GameSession session, PlayerActionCommand.Move command)
    {
        PlayerMoveResult result = session.MovePlayer(command.Direction);

        return result switch
        {
            PlayerMoveResult.Moved => Executed(),
            PlayerMoveResult.GameAlreadyEnded => Rejected(PlayerActionRejectionReason.GameAlreadyEnded),
            PlayerMoveResult.OutOfBounds => Rejected(PlayerActionRejectionReason.OutOfBounds),
            PlayerMoveResult.Occupied => Rejected(PlayerActionRejectionReason.Occupied),
            _ => throw new ArgumentOutOfRangeException(nameof(result))
        };
    }

    private SubmitPlayerActionResult ExecuteAttack(GameSession session, PlayerActionCommand.Attack command)
    {
        PlayerAttackResult result = session.PlayerAttack(command.Direction, _randomSource);

        return result switch
        {
            PlayerAttackResult.Attacked => Executed(),
            PlayerAttackResult.GameAlreadyEnded => Rejected(PlayerActionRejectionReason.GameAlreadyEnded),
            PlayerAttackResult.NoTarget => Rejected(PlayerActionRejectionReason.NoTarget),
            _ => throw new ArgumentOutOfRangeException(nameof(result))
        };
    }

    private SubmitPlayerActionResult ExecuteWait(GameSession session)
    {
        PlayerWaitResult result = session.WaitPlayer();

        return result switch
        {
            PlayerWaitResult.Waited => Executed(),
            PlayerWaitResult.GameAlreadyEnded => Rejected(PlayerActionRejectionReason.GameAlreadyEnded),
            _ => throw new ArgumentOutOfRangeException(nameof(result))
        };
    }

    private static SubmitPlayerActionResult Executed()
    {
        return new SubmitPlayerActionResult(SubmitPlayerActionOutcome.Executed);
    }

    private static SubmitPlayerActionResult Rejected(PlayerActionRejectionReason reason)
    {
        return new SubmitPlayerActionResult(SubmitPlayerActionOutcome.Rejected, reason);
    }
}