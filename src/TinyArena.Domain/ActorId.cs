namespace TinyArena.Domain;

public readonly record struct ActorId
{
    public int Value { get; }

    public ActorId(int value)
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value));
        }

        Value = value;
    }
}