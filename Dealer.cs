/*
 ***US07: Dealer – hålkortet och den fasta regeln***
 ***Skelett – implementera TODO:erna.***
 */

/// Givaren ärver Participant. I stället för ett eget beslut följer
/// givaren en fast regel: köpa medan handens värde är 16 eller lägre,
/// stanna på 17 eller högre – även om det är en mjuk 17 (grundversionen).
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
            // TODO US07: return Hand.Count >= 1 ? Hand.Cards[0] : null;
            throw new NotImplementedException();
        }
    }

    /// Det dolda kortet (hålkortet). Syns först när givaren vänder det.
    public Card? HoleCard
    {
        get
        {
            // TODO US07: return Hand.Count >= 2 ? Hand.Cards[1] : null;
            throw new NotImplementedException();
        }
    }

    /// Givarens fasta regel: ska givaren köpa?
    /// Designfråga 4: regeln ligger på Dealer, inte i Round.
    public bool ShouldHit
    {
        get
        {
            // TODO US07 (kräver US06): return Hand.GetValue() <= 16;
            throw new NotImplementedException();
        }
    }
}
