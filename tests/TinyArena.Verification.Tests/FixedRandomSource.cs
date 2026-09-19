using TinyArena.Domain;

namespace TinyArena.Verification.Tests;

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
            string message = $"Configured value {_value} is outside [{minInclusive}, {maxExclusive}).";

            throw new InvalidOperationException(message);
        }

        return _value;
    }
}
