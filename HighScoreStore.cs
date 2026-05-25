namespace TheAdventure;

public class HighScoreStore
{
    private readonly string _filePath;
    private readonly List<HighScoreEntry> _scores = new();

    public HighScoreStore(string filePath)
    {
        _filePath = filePath;
    }

    public IReadOnlyList<HighScoreEntry> Scores => _scores;

    public void Load()
    {
        if (!File.Exists(_filePath))
        {
            return;
        }
    }

    public void Save()
    {
    }

    public void AddScore(int score)
    {
    }

    public IReadOnlyList<HighScoreEntry> Top5()
    {
        return Array.Empty<HighScoreEntry>();
    }
}
