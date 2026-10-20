/*
 ***US08: Round – en giv från start till avgörande***
 */

/// En giv (en runda): korten delas ut, spelarna tar sina beslut i tur
/// och ordning, givaren följer sin regel och varje spelare får ett
/// avgörande. Round är orkestern – den bestämmer aldrig själv,
/// spelarna gör det.
///
/// Strukturkrav: Round pratar ALDRIG med Console. Spelarens beslut
/// skickas in som en parameter (Func&lt;Player, Card, Move&gt;) – det kan
/// vara en spelare som frågas av ConsoleUi (US11), eller en
/// IPlayerStrategy (US13). Round behöver inte veta skillnaden.
public class Round
{
    private readonly Deck _deck;
    private readonly List<Player> _players;
    private readonly Func<Player, Card, Move> _askPlayer;
    private readonly Action<Dealer, bool>? _showDealer;
    private readonly Action<Player>? _showBusted;
    private readonly Func<Player, int> _readBet;
    private readonly RuleSet _rules;
    private readonly Dealer _dealer;

    public Round(Deck deck, RuleSet rules, IReadOnlyList<Player> players, Func<Player, Card, Move> askPlayer,
        Func<Player, int> readBet, Action<Dealer, bool>? showDealer = null, Action<Player>? showBusted = null)
    {
        _deck = deck;
        _rules = rules;
        _dealer = new Dealer(rules);
        _players = new List<Player>(players);
        _askPlayer = askPlayer;
        _showDealer = showDealer;
        _showBusted = showBusted;
        _readBet = readBet;
    }

    /// De spelare som deltar i given (för att visa resultat efter rundan).
    public IReadOnlyList<Player> Players => _players;

    public void Stake()
    {
        foreach (var player in _players)
        {
            player.Wallet.Bet(_readBet(player));
        }
    }

    /// Dela ut: två kort öppet till varje spelare, ett öppet och ett
    /// dolt till givaren.
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

    /// Spelarna i tur och ordning: fråga efter beslut (köpa/stanna),
    /// köp så länge spelaren vill. Spräckt = omedelbar förlust.
    public void PlayPlayers()
    {
        foreach (var player in _players)
        {
            // DealCards har alltid körts först, så givarens öppna kort finns.
            while (!player.IsBusted && _askPlayer(player, _dealer.UpCard!) == Move.Hit)
                player.AddCard(_deck.Deal());
            if (player.IsBusted)
                _showBusted?.Invoke(player); // visa den spräckta handen
        }
    }

    /// Givarens tur: vänd hålkortet, köp medan värdet är ≤16,
    /// stanna på 17 eller högre (US07).
    public void PlayDealer()
    {
        while (_dealer.ShouldHit)
            _dealer.AddCard(_deck.Deal());
        _showDealer?.Invoke(_dealer, true); // hålkortet vänt, visa slutvärde
    }

    /// Avgörande för en spelare (tabellen i specen):
    /// spelare spräckt → Lose
    /// naturlig 21 på båda sidor → Push
    /// spelare naturlig 21 → Blackjack (eller Push mot givarens 21 av tre kort,
    ///   om husregeln NaturalBeatsThreeCardTwentyOne är avstängd)
    /// givare spräckt → Win
    /// givare naturlig 21 → Lose (eller Push mot spelarens 21 av tre kort,
    ///   om husregeln är avstängd)
    /// annars: högre hand vinner; lika → Push
    public Outcome Settle(Player player)
    {
        if (player.IsBusted)
            return Outcome.Lose;

        bool playerNatural = player.Hand.IsNatural;
        bool dealerNatural = _dealer.Hand.IsNatural;
        int playerValue = player.Hand.GetValue();
        int dealerValue = _dealer.Hand.GetValue();

        if (playerNatural && dealerNatural)
            return Outcome.Push;

        if (playerNatural)
        {
            if (!_rules.NaturalBeatsThreeCardTwentyOne && dealerValue == 21)
                return Outcome.Push;
            return Outcome.Blackjack;
        }

        if (_dealer.IsBusted)
            return Outcome.Win;

        if (dealerNatural)
        {
            if (!_rules.NaturalBeatsThreeCardTwentyOne && playerValue == 21)
                return Outcome.Push;
            return Outcome.Lose;
        }

        if (playerValue > dealerValue)
            return Outcome.Win;
        if (playerValue < dealerValue)
            return Outcome.Lose;
        return Outcome.Push;
    }

    /// Kör hela given och uppdaterar spelarnas statistik.
    public void Play()
    {
        Stake();
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
