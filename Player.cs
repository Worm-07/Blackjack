/*
US16-Shoe-card-counting
 ***US16 (Tillval 5): Player med korträkning***
 ***Skelett – implementera TODO:erna.***
 

/// En spelare som sitter vid bordet. Ärver namn och hand från
/// Participant och har sin egen statistik och sin egen korträknare.
/// Räknaren är en ren observatör – den påverkar aldrig spelet.

 ***US10: Player – ärver Participant***
 

/// En spelare som sitter vid bordet. Ärver namn och hand från
/// Participant (US10) och har sin egen statistik.
dev
*/
public class Player : Participant
{
    public int Wins { get; private set; }
    public int Losses { get; private set; }
    public int Pushes { get; private set; }

    /// <summary>Korträknaren (Tillval 5).</summary>
    public HiLoCounter Counter { get; }

    public Player(string name, int decksInShoe = 1) : base(name)
    {
        Counter = new HiLoCounter(decksInShoe);
    }

    // Används av GameStorage (US10) när resultat läses in ur filen.
    public Player(string name, int wins, int losses, int pushes,
        int decksInShoe = 1) : base(name)
    public Player(string name) : base(name)
    {
    }

    // Används av GameStorage (US19) när resultat läses in ur filen.
    public Player(string name, int wins, int losses, int pushes) : base(name)
    {
        Wins = wins;
        Losses = losses;
        Pushes = pushes;
        Counter = new HiLoCounter(decksInShoe);
    }

    /// <summary>Räkna en vunnen runda.</summary>
    public void RecordWin() => Wins++;

    /// <summary>Räkna en förlorad runda.</summary>
    public void RecordLoss() => Losses++;

    /// <summary>Räkna en lika runda.</summary>
    public void RecordPush() => Pushes++;

    /// <summary>
    /// Tillval 5: anropas när ett kort blir synligt (Round/Game kallar
    /// detta). Kortet räknas men ändrar aldrig spelet.
    /// </summary>
    public void CardSeen(Card card)
    {
        // TODO Tillval 5: Counter.CardSeen(card);
        throw new NotImplementedException();
    }

    /// <summary>Räkna en vunnen runda.</summary>
    public void RecordWin() => Wins++;

    /// <summary>Räkna en förlorad runda.</summary>
    public void RecordLoss() => Losses++;

    /// <summary>Räkna en lika runda (räknas varken som vinst eller förlust).</summary>
    public void RecordPush() => Pushes++;
}