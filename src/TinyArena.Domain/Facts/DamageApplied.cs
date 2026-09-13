using Module.Verification.SystemFact;

namespace TinyArena.Domain;

public sealed record DamageApplied(
    GameSessionId SessionId,
    ActorId SourceActorId,
    ActorId TargetActorId,
    int Amount,
    int PreviousHealth,
    int CurrentHealth) : IDomainFact;