/*
 ***US11: ConsoleUi – all inmatning och utmatning***
 ***US01-koden (antal + namn) behålls; US11 lägger till
 *** validering och visa händar.
 */

/// <summary>
/// ConsoleUi: samlingspunkt för all in- och utmatning.
/// Regelklasserna (Deck, Hand, Player, Dealer, Round) pratar
/// ALDRIG med Console – allt går via denna klass.
/// </summary>
class ConsoleUi
{
    public List<string> players = [];
    public int NumbersOfPlayers;

    // ---------- US01: antal och namn på spelare ----------

    public void Print()
    {
        Console.WriteLine("1 - 5 spelare");
        NumbersOfPlayers = int.Parse(
            ReadUntilValid(
                "Välj antalet spelare: ",
                " === ANGE ETT NUMMER MELLAN 1 OCH 5 ===",
                s => int.TryParse(s, out int n) && n is >= 1 and <= 5));
    }

    public void NamesOfPlayers()
    {
        for (int i = 0; i < NumbersOfPlayers; i++)
        {
            players.Add(ReadUntilValid(
                "Ange namn på spelaren: ",
                " === ANGE ETT NAMN ===",
                s => !string.IsNullOrWhiteSpace(s)).Trim());
        }
    }

    // ---------- US11: validering ----------

    /// <summary>
    /// Fråga om tills svaret är giltigt – ingen inmatning får någonsin
    /// krascha programmet eller tyst acceptera fel.
    /// </summary>
    /// <param name="prompt">Frågan som visas för spelaren.</param>
    /// <param name="errorMessage">Visas om svaret inte är giltigt.</param>
    /// <param name="isValid">Returnerar true om svaret är giltigt.</param>
    /// <returns>Det första giltiga svaret.</returns>
    public string ReadUntilValid(string prompt, string errorMessage,
        Func<string, bool> isValid)
    {
        while (true)
        {
            Console.Write(prompt);
            string? line = Console.ReadLine();
            if (line == null)
            {
                // Inmatningen tog slut (EOF – t.ex. Ctrl+Z eller pipad
                // inmatning som slutat). Avsluta vänligt istället för att
                // fråga om i oändlighet.
                Environment.Exit(0);
            }
            if (isValid(line))
                return line;
            Console.WriteLine(errorMessage);
        }
    }

    /// <summary>Läs in ett drag: köpa eller stanna.</summary>
    public Move ReadMove(Player player)
    {
        ShowHand(player);
        return ReadUntilValid(
            "Köpa (k) eller stanna (s)? ",
            " === K ELLER S ===",
            s => s.Length == 1 && (s[0] is 'k' or 'K' or 's' or 'S'))
            .StartsWith("k", StringComparison.OrdinalIgnoreCase)
            ? Move.Hit : Move.Stand;
    }

    // ---------- US11: visa händar ----------

    /// <summary>Visa en spelares hand: korten + totalt värde.</summary>
    public void ShowHand(Player player)
    {
        foreach (Card card in player.Hand.Cards)
            Console.WriteLine($"  {card}");
        Console.WriteLine($"{player.Name}: {player.Hand.GetValue()}");
    }

    /// <summary>Visa givarens hand. Hålkortet visas som ?? tills det vänts.</summary>
    public void ShowDealer(Dealer dealer, bool holeRevealed = false)
    {
        string hole = holeRevealed
            ? dealer.HoleCard?.ToString() ?? "??"
            : "??";
        Console.WriteLine($"  {dealer.UpCard}  {hole}");

        // Värdet visas först när hålkortet är vänt – annars skulle
        // hålkortet "läcka" genom summan.
        if (holeRevealed)
            Console.WriteLine($"{dealer.Name}: {dealer.Hand.GetValue()}");
    }

    /// <summary>Visa rundans utfall för en spelare.</summary>
    public void ShowResult(Player player, Outcome outcome)
    {
        string text = outcome switch
        {
            Outcome.Win => "Vinst!",
            Outcome.Lose => "Förlust",
            Outcome.Push => "Lika",
            Outcome.Blackjack => "Tjugoett!",
            _ => "?"
        };
        Console.WriteLine($"{player.Name}: {text}");
    }
}