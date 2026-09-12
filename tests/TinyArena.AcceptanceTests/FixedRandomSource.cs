using TinyArena.Domain;

namespace TinyArena.AcceptanceTests;

internal sealed class FixedRandomSource : IRandomSource
{
    private readonly int _value;

    public FixedRandomSource(int value)
    {
        _value = value;
    }

    public int Next(int minInclusive, int maxExclusive)
    {
        if (_value < minInclusive || _value >= maxExclusive)
        {
            throw new InvalidOperationException(
                $"Configured value {_value} is outside [{minInclusive}, {maxExclusive}).");
        }

        return _value;
    }
}