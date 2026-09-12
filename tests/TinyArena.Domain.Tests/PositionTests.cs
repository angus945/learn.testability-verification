using TinyArena.Domain;

namespace TinyArena.Domain.Tests;

public sealed class PositionTests
{
    [Fact]
    public void PositionsWithSameCoordinates_ShouldBeEqual()
    {
        Position first = new Position(2, 3);
        Position second = new Position(2, 3);

        Assert.Equal(first, second);
    }

    [Fact]
    public void PositionsWithDifferentCoordinates_ShouldNotBeEqual()
    {
        Position first = new Position(2, 3);
        Position second = new Position(3, 2);

        Assert.NotEqual(first, second);
    }
}