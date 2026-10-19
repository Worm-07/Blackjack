using System.Linq;

/*
 ***US09: Game – meny, spelare och spelloop***
 ***US01/US02-koden behålls; US09 lägger till rundor.***
 */

public class Game
{
    private readonly List<Player> _savedPlayers;
    private readonly List<Player> _seatedPlayers = new List<Player>();
    private readonly RuleSet _rules;
    private readonly Random _random;

    // Decken delas över rundorna så att regeln "blanda om vid <15 kort"
    // gäller mellan rundor, inte bara inom en.
    private Deck? _deck;

     public Game(List<Player> savedPlayers, Random random, RuleSet rules)
    {
        _savedPlayers = savedPlayers;
        _random = random;
        _rules = rules;
    }

    // Husreglerna som gäller vid bordet (Tillval 3)
    public RuleSet Rules => _rules;

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

    /// <summary>
    /// US13: skapar en datorspelare med given strategi och sätter ner
    /// den vid bordet. Spelaren läggs EJ i _savedPlayers – den sparas
    /// alltså inte till players.txt och kan inte krocka med ett
    /// mänskligt namn.
    /// </summary>
    public Player CreateAndSeatComputer(string name, IPlayerStrategy strategy)
    {
        Player computer = new Player(name, strategy);
        _seatedPlayers.Add(computer);
        return computer;
    }

    /*
    ***US09: Spela en runda***
    */

    /// <summary>
    /// Spela en runda med alla spelare som sitter vid bordet.
    /// Mänskliga spelare spelar först i tur och ordning,
    /// sedan datorspelarna (US13).
    /// Decken skapas vid första rundan och blandas om när färre än 15
    /// kort är kvar. Spelarens beslut (köpa/stanna) skickas in som en
    /// funktion – Round pratar aldrig direkt med Console.
    /// Returnerar rundan så att loopen kan visa avgörandet för varje
    /// spelare (Settle är ren – samma svar före och efter Play()).
    /// </summary>
    public Round PlayRound(Func<Player, Move> askPlayer,
        Func<Player, int> readBet, Action<Dealer, bool>? showDealer = null, Action<Player>? showBusted = null)
    {
        // Ny lek vid första rundan; blandas om när korten tar slut
        // (US05: färre än 15 kort kvar → ny blandad lek).
        if (_deck == null)
        {
            _deck = new Deck(_random, _rules);
        }
        else
        {
            _deck.ReshuffleIfLow();
        }

        Round round = new(_deck, _rules, _seatedPlayers, askPlayer, showDealer, showBusted);
        //round.Play();
       
        return round;
    }
}
