/*
 ***US10: Player – ärver Participant***
 */

/// En spelare som sitter vid bordet. Ärver namn och hand från
/// Participant (US10) och har sin egen statistik.
public class Player : Participant
{
    public int Wins { get; private set; }
    public int Losses { get; private set; }
    public int Pushes { get; private set; }

    // Ny spelare utan tidigare resultat.
    public Player(string name) : base(name)
    {
    }

    // Används av GameStorage (US10) när resultat läses in ur filen.
    public Player(string name, int wins, int losses, int pushes) : base(name)
    {
        Wins = wins;
        Losses = losses;
        Pushes = pushes;
    }

    /// <summary>Räkna en vunnen runda.</summary>
    public void RecordWin() => Wins++;

    /// <summary>Räkna en förlorad runda.</summary>
    public void RecordLoss() => Losses++;

    /// <summary>Räkna en lika runda (räknas varken som vinst eller förlust).</summary>
    public void RecordPush() => Pushes++;
}