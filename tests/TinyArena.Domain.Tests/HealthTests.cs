using TinyArena.Domain;

namespace TinyArena.Domain.Tests;

public sealed class HealthTests
{
    [Fact]
    public void Create_ShouldStoreCurrentAndMaximum()
    {
        Health health = new Health(8, 10);

        Assert.Equal(8, health.Current);
        Assert.Equal(10, health.Maximum);
    }

    [Fact]
    public void Create_WhenMaximumIsNotPositive_ShouldThrow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Health(0, 0));
    }

    [Fact]
    public void Create_WhenCurrentIsNegative_ShouldThrow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Health(-1, 10));
    }

    [Fact]
    public void Create_WhenCurrentExceedsMaximum_ShouldThrow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Health(11, 10));
    }

    [Fact]
    public void Damage_ShouldReduceCurrent()
    {
        Health health = new Health(10, 10);

        Health result = health.Damage(3);

        Assert.Equal(7, result.Current);
        Assert.Equal(10, result.Maximum);
    }

    [Fact]
    public void Damage_WhenDamageExceedsCurrent_ShouldStopAtZero()
    {
        Health health = new Health(3, 10);

        Health result = health.Damage(5);

        Assert.Equal(0, result.Current);
    }

    [Fact]
    public void Heal_ShouldIncreaseCurrent()
    {
        Health health = new Health(5, 10);

        Health result = health.Heal(3);

        Assert.Equal(8, result.Current);
    }

    [Fact]
    public void Heal_WhenAmountExceedsMaximum_ShouldStopAtMaximum()
    {
        Health health = new Health(8, 10);

        Health result = health.Heal(5);

        Assert.Equal(10, result.Current);
    }

    [Fact]
    public void IsDead_WhenCurrentIsZero_ShouldBeTrue()
    {
        Health health = new Health(0, 10);

        Assert.True(health.IsDead);
    }

    [Fact]
    public void IsDead_WhenCurrentIsPositive_ShouldBeFalse()
    {
        Health health = new Health(1, 10);

        Assert.False(health.IsDead);
    }
}