using Module.Verification.SystemFact;

namespace TinyArena.Domain;

public sealed record BattleEnded(GameSessionId SessionId, GameStatus FinalStatus) : IDomainFact;