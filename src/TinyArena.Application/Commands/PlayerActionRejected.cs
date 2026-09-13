using Module.Verification.SystemFact;
using TinyArena.Domain;

namespace TinyArena.Application;

public sealed record PlayerActionRejected(GameSessionId SessionId, PlayerActionCommand Action, PlayerActionRejectionReason Reason) : IApplicationFact;