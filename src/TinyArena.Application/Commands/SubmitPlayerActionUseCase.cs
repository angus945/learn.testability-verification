using TinyArena.Domain;
using Module.Verification.SystemFact;
using Module.Verification.SystemFact.Observability;
using Module.Verification.StateSnapshot;

namespace TinyArena.Application;

public sealed class SubmitPlayerActionUseCase
{
    private readonly IGameSessionRepository _repository;
    private readonly IRandomSource _randomSource;
    private readonly ISystemFactSink _factSink;
    public readonly GameSessionCommitter _committer;

    public SubmitPlayerActionUseCase(IGameSessionRepository repository, IRandomSource randomSource, ISystemFactSink factSink, GameSessionCommitter committer)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _randomSource = randomSource ?? throw new ArgumentNullException(nameof(randomSource));
        _factSink = factSink ?? throw new ArgumentNullException(nameof(factSink));
        _committer = committer ?? throw new ArgumentNullException(nameof(committer));
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
            CommitExecutedAction(session, command);
        }
        else if (result.Outcome == SubmitPlayerActionOutcome.Rejected && result.RejectionReason is PlayerActionRejectionReason reason)
        {
            PublishRejectedAction(sessionId, command, reason);
        }

        return result;
    }
    private void CommitExecutedAction(GameSession session, PlayerActionCommand command)
    {
        _committer.CommitChanges(session);

        PlayerActionCommitted fact = new PlayerActionCommitted(session.Id, command);
        _factSink.Publish(fact);
    }
    private void PublishRejectedAction(GameSessionId sessionId, PlayerActionCommand command, PlayerActionRejectionReason reason)
    {
        PlayerActionRejected fact = new PlayerActionRejected(sessionId, command, reason);
        _factSink.Publish(fact);
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
