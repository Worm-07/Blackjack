/*
 ***US12: Dealer – hålkortet och den fasta regeln***
 */

/// Givaren ärver Participant. I stället för ett eget beslut följer
/// givaren en fast regel: köpa medan handens värde är 16 eller lägre,
/// stanna på 17 eller högre – även om det är en mjuk 17 (grundversionen;
/// US25 gör mjuk 17 inställbar via RuleSet).
public class Dealer : Participant
{
    private readonly bool _hitsOnSoftSeventeen;

    public Dealer(RuleSet rules, string name = "Given") : base(name)
    {
        _hitsOnSoftSeventeen = rules.DealerHitsOnSoftSeventeen;
    }

    /// Det öppna kortet (upcard).
    public Card? UpCard
    {
        get
        {
            return Hand.Count >= 1 ? Hand.Cards[0] : null;
        }
    }

    /// Det dolda kortet (hålkortet). Syns först när givaren vänder det.
    public Card? HoleCard
    {
        get
        {
            return Hand.Count >= 2 ? Hand.Cards[1] : null;
        }
    }

    /// Givarens fasta regel: ska givaren köpa?
    /// Designfråga (US12): regeln ligger på Dealer, inte i Round.
    public bool ShouldHit
    {
        get
        {
            int value = Hand.GetValue();
            if (value <= 16)
                return true;

            // Husregel: drar givaren på mjuk 17 (t.ex. ess + 6)?
            return _hitsOnSoftSeventeen && value == 17 && Hand.IsSoft;
        }
    }
}
