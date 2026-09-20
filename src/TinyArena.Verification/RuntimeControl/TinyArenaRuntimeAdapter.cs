using Module.Verification.RuntimeControl;
using Module.Verification.StateSnapshot;
using TinyArena.Application;
using TinyArena.Domain;

namespace TinyArena.Verification;

public sealed class TinyArenaRuntimeAdapter
{
    private const string StartGameOperationType = "tinyarena.start-game";
    private const string SubmitPlayerActionOperationType = "tinyarena.submit-player-action";

    private readonly IStateSnapshotReader<GameSessionSnapshot> _snapshotReader;

    private readonly StartGameUseCase _startGame;
    private readonly SubmitPlayerActionUseCase _submitPlayerAction;
    private readonly RuntimeOperationExecutor<GameSessionId> _startGameExecutor;
    private readonly RuntimeOperationExecutor<SubmitPlayerActionResult> _playerActionExecutor;

    public TinyArenaRuntimeAdapter(
        StartGameUseCase startGame,
        SubmitPlayerActionUseCase submitPlayerAction,
        RuntimeOperationExecutor<GameSessionId> startGameExecutor,
        RuntimeOperationExecutor<SubmitPlayerActionResult> playerActionExecutor,
        IStateSnapshotReader<GameSessionSnapshot> snapshotReader)
    {
        _startGame = startGame ?? throw new ArgumentNullException(nameof(startGame));
        _submitPlayerAction = submitPlayerAction ?? throw new ArgumentNullException(nameof(submitPlayerAction));
        _startGameExecutor = startGameExecutor ?? throw new ArgumentNullException(nameof(startGameExecutor));
        _playerActionExecutor = playerActionExecutor ?? throw new ArgumentNullException(nameof(playerActionExecutor));
        _snapshotReader = snapshotReader ?? throw new ArgumentNullException(nameof(snapshotReader));
    }

    public OperationAdmission StartGame(StartGameCommand command, string? idempotencyKey = null, string? correlationId = null)
    {
        ArgumentNullException.ThrowIfNull(command);

        OperationDescriptor descriptor = new OperationDescriptor(StartGameOperationType, idempotencyKey, correlationId);

        Func<GameSessionId> execute = () => _startGame.Execute(command);
        Func<GameSessionId, OperationCompletion<GameSessionId>> createCompletion = CreateStartGameCompletion;

        return _startGameExecutor.Execute(descriptor, execute, createCompletion, "tinyarena.game.start-failed");
    }

    public OperationAdmission SubmitPlayerAction(GameSessionId sessionId, PlayerActionCommand command, string? idempotencyKey = null, string? correlationId = null)
    {
        ArgumentNullException.ThrowIfNull(command);

        OperationDescriptor descriptor = new OperationDescriptor(SubmitPlayerActionOperationType, idempotencyKey, correlationId);

        Func<SubmitPlayerActionResult> execute = () => _submitPlayerAction.Execute(sessionId, command);
        Func<SubmitPlayerActionResult, OperationCompletion<SubmitPlayerActionResult>> createCompletion = CreatePlayerActionCompletion;

        return _playerActionExecutor.Execute(descriptor, execute, createCompletion, "tinyarena.action.execution-failed");
    }

    public OperationRead<GameSessionId> ReadStartGame(OperationHandle handle)
    {
        return _startGameExecutor.Read(handle);
    }

    public OperationRead<SubmitPlayerActionResult> ReadPlayerAction(OperationHandle handle)
    {
        return _playerActionExecutor.Read(handle);
    }

    private OperationCompletion<GameSessionId> CreateStartGameCompletion(GameSessionId result)
    {
        string? observationBarrier = GetLatestObservationBarrier();

        return new OperationCompletion<GameSessionId>(
            OperationState.Succeeded,
            "tinyarena.game.started",
            result,
            observationBarrier);
    }

    private OperationCompletion<SubmitPlayerActionResult> CreatePlayerActionCompletion(SubmitPlayerActionResult result)
    {
        string? observationBarrier = GetLatestObservationBarrier();

        return result.Outcome switch
        {
            SubmitPlayerActionOutcome.Executed =>
                new OperationCompletion<SubmitPlayerActionResult>(
                    OperationState.Succeeded,
                    "tinyarena.action.executed",
                    result,
                    observationBarrier),

            SubmitPlayerActionOutcome.Rejected =>
                new OperationCompletion<SubmitPlayerActionResult>(
                    OperationState.Rejected,
                    "tinyarena.action.rejected",
                    result,
                    observationBarrier),

            SubmitPlayerActionOutcome.SessionNotFound =>
                new OperationCompletion<SubmitPlayerActionResult>(
                    OperationState.Rejected,
                    "tinyarena.session.not-found",
                    result,
                    observationBarrier),

            _ => throw new ArgumentOutOfRangeException(nameof(result.Outcome))
        };
    }

    private string? GetLatestObservationBarrier()
    {
        StateSnapshotRead<GameSessionSnapshot> snapshotRead = _snapshotReader.ReadLatest();

        if (snapshotRead.State != StateSnapshotReadState.Available)
        {
            return null;
        }

        return SnapshotObservationBarrier.Encode(snapshotRead.Reference);
    }
}