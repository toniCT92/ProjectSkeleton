using Silk.NET.SDL;

namespace TheAdventure;

public class SnakeBody : GameEntity
{
    private readonly List<(int X, int Y)> _segments = new();
    private Direction _currentDirection = Direction.Right;

    public SnakeBody(int startX, int startY) : base(startX, startY)
    {
        _segments.Add((startX, startY));
    }

    public IReadOnlyList<(int X, int Y)> Segments => _segments;
    public Direction CurrentDirection => _currentDirection;

    public bool TryChangeDirection(Direction next)
    {
        if (next == _currentDirection)
        {
            return false;
        }
        return false;
    }

    public override void Render(Sdl sdl, IntPtr renderer, int cellSize)
    {
    }
}
