/*
 ***US12: Dealer – hålkortet och den fasta regeln***
 */

/// Givaren ärver Participant. I stället för ett eget beslut följer
/// givaren en fast regel: köpa medan handens värde är 16 eller lägre,
/// stanna på 17 eller högre – även om det är en mjuk 17 (grundversionen;
/// US25 gör mjuk 17 inställbar via RuleSet).
public class Dealer : Participant
{
    public Dealer(string name = "Given") : base(name)
    {
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
            return Hand.GetValue() <= 16;
        }
    }
}
