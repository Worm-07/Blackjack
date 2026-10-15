/*
 ***US08: Round – en giv från start till avgörande***
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
    private readonly Action<Dealer, bool>? _showDealer;
    private readonly Action<Player>? _showBusted;
    private readonly Dealer _dealer = new();

    public Round(Deck deck, IReadOnlyList<Player> players, Func<Player, Move> askPlayer,
        Action<Dealer, bool>? showDealer = null, Action<Player>? showBusted = null)
    {
        _deck = deck;
        _players = new List<Player>(players);
        _askPlayer = askPlayer;
        _showDealer = showDealer;
        _showBusted = showBusted;
    }

    /// <summary>De spelare som deltar i given (för att visa resultat efter rundan).</summary>
    public IReadOnlyList<Player> Players => _players;

    /// <summary>
    /// Dela ut: två kort öppet till varje spelare, ett öppet och ett
    /// dolt till givaren.
    /// </summary>
    public void DealCards()
    {
        foreach (var player in _players)
        {
            player.Hand.Clear(); // ny runda: handen startar tom
            player.AddCard(_deck.Deal());
            player.AddCard(_deck.Deal());
        }
        _dealer.AddCard(_deck.Deal()); // upcard
        _dealer.AddCard(_deck.Deal()); // hole card
    }

    /// <summary>
    /// Spelarna i tur och ordning: fråga efter beslut (köpa/stanna),
    /// köp så länge spelaren vill. Spräckt = omedelbar förlust.
    /// </summary>
    public void PlayPlayers()
    {
        foreach (var player in _players)
        {
            while (!player.IsBusted && _askPlayer(player) == Move.Hit)
                player.AddCard(_deck.Deal());
            if (player.IsBusted)
                _showBusted?.Invoke(player); // visa den spräckta handen
        }
    }

    /// <summary>
    /// Givarens tur: vänd hålkortet, köp medan värdet är ≤16,
    /// stanna på 17 eller högre (US07).
    /// </summary>
    public void PlayDealer()
    {
        while (_dealer.ShouldHit)
            _dealer.AddCard(_deck.Deal());
        _showDealer?.Invoke(_dealer, true); // hålkortet vänt, visa slutvärde
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
        if (player.IsBusted)
            return Outcome.Lose;

        if (player.Hand.IsNatural && !_dealer.Hand.IsNatural)
            return Outcome.Blackjack;

        if (_dealer.IsBusted)
            return Outcome.Win;

        if (_dealer.Hand.IsNatural)
                        return player.Hand.IsNatural ? Outcome.Push : Outcome.Lose;

        int playerValue = player.Hand.GetValue();
        int dealerValue = _dealer.Hand.GetValue();

        if (playerValue > dealerValue)
            return Outcome.Win;
        if (playerValue < dealerValue)
            return Outcome.Lose;
        return Outcome.Push;
    }

    /// <summary>Kör hela given och uppdaterar spelarnas statistik.</summary>
    public void Play()
    {
        DealCards();
        _showDealer?.Invoke(_dealer, false); // öppet kort, hålkortet dolt
        PlayPlayers();
        PlayDealer();

        foreach (var player in _players)
        {
            switch (Settle(player))
            {
                case Outcome.Win:
                case Outcome.Blackjack:
                    player.RecordWin();
                    break;
                case Outcome.Lose:
                    player.RecordLoss();
                    break;
                case Outcome.Push:
                    player.RecordPush();
                    break;
            }
        }
    }
}
