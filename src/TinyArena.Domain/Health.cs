namespace TinyArena.Domain;

public readonly record struct Health
{
    public int Current { get; }

    public int Maximum { get; }

    public bool IsDead => Current == 0;

    public Health(int current, int maximum)
    {
        if (maximum <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maximum));
        }

        if (current < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(current));
        }

        if (current > maximum)
        {
            throw new ArgumentOutOfRangeException(nameof(current));
        }

        Current = current;
        Maximum = maximum;
    }
    public Health Damage(int amount)
    {
        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount));
        }

        int nextCurrent = Math.Max(0, Current - amount);

        return new Health(nextCurrent, Maximum);
    }

    public Health Heal(int amount)
    {
        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount));
        }

        int nextCurrent = Math.Min(Maximum, Current + amount);

        return new Health(nextCurrent, Maximum);
    }
}