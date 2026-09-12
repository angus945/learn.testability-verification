using TinyArena.Domain;

namespace TinyArena.Application;

public sealed record StartGameCommand(GameSessionId SessionId, int Width, int Height, ActorSetup Player, IReadOnlyList<ActorSetup> Enemies);