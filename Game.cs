namespace TheAdventure;

public sealed class Game : IDisposable
{
    private bool _disposed;

    public void Run()
    {
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        _disposed = true;
    }
}
