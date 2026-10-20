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

    /// <summary>
    /// US15: mest antal marker spelaren haft under spelet. Uppdateras
    /// efter varje runda (Round) och sparas till players.txt.
    /// </summary>
    public int MaxChipsDuringGame { get; private set; }
    public ChipWallet Wallet { get; }

    // US13: null för människa, en strategi för datorspelare.
    // Strategin bestämmer draget – Player bestämmer aldrig själv.
    public IPlayerStrategy? Strategy { get; }

    /// True om spelaren är en datorspelare (US13).
    public bool IsComputer => Strategy != null;

    // Ny spelare utan tidigare resultat.
    public Player(string name) : base(name)
    {
        Wallet = new ChipWallet();
        MaxChipsDuringGame = Wallet.Balance;
    }

    // US13: datorspelare med strategi.
    public Player(string name, IPlayerStrategy strategy) : base(name)
    {
        Wallet = new ChipWallet();
        Strategy = strategy;
        MaxChipsDuringGame = Wallet.Balance;
    }

    // Används av GameStorage (US10) när resultat läses in ur filen.
    // maxChipsDuringGame saknas i gamla filer → 0, då räcker saldot.
    public Player(string name, int wins, int losses, int pushes, int wallet,
        int maxChipsDuringGame = 0) : base(name)
    {
        Wins = wins;
        Losses = losses;
        Pushes = pushes;
        Wallet = new ChipWallet(wallet);
        MaxChipsDuringGame = Math.Max(maxChipsDuringGame, wallet);
    }

    /// Räkna en vunnen runda.
    public void RecordWin() => Wins++;

    /// Räkna en förlorad runda.
    public void RecordLoss() => Losses++;

    /// Räkna en lika runda (räknas varken som vinst eller förlust).
    public void RecordPush() => Pushes++;

    /// <summary>
    /// US15: uppdatera "mest antal marker under spel" – saldot kan ha
    /// ökat efter rundans utbetalning. Anropas av Round efter varje runda.
    /// </summary>
    public void UpdateChipsPeak()
        => MaxChipsDuringGame = Math.Max(MaxChipsDuringGame, Wallet.Balance);
}