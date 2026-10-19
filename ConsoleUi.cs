/*
 ***US03/US11/US12: ConsoleUi – all inmatning och utmatning***
 ***US01-koden (antal + namn) behålls; US11 lägger till validering
 *** och visa händar; US03 huvudmenyn.
 ***
 *** Utökat med ett grafiskt konsol-UI:
 ***  • Huvudmeny med piltangentnavigering (↑/↓ + Enter)
 ***  • Highscore-skärm: mest vunnen / mest förlorad + sortering i programmet
 ***  • Reglerskärm som tydligt sammanfattar spelreglerna
 ***  • Livebord under rundan: kortgrafiken uppdateras vid varje drag
 ***  • Avgränsare mellan spelarnas turer: varje spelare får ett eget
 ***    avgränsat fält och en färgad TUR-banner när det är deras tur
 ***
 *** Om konsolen inte stödjer ReadKey (t.ex. omdirigerad inmatning)
 *** fallerar alla skärmar tillbaka till radinmatning.
 */

/// <summary>
/// ConsoleUi: samlingspunkt för all in- och utmatning.
/// Regelklasserna (Deck, Hand, Player, Dealer, Round) pratar
/// ALDRIG med Console – allt går via denna klass.
/// </summary>
class ConsoleUi
{
    /// Bredden på rubriker, linjer och banners (kortraderna får vara bredare).
    private const int ScreenWidth = 52;

    // ---------- US01: fält för antal och namn på spelare ----------

    public List<string> players = [];
    public int NumbersOfPlayers;

    /// Spelet – sätts i ShowMenu så att rundans skärmar hittar
    /// spelarna som sitter vid bordet.
    private Game? _game;

    /// Rundans givare. Round skapar en ny Dealer varje runda;
    /// ShowDealer (det första anropet i rundan) sparar den här så
    /// att livebordet kan rita givarens hand.
    private Dealer? _dealer;

    /// Är givarens hålkort synligt? (dolt tills givaren vänder det)
    private bool _holeRevealed;

    // ================================================================
    // Tangenthantering – piltangentnavigering
    // ================================================================

    /// <summary>
    /// Kan konsolen läsa enskilda tangenttryck (piltangenter, Enter, Esc)?
    /// Nej när inmatningen är omdirigerad (t.ex. via pipe) – då fallerar
    /// skärmarna tillbaka till radinmatning så att spelet alltid går att köra.
    /// </summary>
    private static bool InteractiveConsole => !Console.IsInputRedirected;

    /// Navigeringstangenterna som skärmarna förstår.
    private enum MenuKey { None, Up, Down, Select, Cancel }

    /// <summary>Läs ett tangenttryck utan att skrifta ut det på skärmen.</summary>
    private static MenuKey ReadMenuKey()
    {
        ConsoleKeyInfo key = Console.ReadKey(intercept: true);
        return key.Key switch
        {
            ConsoleKey.UpArrow => MenuKey.Up,
            ConsoleKey.DownArrow => MenuKey.Down,
            ConsoleKey.Enter => MenuKey.Select,
            ConsoleKey.Escape => MenuKey.Cancel,
            _ => MenuKey.None
        };
    }

    // ================================================================
    // Skärmaritning – gemensamma byggstenar
    // ================================================================

    /// <summary>
    /// Tömmer skärmen så varje vy ritsas "frätt" utan skrollartefakter.
    /// När utmatningen är omdirigerad (pipe/fil) finns inget konsolfönster
    /// att töma – då skrivs en avgränsningsrad i stället.
    /// </summary>
    private static void Clear()
    {
        if (Console.IsOutputRedirected)
        {
            Console.WriteLine("\n================ SKÄRM ================");
            return;
        }

        Console.Clear();
        Console.SetCursorPosition(0, 0);
    }

    /// <summary>Ritar en rubrik med linjer över och under.</summary>
    private static void DrawTitle(string text)
    {
        int width = Math.Max(ScreenWidth, text.Length + 6);
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(new string('═', width));
        Console.WriteLine("   " + text.PadRight(width - 2));
        Console.WriteLine(new string('═', width));
        Console.ResetColor();
        Console.WriteLine();
    }

