using Silk.NET.SDL;

namespace TheAdventure;

public abstract class GameEntity
{
    public int X { get; protected set; }
    public int Y { get; protected set; }

    protected GameEntity(int x, int y)
    {
        X = x;
        Y = y;
    }

    public abstract void Render(Sdl sdl, IntPtr renderer, int cellSize);
}
