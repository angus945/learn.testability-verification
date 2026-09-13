using Module.Verification.SystemFact;

namespace TinyArena.Domain;

public sealed record ActorDefeated(GameSessionId SessionId, ActorId DefeatedActorId, ActorId SourceActorId) : IDomainFact;