using System.Text.Json;

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

        var json = File.ReadAllText(_filePath);

        List<HighScoreEntry>? loaded;
        try
        {
            loaded = JsonSerializer.Deserialize<List<HighScoreEntry>>(json);
        }
        catch (JsonException ex)
        {
            throw new SaveCorruptedException($"Failed to parse high scores file at {_filePath}.", ex);
        }

        if (loaded == null)
        {
            throw new SaveCorruptedException($"High scores file at {_filePath} contained no data.");
        }

        _scores.Clear();
        _scores.AddRange(loaded);
    }

    public void Save()
    {
        var json = JsonSerializer.Serialize(_scores, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_filePath, json);
    }

    public void AddScore(int score)
    {
        _scores.Add(new HighScoreEntry(score, DateTime.UtcNow));
    }

    public IReadOnlyList<HighScoreEntry> Top5()
    {
        return _scores
            .OrderByDescending(s => s.Score)
            .Take(5)
            .ToList();
    }
}
