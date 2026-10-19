using System.Linq;

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
    public int BetAmount;

    // ---------- US01: antal och namn på spelare ----------

    public void Print(int maxPlayers = 5)
    {
        Console.WriteLine($"1 - {maxPlayers} spelare");
        NumbersOfPlayers = int.Parse(
            ReadUntilValid(
                "Välj antalet spelare: ",
                $" === ANGE ETT NUMMER MELLAN 1 OCH {maxPlayers} ===",
                s => int.TryParse(s, out int n) && n >= 1 && n <= maxPlayers));
    }

    public void NamesOfPlayers(int count)
    {
        for (int i = 0; i < count; i++)
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

    public int ReadBet(Player player)
    {
        // US13: en datorspelare sätter sitt eget spel – den frågas inte.
        if (player.IsComputer)
            return Math.Min(10, player.Wallet.Balance);

        Console.WriteLine($"Saldo: {player.Wallet.Balance}");
        BetAmount = int.Parse(
            ReadUntilValid(
                "Hur många marker vill du satsa?  ",
                "=== Ange en insättning över 0 och en du har råd med... ===",
                s => int.TryParse(s, out int n) && player.Wallet.CanAfford(n)));
        return BetAmount;
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
        ShowHand(player.Hand.Cards);
        Console.WriteLine($"{player.Name}: {player.Hand.GetValue()}");
    }

    /// <summary>Visa givarens hand. Hålkortet visas som ?? tills det vänts.</summary>
    public void ShowDealer(Dealer dealer, bool holeRevealed = false)
    {
        ShowHand(dealer.Hand.Cards, hideHoleCard: !holeRevealed);

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

      // ---------- Tillval 3: husregler ----------

    /// <summary>
    /// Visar husreglerna som gäller och eventuella meddelanden från
    /// inläsningen (saknad fil, felaktiga rader).
    /// </summary>
    public void ShowRules(RuleSet rules)
    {
        foreach (string message in rules.Messages)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($" OBS: {message}");
            Console.ResetColor();
        }

        Console.WriteLine();
        Console.WriteLine("=== Husregler ===");
        Console.WriteLine($"  Antal lekar: {rules.NumberOfDecks}");
        Console.WriteLine($"  Blandas om vid färre än {rules.ReshuffleThreshold} kort");
        Console.WriteLine($"  Givaren {(rules.DealerHitsOnSoftSeventeen ? "drar" : "stannar")} på mjuk 17");
        Console.WriteLine($"  Naturlig tjugoett {(rules.NaturalBeatsThreeCardTwentyOne ? "slår" : "slår inte")} 21 av tre kort");
    }

  
    // ---------- US13: datorspelare ----------

    /// <summary>Fråga hur många av de valda spelarna som ska vara
    /// datorspelare. Det måste alltid bli minst en mänsklig spelare,
    /// så max är antalet valda spelare minus ett.</summary>
    public int AskComputerCount()
    {
        int maxComputers = NumbersOfPlayers - 1;
        return int.Parse(ReadUntilValid(
            $"Hur många av dem ska vara datorspelare? (0–{maxComputers}) ",
            $" === ANGE ETT NUMMER MELLAN 0 OCH {maxComputers} ===",
            s => int.TryParse(s, out int n) && n >= 0 && n <= maxComputers));
    }

    /// <summary>Fråga vilken strategi datorspelaren ska ha.</summary>
    public IPlayerStrategy AskStrategy()
    {
        Console.WriteLine("Vilken strategi ska datorspelaren ha?");
        Console.WriteLine("  1. Enkel (köpa under 17)");
        Console.WriteLine("  2. Grundstrategi (tabell)");

        string choice = ReadUntilValid(
            "Välj strategi: ",
            " === VÄLJ 1 ELLER 2 ===",
            s => s is "1" or "2");

        return choice == "1" ? new SimpleStrategy() : new BasicStrategy();
    }

    /// <summary>Visa vad en datorspelare gör så att man kan följa rundan.</summary>
    public void ShowComputerMove(Player player, Move move)
    {
        Console.WriteLine($"{player.Name} (dator): {(move == Move.Hit ? "köper" : "stannar")}");
    }

    /// <summary>Tillverkar ett unikt namn för en datorspelare:
    /// "Dator 1", "Dator 2", …</summary>
    private static string UniqueComputerName(Game game, int i)
    {
        List<Player> taken = game.SavedPlayers.Concat(game.SeatedPlayers).ToList();
        int n = i;
        string name = $"Dator {n}";
        while (taken.Any(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            name = $"Dator {++n}";
        return name;
    }

    // ---------- US03: huvudmeny ----------

    /// <summary>Antal spelare som sitter vid bordet (inkl. datorspelare, US13).</summary>
    private static int SeatedCount(Game game)
        => game.SeatedPlayers.Count;

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
                    if (SeatedCount(game) == 0)
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
        List<Player> available = game.GetAvailablePlayers();
        List<Player> seated = game.SeatedPlayers.ToList();

        Console.WriteLine();
        Console.WriteLine("Spelare vid bordet:");
        if (seated.Count == 0)
        {
            Console.WriteLine("  (inga)");
        }
        else
        {
            foreach (Player p in seated)
                Console.WriteLine($"  {p.Name}{(p.IsComputer ? " (dator)" : "")}  (W {p.Wins} / L {p.Losses} / T {p.Pushes})");
        }

        Console.WriteLine();
        Console.WriteLine("Tillgängliga spelare:");
        if (available.Count == 0)
        {
            Console.WriteLine("  (inga)");
        }
        else
        {
            foreach (Player p in available)
                Console.WriteLine($"  {p.Name}  (W {p.Wins} / L {p.Losses} / T {p.Pushes})");
        }

        string add = ReadUntilValid(
            "Vill du lägga till spelare? (j/n) ",
            " === J ELLER N ===",
            s => s.Length == 1 && (s[0] is 'j' or 'J' or 'n' or 'N'));

        if (add.StartsWith("j", StringComparison.OrdinalIgnoreCase))
            AddSeatedPlayers(game);
    }

    /// <summary>Frågar antal (US01), direkt efter det hur många av
    /// spelarna som är datorer (US13), och sedan namn (US01/US11) på
    /// de mänskliga. Sätter alla ner vid bordet.</summary>
    private void AddSeatedPlayers(Game game)
    {
        // Max 5 spelare vid bordet totalt, inte bara per gång.
        int freeSeats = 5 - SeatedCount(game);
        if (freeSeats == 0)
        {
            Console.WriteLine(" === BORDET ÄR FULLT – MAX 5 SPELARE ===");
            return;
        }

        Print(freeSeats); // US01: totalt antal spelare, validerat

        // US13: direkt efter antalet: hur många av dem ska vara datorer?
        int computers = AskComputerCount();
        int humans = NumbersOfPlayers - computers;

        // US13: datorspelarna – automatiskt namn + vald strategi, direkt ner.
        for (int i = 1; i <= computers; i++)
        {
            IPlayerStrategy strategy = AskStrategy();
            string name = UniqueComputerName(game, i);
            game.CreateAndSeatComputer(name, strategy);
            Console.WriteLine($" {name} sitter nu vid bordet (dator).");
        }

        // US01/US11: sedan de mänskliga – namn, validerat.
        // humans >= 1 alltid: AskComputerCount släpper bara igenom
        // max N−1 datorer, så minst en mänsklig spelare frågas alltid.
        NamesOfPlayers(humans);
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

    // Skriver ut en hand som kort, bredvid varandra och i färg.
    // Formen kommer från CardRenderer, färgen sätts här.
    public void ShowHand(IReadOnlyList<Card> cards, bool hideHoleCard = false)
    {
        List<string[]> renderedCards = CardRenderer.RenderCards(cards, hideHoleCard);

        if (renderedCards.Count == 0)
        {
            return;
        }

        for (int row = 0; row < renderedCards[0].Length; row++)
        {
            for (int i = 0; i < renderedCards.Count; i++)
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = CardColor(cards[i], CardRenderer.IsFaceDown(i, hideHoleCard));
                Console.Write(renderedCards[i][row]);
                Console.ResetColor();

                if (i < renderedCards.Count - 1)
                {
                    Console.Write(CardRenderer.CardSpacing);
                }
            }

            Console.WriteLine();
        }
    }

    // Hjärter och ruter röda, klöver och spader svarta, baksidan mörkblå.
    private static ConsoleColor CardColor(Card card, bool isFaceDown)
    {
        if (isFaceDown)
        {
            return ConsoleColor.DarkBlue;
        }

        return card.Suit is Suit.Hearts or Suit.Diamonds ? ConsoleColor.Red : ConsoleColor.Black;
    }
}