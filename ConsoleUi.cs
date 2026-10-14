/*
 ***US11: Consoleui – all inmatning och utmatning***
 ***US01-koden (antal + namn) behålls; US11 lägger till
 *** validering och visa händar.
 */

/// <summary>
/// Consoleui: samlingspunkt för all in- och utmatning.
/// Regelklasserna (Deck, Hand, Player, Dealer, Round) pratar
/// ALDRIG med Console – allt går via denna klass.
/// </summary>
class Consoleui
{
    public List<string> players = [];
    public int NumbersOfPlayers;

    // ---------- US01: antal och namn på spelare ----------

    public void Print()
    {
        // TODO US11: byt ut mot ReadUntilValid så att felaktigt svar
        // aldrig lämnar NumbersOfPlayers osatt (fråga om tills giltigt).
        Console.WriteLine("1 - 5 spelare");
        Console.WriteLine("Välj antalet spelare: ");
        string? input = Console.ReadLine();
        if (int.TryParse(input, out int output))
        {
            if (output >= 1 && output <= 5)
            {
                NumbersOfPlayers = output;
            }
            else
            {
                Console.WriteLine(" === VÄLJ MELLAN 1 OCH 5 SPELARE ===");
            }
        }
        else
        {
            Console.WriteLine(" === ANGE ETT NUMMER ===");
        }
    }

    public void NamesOfPlayers()
    {
        // TODO US11: byt ut mot ReadName (validering via ReadUntilValid).
        for (int i = 0; i < NumbersOfPlayers; i++)
        {
            string? name = "";
            while (true)
            {
                Console.WriteLine("Ange namn på spelaren: ");
                name = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(name))
                {
                    Console.WriteLine(" === ANGE ETT NAMN ===");
                }
                else
                {
                    break;
                }
            }
            players.Insert(i, name);
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
        // TODO US11:
        // while (true)
        // {
        //     Console.Write(prompt);
        //     string input = Console.ReadLine() ?? "";
        //     if (isValid(input)) return input;
        //     Console.WriteLine(errorMessage);
        // }
        throw new NotImplementedException();
    }

    /// <summary>Läs in ett drag: köpa eller stanna.</summary>
    public Move ReadMove(Player player)
    {
        // TODO US11 (kräver US07 Move):
        // ShowHand(player);
        // return ReadUntilValid("Köpa (k) eller stanna (s)? ",
        //     " === K ELLER S ===",
        //     s => s.Length == 1 && (s[0] is 'k' or 'K' or 's' or 'S'))
        //        .StartsWith('k', StringComparison.OrdinalIgnoreCase)
        //        ? Move.Hit : Move.Stand;
        throw new NotImplementedException();
    }

    // ---------- US11: visa händar ----------

    /// <summary>Visa en spelares hand: korten + totalt värde.</summary>
    public void ShowHand(Player player)
    {
        // TODO US11 (kräver US06/US07: player.Hand):
        // foreach (Card card in player.Hand.Cards)
        //     Console.WriteLine($"  {card}");
        // Console.WriteLine($"{player.Name}: {player.Hand.GetValue()}");
        throw new NotImplementedException();
    }

    /// <summary>Visa givarens hand. Hålkortet visas som ?? tills det vänts.</summary>
    public void ShowDealer(Dealer dealer, bool holeRevealed = false)
    {
        // TODO US11 (kräver US07: Dealer.HoleCard/UpCard):
        // Visa upcard + (holeRevealed ? HoleCard : "??")
        throw new NotImplementedException();
    }

    /// <summary>Visa rundans utfall för en spelare.</summary>
    public void ShowResult(Player player, Outcome outcome)
    {
        // TODO US11 (kräver US07: Outcome):
        // t.ex. "Vinst! / Förlust / Lika / Tjugoett!"
        throw new NotImplementedException();
    }
}