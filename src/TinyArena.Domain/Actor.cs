namespace TinyArena.Domain;

public sealed class Actor
{
    public ActorId Id { get; }

    public Position Position { get; private set; }

    public Health Health { get; private set; }

    public bool IsDead => Health.IsDead;

    public Actor(ActorId id, Position position, Health health)
    {
        Id = id;
        Position = position;
        Health = health;
    }

    internal void MoveTo(Position position)
    {
        Position = position;
    }

    internal void ReceiveDamage(int amount)
    {
        Health = Health.Damage(amount);
    }
}