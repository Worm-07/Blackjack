using System.Linq;

/*
 ***US09: Game – meny, spelare och spelloop***
 ***US01/US02-koden behålls; US09 lägger till rundor.***
 */

public class Game
{
    private readonly List<Player> _savedPlayers;
    private readonly List<Player> _seatedPlayers = new List<Player>();
    private readonly Random _random;

    // Decken delas över rundorna så att regeln "blanda om vid <15 kort"
    // gäller mellan rundor, inte bara inom en.
    private Deck? _deck;

    public Game(List<Player> savedPlayers, Random random)
    {
        _savedPlayers = savedPlayers;
        _random = random;
    }

    // Alla sparade spelare, så att GameStorage kan spara dem
    public IReadOnlyList<Player> SavedPlayers => _savedPlayers;

    // Spelare som sitter vid bordet (spelar i nästa runda)
    public IReadOnlyList<Player> SeatedPlayers => _seatedPlayers;

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

    /*
    ***US09: Spela en runda***
    */

    /// <summary>
    /// Spela en runda med alla spelare som sitter vid bordet.
    /// Decken skapas vid första rundan och blandas om när färre än 15
    /// kort är kvar. Spelarens beslut (köpa/stanna) skickas in som en
    /// funktion – Round pratar aldrig direkt med Console.
    /// </summary>
    public void PlayRound(Func<Player, Move> askPlayer)
    {
        // TODO US09 (kräver US05 Deck, US07 Move, US08 Round):
        // if (_deck == null || _deck.RemainingCards < 15)
        //     _deck = new Deck(_random);
        //
        // Round round = new(_deck, _seatedPlayers, askPlayer);
        // round.Play();
        throw new NotImplementedException();
    }
}