    /// <summary>
    /// En färgad banner – avgränsaren mellan spelarnas turer
    /// (gul "TUR: KALLE") eller ett larm (röd "SPRÄCKT!").
    /// </summary>
    private static void DrawBanner(string text, ConsoleColor color)
    {
        int width = Math.Max(ScreenWidth, text.Length + 8);
        string line = new string('━', width);
        Console.BackgroundColor = color;
        Console.ForegroundColor = ConsoleColor.Black;
        Console.WriteLine(line);
        Console.WriteLine("   " + text.PadRight(width - 2));
        Console.WriteLine(line);
        Console.ResetColor();
        Console.WriteLine();
    }

    /// <summary>En tunn avgränsande linje mellan sektioner på livebordet.</summary>
    private static void DrawRule()
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine(new string('─', ScreenWidth));
        Console.ResetColor();
    }

    /// <summary>En sektionrubrik (t.ex. "KALLE  (14)"), markerad i gult om aktiv.</summary>
    private static void DrawSectionHeader(string text, bool active)
    {
        string marker = active ? "►" : " ";
        Console.ForegroundColor = active ? ConsoleColor.Yellow : ConsoleColor.Cyan;
        Console.WriteLine($"   {marker} {text}");
        Console.ResetColor();
    }

    /// <summary>Mörkgrå hjälptext i skärmens botten.</summary>
    private static void DrawFooter(string text)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine();
        Console.WriteLine("   " + text);
        Console.ResetColor();
    }

    /// <summary>Stanna tills användaren trycker Enter (fungerar i båda inmatningslägena).</summary>
    private static void WaitForEnter()
    {
        DrawFooter("Tryck Enter för att fortsätta ...");
        Console.ReadLine();
    }

    // ================================================================
    // US03: huvudmeny med piltangentnavigering
    // ================================================================

    /// <summary>Antal spelare som sitter vid bordet (sparade minus tillgängliga).</summary>
    private static int SeatedCount(Game game)
        => game.SavedPlayers.Count - game.GetAvailablePlayers().Count;

    /// <summary>
    /// Huvudmenyn: Starta runda / Highscores / Spelare / Regler / Avsluta.
    /// Navigera med ↑/↓ och bekräfta med Enter; med omdirigerad inmatning
    /// skriver man menynumret i stället.
    /// Returnerar true om användaren valde att spela en runda,
    /// false om programmet ska avslutas.
    /// </summary>
    public bool ShowMenu(Game game)
    {
        _game = game;
        string[] items =
        [
            "Starta runda",
            "Highscores",
            "Spelare (visa/lägg till)",
            "Regler",
            "Avsluta"
        ];
        int cursor = 0;

        while (true)
        {
            Clear();
            DrawTitle("BLACKJACK – TJUGOETT");

            // Menyposterna; den markerade är gul med en pekare framför.
            for (int i = 0; i < items.Length; i++)
            {
                bool selected = i == cursor;
                string marker = selected ? "►" : " ";
                Console.ForegroundColor = selected ? ConsoleColor.Yellow : ConsoleColor.White;
                Console.WriteLine($"    {marker} {items[i]}");
                Console.ResetColor();
            }

            DrawFooter(InteractiveConsole
                ? "↑/↓ = välj    Enter = bekräfta"
                : "Skriv menynumret (1–5) och tryck Enter");

            int choice;
            if (InteractiveConsole)
            {
                MenuKey key;
                do
                {
                    key = ReadMenuKey();
                }
                while (key is MenuKey.None or MenuKey.Cancel); // Esc ignoreras i huvudmenyn

                if (key == MenuKey.Up)
                {
                    cursor = (cursor + items.Length - 1) % items.Length; // wrapar runt
                    continue;
                }
                if (key == MenuKey.Down)
                {
                    cursor = (cursor + 1) % items.Length; // wrapar runt
                    continue;
                }

                choice = cursor + 1; // Enter bekräftar den markerade
            }
            else
            {
                choice = int.Parse(ReadUntilValid(
                    "Välj: ",
                    " === VÄLJ ETT NUMMER FRÅN MENYN ===",
                    s => int.TryParse(s, out int n) && n >= 1 && n <= items.Length));
            }

            switch (choice)
            {
                case 1:
                    if (SeatedCount(game) == 0)
                    {
                        // Ingen spelare vid bordet – informera och visa menyn igen.
                        Clear();
                        DrawTitle("BLACKJACK – TJUGOETT");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("   Inga spelare sitter vid bordet.");
                        Console.WriteLine("   Välj 'Spelare (visa/lägg till)' först.");
                        Console.ResetColor();
                        WaitForEnter();
                        continue;
                    }
                    return true;

                case 2:
                    ShowHighscores(game);
                    break;

                case 3:
                    ShowPlayers(game);
                    break;

                case 4:
                    ShowRules();
                    break;

                case 5:
                    return false;
            }
        }
    }

    // ================================================================
    // Highscores – mest vunnen/förlorad + sortering i programmet
    // ================================================================

    /// <summary>
    /// Highscore-skärmen. Visar mest vunnen och mest förlorad spelare
    /// upptill och en tabell över allas statistik.
    /// Sortering i programmet: 1 = flest vinstar, 2 = flest förluster,
    /// 3 = namn A-Ö. Enter/Esc (eller en tom rad) tillbaka till menyn.
    /// </summary>
    public void ShowHighscores(Game game)
    {
        List<Player> all = game.SavedPlayers.ToList();
        int sortMode = 1;      // 1 = vinstar, 2 = förluster, 3 = namn
        int firstVisible = 0;  // scroll: index på första synliga raden

        while (true)
        {
            List<Player> sorted = SortHighscores(all, sortMode);

            Clear();
            DrawTitle("HIGHSCORES");

            if (all.Count == 0)
            {
                Console.WriteLine("   (inga sparade spelare)");
            }
            else
            {
                // Mest vunnen / mest förlorad – skärmens topp.
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"   MEST VUNNET:   {TopScore(all, p => p.Wins, "vinst", "vinstar")}");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"   MEST FÖRLORAT: {TopScore(all, p => p.Losses, "förlust", "förluster")}");
                Console.ResetColor();
                Console.WriteLine();

                // Statistiktabellen; scrollas om den är längre än fönstret.
                int rows = Math.Min(VisibleRows(), sorted.Count);

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"   {"#",3}  {"Namn",-18} {"Vinstar",8} {"Förluster",10} {"Lika",4}");
                Console.ResetColor();

                for (int i = 0; i < rows; i++)
                {
                    Player p = sorted[firstVisible + i];
                    int rank = firstVisible + i + 1;
                    Console.WriteLine($"   {rank,3}  {p.Name,-18} {p.Wins,8} {p.Losses,10} {p.Pushes,4}");
                }

                if (sorted.Count > rows)
                    DrawFooter($"... {sorted.Count - rows} spelare till   ·   ↑/↓ = scrolla");
            }

            DrawFooter("[1] flest vinstar   [2] flest förluster   [3] namn A-Ö   ·   Enter/Esc = tillbaka");

            // Läs nästa kommando.
            if (InteractiveConsole)
            {
                while (true)
                {
                    ConsoleKeyInfo key = Console.ReadKey(intercept: true);
                    if (key.Key == ConsoleKey.D1 || key.KeyChar == '1') { sortMode = 1; break; }
                    if (key.Key == ConsoleKey.D2 || key.KeyChar == '2') { sortMode = 2; break; }
                    if (key.Key == ConsoleKey.D3 || key.KeyChar == '3') { sortMode = 3; break; }
                    if (key.Key == ConsoleKey.UpArrow)
                    {
                        firstVisible = Math.Max(0, firstVisible - 1);
                        break;
                    }
                    if (key.Key == ConsoleKey.DownArrow)
                    {
                        firstVisible++; // kläms till max nedan
                        break;
                    }
                    if (key.Key is ConsoleKey.Enter or ConsoleKey.Escape)
                        return; // tillbaka till huvudmenyn
                    // övriga tangenter ignoreras
                }
            }
            else
            {
                Console.Write("   Sortera (1/2/3) eller Enter för att gå tillbaka: ");
                string input = (Console.ReadLine() ?? "").Trim();
                if (input is "1" or "2" or "3")
                    sortMode = int.Parse(input);
                else
                    return; // tom rad eller annat = tillbaka
            }

            // Kläm scroll-offsetet till ett giltigt intervall.
            int maxOffset = Math.Max(0, sorted.Count - VisibleRows());
            firstVisible = Math.Clamp(firstVisible, 0, maxOffset);
        }
    }

    /// <summary>
    /// Sorterar highscorelistan: 1 = flest vinstar (sedan minst förluster
    /// vid oavgjort), 2 = flest förluster (sedan flest vinstar vid oavgjort),
    /// 3 = namn A-Ö.
    /// </summary>
    private static List<Player> SortHighscores(List<Player> players, int sortMode)
    {
        return sortMode switch
        {
            1 => players.OrderByDescending(p => p.Wins)
                        .ThenBy(p => p.Losses)
                        .ThenBy(p => p.Name, StringComparer.OrdinalIgnoreCase)
                        .ToList(),
            2 => players.OrderByDescending(p => p.Losses)
                        .ThenByDescending(p => p.Wins)
                        .ThenBy(p => p.Name, StringComparer.OrdinalIgnoreCase)
                        .ToList(),
            _ => players.OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase)
                        .ToList()
        };
    }

    /// <summary>
    /// Spelaren (spelarna) med högst värde i statistiken. Oavgjorda
    /// toppar listas alla, förenade med " & ". Returnerar t.ex.
    /// "Kalle & Hedda (2 vinstar)".
    /// </summary>
    private static string TopScore(IReadOnlyList<Player> players, Func<Player, int> score,
        string wordSingular, string wordPlural)
    {
        if (players.Count == 0)
            return "(inga)";

        int best = players.Max(score);
        string names = string.Join(" & ", players.Where(p => score(p) == best).Select(p => p.Name));
        string unit = best == 1 ? wordSingular : wordPlural;
        return $"{names} ({best} {unit})";
    }

    /// <summary>Hur många tabellrader som ryms på skärmen.</summary>
    private static int VisibleRows()
    {
        try
        {
            // Rubrik, toppstatistik, tabellrubrik och hjälptext tar ca 13 rader.
            return Math.Max(4, Console.WindowHeight - 13);
        }
        catch
        {
            return 20; // omdirigerad konsol utan fönsterhöjd
        }
    }

    // ================================================================
    // Reglerskärmen – tydliga spelregler
    // ================================================================

    /// <summary>
    /// En skärm som tydligt sammanfattar spelets regler.
    /// Enter (eller en rad i fallback-läget) tillbaka till menyn.
    /// </summary>
    public void ShowRules()
    {
        Clear();
        DrawTitle("SPELREGLER");

        // Raderna som inte börjar med blanksteg är rubriker.
        string[] rules =
        [
            "MÅL",
            "  Kom närmare 21 än givaren utan att överstiga 21.",
            "",
            "KORTENS VÄRDE",
            "  2–10: sitt eget värde",
            "  Knekt, Dama, Kung: 10",
            "  Ess: 1 eller 11 – det som är bäst för handen",
            "",
            "TJUGOETT (BLACKJACK)",
            "  Naturlig 21 (ess + tiokort/klätt på de två första korten)",
            "  slår en 21 byggd av tre eller fler kort.",
            "",
            "DIN TUR",
            "  Köpa (k) = ta ett kort till · Stanna (s) = avsluta din tur.",
            "  Över 21 = spräckt = förlust direkt.",
            "",
            "GIVAREN",
            "  Väntar tills alla spelare stannat eller spräckt, vänder",
            "  hålkortet och köpar medan värdet är 16 eller lägre.",
            "  Stannar på 17 eller högre (även mjuk 17).",
            "",
            "AVGÖRANDE",
            "  Vinst · Förlust · Lika (push) · Tjugoett.",
            "  Lika räknas varken som vinst eller förlust.",
            "",
            "LEKEN",
            "  52 kort. Blandas om när färre än 15 kort är kvar.",
            "  1–5 spelare per runda."
        ];

        foreach (string line in rules)
        {
            if (line.Length == 0)
            {
                Console.WriteLine();
                continue;
            }

            bool header = !line.StartsWith("  ");
            Console.ForegroundColor = header ? ConsoleColor.Cyan : ConsoleColor.White;
            Console.WriteLine(line);
            Console.ResetColor();
        }

        WaitForEnter();
    }

    // ================================================================
    // Spelare – visa vid bordet / tillgängliga, lägg till (US01/US02)
    // ================================================================

    /// <summary>
    /// Visar spelare vid bordet respektive tillgängliga spelare
    /// och ger möjlighet att lägga till nya.
    /// </summary>
    private void ShowPlayers(Game game)
    {
        Clear();
        DrawTitle("SPELARE");

        List<Player> available = game.GetAvailablePlayers();
        List<Player> seated = game.SavedPlayers
            .Where(p => !available.Contains(p)).ToList();

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("   VID BORDET");
        Console.ResetColor();
        if (seated.Count == 0)
        {
            Console.WriteLine("     (inga)");
        }
        else
        {
            foreach (Player p in seated)
                Console.WriteLine($"     {p.Name}  (W {p.Wins} / L {p.Losses} / T {p.Pushes})");
        }

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("   TILLGÄNGLIGA");
        Console.ResetColor();
        if (available.Count == 0)
        {
            Console.WriteLine("     (inga)");
        }
        else
        {
            foreach (Player p in available)
                Console.WriteLine($"     {p.Name}  (W {p.Wins} / L {p.Losses} / T {p.Pushes})");
        }

        string add = ReadUntilValid(
            "\n   Vill du lägga till spelare? (j/nej) ",
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
            Console.WriteLine($"   {name} sitter nu vid bordet.");
        }
        players.Clear();
    }

    /// <summary>Frågar antalet spelare (1–5), validerat.</summary>
    public void Print()
    {
        Console.WriteLine("1 - 5 spelare");
        NumbersOfPlayers = int.Parse(
            ReadUntilValid(
                "Välj antalet spelare: ",
                " === ANGE ETT NUMMER MELLAN 1 OCH 5 ===",
                s => int.TryParse(s, out int n) && n is >= 1 and <= 5));
    }

    /// <summary>Frågar namnen på alla spelare, validerade.</summary>
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

    // ================================================================
    // US08/US11/US12: rundan – livebord, turer och resultat
    // ================================================================

    /// <summary>
    /// Spelarens tur: ritar livebordet med en gul TUR-banner
    /// (avgränsaren mellan spelarnas turer) och läser beslutet.
    /// k eller ← = köpa, s eller → = stanna (piltangenterna
    /// fungerar bara i interaktiv konsol).
    /// </summary>
    public Move ReadMove(Player player)
    {
        DrawBoard(_game!, activePlayer: player,
            banner: $"TUR: {player.Name.ToUpperInvariant()}");

        if (InteractiveConsole)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("   Köpa (k) eller stanna (s)?    (k/← köpa · s/→ stanna)");
            Console.ResetColor();

            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(intercept: true);
                if (key.Key == ConsoleKey.K || key.Key == ConsoleKey.LeftArrow)
                    return Move.Hit;
                if (key.Key == ConsoleKey.S || key.Key == ConsoleKey.RightArrow)
                    return Move.Stand;
                // övriga tangenter ignoreras – frågan ligger kvar på skärmen
            }
        }

        return ReadUntilValid(
            "Köpa (k) eller stanna (s)? ",
            " === K ELLER S ===",
            s => s.Length == 1 && (s[0] is 'k' or 'K' or 's' or 'S'))
            .StartsWith("k", StringComparison.OrdinalIgnoreCase)
            ? Move.Hit : Move.Stand;
    }

    /// <summary>
    /// Anropas av Round: vid rundans start (hålkortet dolt) och när
    /// givaren vänder hålkortet. Sparar givaren och ritar om livebordet.
    /// </summary>
    public void ShowDealer(Dealer dealer, bool holeRevealed = false)
    {
        _dealer = dealer;
        _holeRevealed = holeRevealed;
        DrawBoard(_game!);
    }

    /// <summary>
    /// Anropas av Round när en spelare spräckt: markera hen i livebordet
    /// med en röd banner i ett avgränsat fält.
    /// </summary>
    public void ShowHand(Player player)
    {
        DrawBoard(_game!, activePlayer: player, banner: "SPRÄCKT!",
            bannerColor: ConsoleColor.Red);
    }

    /// <summary>
    /// Rundans sista vy: livebordet där alla händar (även hålkortet) är
    /// öppna, följt av avgörandet för varje spelare.
    /// </summary>
    public void ShowRoundEnd(Round round)
    {
        _holeRevealed = true; // hålkortet syns i sista vyn
        Clear();
        DrawTitle("RESULTAT");

        if (_dealer != null)
        {
            DrawSectionHeader($"GIVARE  ({_dealer.Hand.GetValue()})", active: false);
            ShowHand(_dealer.Hand.Cards);
            DrawRule();
        }

        foreach (Player p in round.Players)
        {
            (string text, ConsoleColor color) = ResultText(round.Settle(p));
            DrawSectionHeader($"{p.Name}  ({p.Hand.GetValue()})", active: false);
            ShowHand(p.Hand.Cards);

            Console.ForegroundColor = color;
            Console.WriteLine($"     {text}");
            Console.ResetColor();
            DrawRule();
        }

        WaitForEnter();
    }

    /// <summary>Resultatet som svensk text + färg att visa med.</summary>
    private static (string Text, ConsoleColor Color) ResultText(Outcome outcome)
    {
        return outcome switch
        {
            Outcome.Win => ("Vinst!", ConsoleColor.Green),
            Outcome.Lose => ("Förlust", ConsoleColor.Red),
            Outcome.Push => ("Lika", ConsoleColor.Gray),
            Outcome.Blackjack => ("Tjugoett (Blackjack)!", ConsoleColor.Green),
            _ => ("?", ConsoleColor.White)
        };
    }

    /// <summary>
    /// Ritar rundans livebord: givarens hand överst (hålkortet dolt
    /// tills det vänts) och varje spelare som sitter vid bordet i sitt
    /// eget avgränsade fält med namn och värde. Den aktiva spelaren
    /// får en banner (avgränsaren mellan turer) och sin rubrik markerad.
    /// </summary>
    private void DrawBoard(Game game, Player? activePlayer = null,
        string? banner = null, ConsoleColor bannerColor = ConsoleColor.Yellow)
    {
        Clear();
        DrawTitle("BLACKJACK – TJUGOETT");

        // --- Givarens fält (alltid överst) ---
        string dealerLabel = _dealer == null
            ? "GIVARE"
            : _holeRevealed
                ? $"GIVARE  ({_dealer.Hand.GetValue()})"
                : "GIVARE  (hålkortet dolt)";
        DrawSectionHeader(dealerLabel, active: false);
        if (_dealer != null)
            ShowHand(_dealer.Hand.Cards, hideHoleCard: !_holeRevealed);
        DrawRule();

        // --- Ett avgränsat fält per spelare ---
        foreach (Player p in game.SeatedPlayers)
        {
            bool active = ReferenceEquals(p, activePlayer);
            if (active && banner != null)
                DrawBanner(banner, bannerColor);

            string label = $"{p.Name}  ({p.Hand.GetValue()})";
            if (p.IsBusted)
                label += "  – SPRÄCKT!";
            DrawSectionHeader(label, active);

            ShowHand(p.Hand.Cards);
            DrawRule();
        }
    }

    // ================================================================
    // US12: kortgrafik (formen i CardRenderer, färgen här)
    // ================================================================

    /// <summary>
    /// Skriver ut en hand som kort, bredvid varandra och i färg.
    /// Formen kommer från CardRenderer, färgen sätts här.
    /// </summary>
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

    /// <summary>Hjärter och ruter röda, klöver och spader svarta, baksidan mörkblå.</summary>
    private static ConsoleColor CardColor(Card card, bool isFaceDown)
    {
        if (isFaceDown)
        {
            return ConsoleColor.DarkBlue;
        }

        return card.Suit is Suit.Hearts or Suit.Diamonds ? ConsoleColor.Red : ConsoleColor.Black;
    }
}