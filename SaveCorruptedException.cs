namespace TheAdventure;

public class SaveCorruptedException : Exception
{
    public SaveCorruptedException(string message) : base(message)
    {
    }

    public SaveCorruptedException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
