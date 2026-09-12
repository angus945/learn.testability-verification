namespace TinyArena.Domain;

public readonly record struct GameSessionId
{
    public int Value { get; }

    public GameSessionId(int value)
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value));
        }

        Value = value;
    }
}