/*
 ***US15 (Tillval 4): Player med marker och satsning***
 ***Skelett – implementera TODO:erna.***
 */

/// En spelare som sitter vid bordet. Ärver namn och hand från
/// Participant och har sin egen statistik och sin egen plånbok.
public class Player : Participant
{
    public int Wins { get; private set; }
    public int Losses { get; private set; }
    public int Pushes { get; private set; }

    /// <summary>Markerna och satsningarna (Tillval 4).</summary>
    public ChipWallet Chips { get; }

    public Player(string name) : base(name)
    {
        Chips = new ChipWallet();
    }

    // Används av GameStorage (US10) när resultat läses in ur filen.
    // TODO Tillval 4: lägg till chips i filformatet.
    public Player(string name, int wins, int losses, int pushes) : base(name)
    {
        Wins = wins;
        Losses = losses;
        Pushes = pushes;
        Chips = new ChipWallet();
    }

    /// <summary>Räkna en vunnen runda.</summary>
    public void RecordWin() => Wins++;

    /// <summary>Räkna en förlorad runda.</summary>
    public void RecordLoss() => Losses++;

    /// <summary>Räkna en lika runda.</summary>
    public void RecordPush() => Pushes++;

    /// <summary>
    /// Tillval 4: koppla ihop rundans utfall med plånboken –
    /// vinst → CollectWin, naturlig 21 → CollectBlackjack,
    /// lika → ReturnBet, förlust → CollectLoss.
    /// </summary>
    public void SettleChips(Outcome outcome)
    {
        // TODO Tillval 4 (kräver US07 Outcome):
        // outcome switch
        // {
        //     Outcome.Win => Chips.CollectWin(),
        //     Outcome.Blackjack => Chips.CollectBlackjack(),
        //     Outcome.Push => Chips.ReturnBet(),
        //     _ => Chips.CollectLoss(),
        // };
        throw new NotImplementedException();
    }
}