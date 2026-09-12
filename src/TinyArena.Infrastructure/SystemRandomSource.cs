using TinyArena.Domain;

namespace TinyArena.Infrastructure;

public sealed class SystemRandomSource : IRandomSource
{
    public int Next(int minInclusive, int maxExclusive)
    {
        return Random.Shared.Next(minInclusive, maxExclusive);
    }
}