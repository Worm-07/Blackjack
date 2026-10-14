/*
 ***US11: ConsoleUi – all inmatning och utmatning***
 ***US01-koden (antal + namn) behålls; US11 lägger till
 *** validering och visa händar.
 ***US03: huvudmenyn (ShowMenu).
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
            string input = Console.ReadLine() ?? "";
            if (isValid(input))
                return input;
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
            .StartsWith('k', StringComparison.OrdinalIgnoreCase)
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

    // ---------- US03: huvudmeny ----------

    /// <summary>
    /// Huvudmenyn: spela runda / spelare (visa, lägga till) / avsluta.
    /// Returnerar true om spelaren ska spela en runda, false om programmet
    /// ska avslutas.
    /// </summary>
    public bool ShowMenu(Game game)
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("=== Välkommen till Blackjack! ===");
            Console.WriteLine("1. Spela runda");
            Console.WriteLine("2. Spelare (visa/lägg till)");
            Console.WriteLine("3. Avsluta");

            string choice = ReadUntilValid(
                "Välj: ",
                " === VÄLJ ETT NUMMER FRÅN MENYN ===",
                s => s is "1" or "2" or "3");

            switch (choice)
            {
                case "1":
                    if (game.SeatedPlayers.Count == 0)
                    {
                        Console.WriteLine(
                            " === INGA SPELARE VID BORDET – LÄGG TILL SPELARE (VAL 2) ===");
                        continue; // visa menyn igen
                    }
                    return true;

                case "2":
                    ShowPlayers(game);
                    break;

                case "3":
                    return false;
            }
        }
    }

    /// <summary>Visar spelare vid bordet respektive tillgängliga spelare
    /// och ger möjlighet att lägga till nya.</summary>
    private void ShowPlayers(Game game)
    {
        Console.WriteLine();
        Console.WriteLine("Spelare vid bordet:");
        if (game.SeatedPlayers.Count == 0)
        {
            Console.WriteLine("  (inga)");
        }
        else
        {
            foreach (Player p in game.SeatedPlayers)
                Console.WriteLine($"  {p.Name}  (W {p.Wins} / L {p.Losses} / T {p.Pushes})");
        }

        Console.WriteLine();
        Console.WriteLine("Tillgängliga spelare:");
        if (game.GetAvailablePlayers().Count == 0)
        {
            Console.WriteLine("  (inga)");
        }
        else
        {
            foreach (Player p in game.GetAvailablePlayers())
                Console.WriteLine($"  {p.Name}  (W {p.Wins} / L {p.Losses} / T {p.Pushes})");
        }

        string add = ReadUntilValid(
            "Vill du lägga till spelare? (j/nej) ",
            " === J ELLER NEJ ===",
            s => s.Length == 1 && (s[0] is 'j' or 'J' or 'n' or 'N'));

        if (add.StartsWith("j", StringComparison.OrdinalIgnoreCase))
            AddSeatedPlayers(game);
    }

    /// <summary>Frågar antal (US01) och namn (US01/US11) och sätter
    /// spelarna ner vid bordet.</summary>
    private void AddSeatedPlayers(Game game)
    {
        Print();          // US01: antal spelare (1–5), validerat
        NamesOfPlayers(); // US01/US11: namn, validerat

        foreach (string name in players)
        {
            Player? player = game.FindOrCreatePlayer(name);
            if (player == null)
            {
                Console.WriteLine($" === {name} SITTER REDAN VID BORDET ===");
                continue;
            }
            game.SeatPlayer(player);
            Console.WriteLine($" {name} sitter nu vid bordet.");
        }
        players.Clear();
    }
}