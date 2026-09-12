namespace TinyArena.Domain;

public interface IRandomSource
{
    int Next(int minInclusive, int maxExclusive);
}