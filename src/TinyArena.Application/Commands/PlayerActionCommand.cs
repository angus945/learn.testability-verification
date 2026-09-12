using TinyArena.Domain;

namespace TinyArena.Application;

public abstract record PlayerActionCommand
{
    protected PlayerActionCommand()
    {
    }

    public sealed record Move(Direction Direction) : PlayerActionCommand;

    public sealed record Attack(Direction Direction) : PlayerActionCommand;

    public sealed record Wait() : PlayerActionCommand;
}