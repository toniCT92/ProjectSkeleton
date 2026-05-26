using Silk.NET.Maths;
using Silk.NET.SDL;

namespace TheAdventure;

public class Food : GameEntity
{
    public Food(int x, int y) : base(x, y)
    {
    }

    public void Respawn(int gridWidth, int gridHeight, SnakeBody snake)
    {
        var emptyCells = Enumerable.Range(0, gridWidth)
            .SelectMany(x => Enumerable.Range(0, gridHeight).Select(y => (X: x, Y: y)))
            .Where(c => !snake.Contains(c.X, c.Y))
            .ToList();

        if (emptyCells.Count == 0)
        {
            return;
        }

        var pick = emptyCells[Random.Shared.Next(emptyCells.Count)];
        X = pick.X;
        Y = pick.Y;
    }

    public override void Render(Sdl sdl, IntPtr renderer, int cellSize)
    {
        unsafe
        {
            var r = (Renderer*)renderer;
            sdl.SetRenderDrawColor(r, 220, 40, 40, 255);
            var rect = new Rectangle<int>(X * cellSize, Y * cellSize, cellSize, cellSize);
            sdl.RenderFillRect(r, in rect);
        }
    }
}
