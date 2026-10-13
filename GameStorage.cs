using System.Linq;

public class GameStorage
{
    private readonly List<Player> _savedPlayers;
    private readonly List<Player> _seatedPlayers = new List<Player>();

    public GameStorage(List<Player> savedPlayers)
    {
        _savedPlayers = savedPlayers;
    }

    // Alla sparade spelare, så att GameStorage kan spara dem
    public IReadOnlyList<Player> SavedPlayers => _savedPlayers;

    // Spelare som finns sparade men inte sitter vid bordet
    public List<Player> GetAvailablePlayers()
    {
        return _savedPlayers.Where(p => !_seatedPlayers.Contains(p)).ToList();
    }

    // Returnerar sparad spelare om namnet finns, annars en ny.
    // Returnerar null om spelaren redan sitter vid bordet.
    public Player? FindOrCreatePlayer(string name)
    {
        name = name.Trim();

        Player? existing = _savedPlayers.FirstOrDefault(
            p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        if (existing != null)
            return _seatedPlayers.Contains(existing) ? null : existing;

        Player newPlayer = new Player(name);
        _savedPlayers.Add(newPlayer);
        return newPlayer;
    }

    public void SeatPlayer(Player player)
    {
        _seatedPlayers.Add(player);
    }
}