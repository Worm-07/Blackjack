/*
 ***US08: Round – en giv från start till avgörande***
 ***Skelett – metoderna är markerade med TODO och ska implementeras.***
 */

/// En giv (en runda): korten delas ut, spelarna tar sina beslut i tur
/// och ordning, givaren följer sin regel och varje spelare får ett
/// avgörande. Round är orkestern – den bestämmer aldrig själv,
/// spelarna gör det.
///
/// Strukturkrav: Round pratar ALDRIG med Console. Spelarens beslut
/// skickas in som en parameter (Func<Player, Move>) – det kan vara
/// en spelare som frågas av ConsoleUi (US11), eller en strategi
/// (US13). Round behöver inte veta skillnaden.
public class Round
{
    private readonly Deck _deck;
    private readonly List<Player> _players;
    private readonly Func<Player, Move> _askPlayer;
    private readonly Dealer _dealer = new();

    public Round(Deck deck, IReadOnlyList<Player> players, Func<Player, Move> askPlayer)
    {
        _deck = deck;
        _players = new List<Player>(players);
        _askPlayer = askPlayer;
    }

    /// <summary>
    /// Dela ut: två kort öppet till varje spelare, ett öppet och ett
    /// dolt till givaren.
    /// </summary>
    public void DealCards()
    {
        // TODO US08:
        // foreach (var player in _players) { player.AddCard(_deck.Deal()); player.AddCard(_deck.Deal()); }
        // _dealer.AddCard(_deck.Deal()); // upcard
        // _dealer.AddCard(_deck.Deal()); // hole card
        throw new NotImplementedException();
    }

    /// <summary>
    /// Spelarna i tur och ordning: fråga efter beslut (köpa/stanna),
    /// köp så länge spelaren vill. Spräckt = omedelbar förlust.
    /// </summary>
    public void PlayPlayers()
    {
        // TODO US08:
        // foreach (var player in _players)
        //     while (!player.IsBusted && _askPlayer(player) == Move.Hit) player.AddCard(_deck.Deal());
        throw new NotImplementedException();
    }

    /// <summary>
    /// Givarens tur: vänd hålkortet, köp medan värdet är ≤16,
    /// stanna på 17 eller högre (US07).
    /// </summary>
    public void PlayDealer()
    {
        // TODO US08: while (_dealer.ShouldHit) _dealer.AddCard(_deck.Deal());
        throw new NotImplementedException();
    }

    /// <summary>
    /// Avgörande för en spelare (tabellen i specen):
    /// spelare spräckt → Lose
    /// spelare naturlig 21 och givare inte → Blackjack
    /// givare spräckt → Win
    /// givare naturlig 21 → Push
    /// annars: högre hand vinner; lika → Push
    /// </summary>
    public Outcome Settle(Player player)
    {
        // TODO US08 (kräver US06: Hand)
        throw new NotImplementedException();
    }

    /// <summary>Kör hela given och uppdaterar spelarnas statistik.</summary>
    public void Play()
    {
        // TODO US08: DealCards(); PlayPlayers(); PlayDealer();
        // därefter Settle per spelare → uppdatera Wins/Losses/Pushes
        throw new NotImplementedException();
    }
}
