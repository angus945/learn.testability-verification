using Module.Verification.SystemFact;
using TinyArena.Domain;

namespace TinyArena.Application;

public sealed record PlayerActionCommitted(
    GameSessionId SessionId,
    PlayerActionCommand Action) : IApplicationFact;