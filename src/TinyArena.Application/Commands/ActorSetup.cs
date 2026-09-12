using TinyArena.Domain;

namespace TinyArena.Application;

public sealed record ActorSetup(ActorId Id, Position Position, int CurrentHealth, int MaximumHealth);