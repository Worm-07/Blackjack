/*
 ***US03/US11/US12: ConsoleUi – all inmatning och utmatning***
 ***US01-koden (antal + namn) behålls; US11 lägger till validering
 *** och visa händar; US03 huvudmenyn.
 ***
 *** Utökat med ett grafiskt konsol-UI:
 ***  • Huvudmeny med piltangentnavigering (↑/↓ + Enter)
 ***  • Highscore-skärm: mest vunnen / mest förlorad + sortering i programmet
 ***  • Reglerskärm: spelregler + husregler ur rules.txt (Tillval 3)
 ***  • Datorspelare (US13/US15): antal och strategi vid tillägg,
 ***    insats och automatiska drag
 ***  • Livebord under rundan: ritas som en grön blackjack-matta med
 ***    guldplåt "BLACKJACK PAYS 3 TO 2", korten och en rad
 ***    satsningschips (1–100) i botten; uppdateras vid varje drag.
 ***    Mattans bredd anpassas efter antalet spelare (1–5) och
 ***    konsolruttens bredd, så att alla säten ryms i en enda rad
 ***    och brädet aldrig viker om
 ***  • Avgränsare mellan spelarnas turer: gul TUR-banner på mattan
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

    /// Slumpar datorspelarnas insatser – en Random delas av alla.
    private readonly Random _computerBetRandom = new();

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
    // Blackjackmattan – livebordets "grafiska" yta
    // ================================================================

    /// Mattans minsta bredd (inre, utan sidokanter) – den klassiska
    /// bredden som ryms i en vanlig konsolruta.
    private const int MinFeltWidth = 72;

    /// Önskad bredd per säte: två kort sida vid sida (9 + 1 + 9 = 19
    /// tecken) med lite andrum. I en smal konsolruta blir sätena
    /// smalare i stället – då bryts kort och chips i flera rader.
    private const int MinSeatWidth = 22;

    /// <summary>
    /// Konsolruttens bredd i tecken. När utmatningen är omdirigerad
    /// (ingen ruta att mäta) eller mätningen misslyckas antas 80 –
    /// en vanlig minimumbredd.
    /// </summary>
    private static int ConsoleWidth
    {
        get
        {
            try
            {
                int width = Console.WindowWidth;
                return width > 0 ? width : 80;
            }
            catch (IOException)
            {
                return 80; // omdirigerad utmatning – ingen ruta att mäta
            }
            catch (InvalidOperationException)
            {
                return 80; // inget konsolvindu tillgängligt
            }
        }
    }

    /// <summary>
    /// Mattans bredd (inre, utan sidokanter) för ett antal säten:
    /// bred nog att varje säte får <see cref="MinSeatWidth"/> + 2
    /// tecken mellanrum (1–3 spelare 72, fyra 94, fem 118), men
    /// aldrig bredare än konsolrutan – annars skulle raderna vika om
    /// och mattan se trasig ut. I en smalare konsolruta fördelas
    /// bredden jämnt mellan sätena i stället (smalare säten per
    /// spelare/dator), så att alla säten ändå syns i en enda rad.
    /// </summary>
    private static int FeltWidthFor(int seatCount)
    {
        int wanted = Math.Max(MinFeltWidth,
            seatCount * MinSeatWidth + 2 * Math.Max(0, seatCount - 1));
        return Math.Max(1, Math.Min(wanted, ConsoleWidth - 2));
    }

    /// Mattans färg (mörkgrön) och bordskantens färg (guld).
    private const ConsoleColor FeltColor = ConsoleColor.DarkGreen;
    private const ConsoleColor TableRimColor = ConsoleColor.DarkYellow;

    /// <summary>
    /// En cell på mattan: ett textsnutt med egen bakgrundsfärg och
    /// textfärg. Mattan byggs av rader av celler och ritas av DrawFelt.
    /// </summary>
    private sealed class FeltCell
    {
        public FeltCell(string text, ConsoleColor? background = null, ConsoleColor? foreground = null)
        {
            Text = text;
            Bg = background;
            Fg = foreground;
        }

        public string Text { get; }
        public ConsoleColor? Bg { get; }
        public ConsoleColor? Fg { get; }
    }

    /// <summary>En mattfärgad cell (underlag, etiketter, mellanrum).</summary>
    private static FeltCell Felt(string text, ConsoleColor? foreground = null)
        => new(text, FeltColor, foreground);

    /// <summary>
    /// Centerar en rad celler: fyller ut med matta till höger och
    /// vänster så att raden blir exakt <paramref name="width"/> tecken
    /// bred. Räcker inte bredden klippas raden i stället (FitRow) – så
    /// kan t.ex. ett sätes kort och chips få sin egen, smala bredd.
    /// </summary>
    private static List<FeltCell> CenterRow(List<FeltCell> cells, int width)
    {
        int contentWidth = 0;
        foreach (FeltCell cell in cells)
            contentWidth += cell.Text.Length;

        if (contentWidth > width)
            return FitRow(cells, width);

        int left = Math.Max(0, (width - contentWidth) / 2);
        int right = width - contentWidth - left;

        List<FeltCell> row = new List<FeltCell>();
        if (left > 0)
            row.Add(Felt(new string(' ', left)));
        row.AddRange(cells);
        if (right > 0)
            row.Add(Felt(new string(' ', right)));
        return row;
    }

    /// <summary>
    /// Klipper en rad celler till exakt <paramref name="width"/> tecken:
    /// en cell som inte ryms helt får sin text avklippt vid kanten.
    /// </summary>
    private static List<FeltCell> FitRow(List<FeltCell> cells, int width)
    {
        List<FeltCell> row = new List<FeltCell>();
        int remaining = width;

        foreach (FeltCell cell in cells)
        {
            if (remaining <= 0)
                break;
            if (cell.Text.Length <= remaining)
            {
                row.Add(cell);
                remaining -= cell.Text.Length;
            }
            else
            {
                row.Add(new FeltCell(cell.Text[..remaining], cell.Bg, cell.Fg));
                remaining = 0;
            }
        }

        return row;
    }

    /// <summary>
    /// En centerad etikettrad på mattan (rubriker, blanka rader).
    /// </summary>
    private static List<FeltCell> LabelRow(string text, int width, ConsoleColor? foreground = null)
        => CenterRow([Felt(text, foreground)], width);

    /// <summary>Guldplåt på mattan: "BLACKJACK".</summary>
    private static List<FeltCell> TitlePlateRow(int width)
        => CenterRow([new FeltCell("  BLACKJACK  ", TableRimColor, ConsoleColor.Black)], width);

    /// <summary>En tunn avgränsare på mattan mellan givarens och spelarnas fält.</summary>
    private static List<FeltCell> SeparatorRow(int width)
        => LabelRow("· · · · · · · · · · · · · · ·", width, ConsoleColor.DarkCyan);

    /// <summary>
    /// Korten som celler på mattan: vita kort med röd/svart text
    /// (färgen efter kortfärgen, samma regel som ShowHand) och matta
    /// i mellanrummen. Returnerar cellerna ocenterade – det är
    /// CenterRow som fyller ut.
    /// </summary>
    private static List<FeltCell> CardCells(IReadOnlyList<Card> cards, int row, bool hideHoleCard, int startIndex = 0)
    {
        List<string[]> rendered = CardRenderer.RenderCards(cards, hideHoleCard, startIndex);
        List<FeltCell> cells = new List<FeltCell>();

        for (int i = 0; i < rendered.Count; i++)
        {
            cells.Add(new FeltCell(rendered[i][row], ConsoleColor.White,
                CardColor(cards[i], CardRenderer.IsFaceDown(i + startIndex, hideHoleCard))));

            if (i < rendered.Count - 1)
                cells.Add(Felt(CardRenderer.CardSpacing));
        }

        return cells;
    }

    /// <summary>
    /// En hand som en eller flera rader mattceller: så många kort som
    /// ryms på en rad (ett kort är 9 tecken brett + ett mellanrum) hamnar
    /// i raden och resten viker om till nästa rad ovanför. Hålkortet
    /// (andra kortet) förblir dolt även efter omslag tack vare
    /// startIndex.
    /// </summary>
    private static List<List<FeltCell>> HandRows(IReadOnlyList<Card> cards, bool hideHoleCard, int width)
    {
        List<List<FeltCell>> rows = new List<List<FeltCell>>();
        if (cards.Count == 0)
            return rows;

        // CardRenderer-kort: 7 + 2 (ramen) = 9 tecken breda.
        const int cardWidth = 9;
        int spacing = CardRenderer.CardSpacing.Length;
        int perRow = Math.Max(1, (width + spacing) / (cardWidth + spacing));
        int cardRows = (cards.Count + perRow - 1) / perRow;

        for (int rowIdx = 0; rowIdx < cardRows; rowIdx++)
        {
            int start = rowIdx * perRow;
            int count = Math.Min(perRow, cards.Count - start);
            List<Card> part = cards.Skip(start).Take(count).ToList();

            for (int r = 0; r < 7; r++) // ett kort är 7 rader högt
                rows.Add(CenterRow(CardCells(part, r, hideHoleCard, start), width));
        }

        return rows;
    }

    /// <summary>
    /// En visuell chiprad som celler på mattan: varje chip får sin
    /// egen färg (ChipColor) och mellanrummen är matta. Hur många chips
    /// som ryms i raden beror på bredden (ChipRows delar upp dem).
    /// </summary>
    private static List<List<FeltCell>> ChipRowCells(IReadOnlyList<int> values, int width)
    {
        List<string[]> chips = ChipRenderer.RenderChipRow(values);
        List<List<FeltCell>> rows = new List<List<FeltCell>>();

        for (int r = 0; r < chips[0].Length; r++)
        {
            List<FeltCell> cells = new List<FeltCell>();

            for (int i = 0; i < chips.Count; i++)
            {
                (ConsoleColor bg, ConsoleColor fg) = ChipColor(values[i]);
                cells.Add(new FeltCell(chips[i][r], bg, fg));

                if (i < chips.Count - 1)
                    cells.Add(Felt(ChipRenderer.ChipSpacing));
            }

            rows.Add(CenterRow(cells, width));
        }

        return rows;
    }

    /// <summary>
    /// Rader med satsningschips: rymmer inte alla chips i bredden delas
    /// de upp i flera rader. Ett chip är 9 tecken + 2 mellanrum, så
    /// smala breddar (t.ex. ett säte) får färre chips per rad.
    /// </summary>
    private static List<List<FeltCell>> ChipRows(IReadOnlyList<int> values, int width)
    {
        int perRow = Math.Max(1, (width + 2) / 11);

        List<List<FeltCell>> rows = new List<List<FeltCell>>();
        for (int i = 0; i < values.Count; i += perRow)
            rows.AddRange(ChipRowCells(values.Skip(i).Take(perRow).ToList(), width));
        return rows;
    }

    /// <summary>
    /// En spelares insats som chips: beloppet delas upp i vanliga
    /// chipvärden (100, 50, 25, 10, 5, 1 – störst först, som när man
    /// lägger marker på ett riktigt bord) och ritas som en eller flera
    /// rader framför spelaren.
    /// </summary>
    private static List<List<FeltCell>> BetChipRows(int amount, int width)
    {
        List<int> chips = new List<int>();
        foreach (int value in new[] { 100, 50, 25, 10, 5, 1 })
            while (amount >= value)
            {
                chips.Add(value);
                amount -= value;
            }
        return ChipRows(chips, width);
    }

    /// <summary>
    /// Chipens färg efter värdet – som riktiga casinochips:
    /// vit 1, röd 5, grön 10, blå 25, magenta 50, cyan 100.
    /// </summary>
    private static (ConsoleColor Bg, ConsoleColor Fg) ChipColor(int value) => value switch
    {
        1 => (ConsoleColor.White, ConsoleColor.Black),
        5 => (ConsoleColor.Red, ConsoleColor.White),
        10 => (ConsoleColor.Green, ConsoleColor.Black),
        25 => (ConsoleColor.Blue, ConsoleColor.White),
        50 => (ConsoleColor.Magenta, ConsoleColor.Black),
        100 => (ConsoleColor.Cyan, ConsoleColor.Black),
        _ => (ConsoleColor.DarkGray, ConsoleColor.White)
    };

    /// <summary>
    /// Ritar blackjackmattan: en guldbordkant, cellraderna emellan.
    /// Mattan är mörkgrön; varje cell kan ha egna färger (kort, chips
    /// och banners sticker ut från mattan).
    /// </summary>
    private static void DrawFelt(List<List<FeltCell>> rows, int width)
    {
        string rim = new string('─', width);

        Console.BackgroundColor = FeltColor;
        Console.ForegroundColor = TableRimColor;
        Console.WriteLine("╭" + rim + "╮");

        foreach (List<FeltCell> row in rows)
        {
            Console.ForegroundColor = TableRimColor;
            Console.Write("│");

            foreach (FeltCell cell in row)
            {
                Console.BackgroundColor = cell.Bg ?? FeltColor;
                Console.ForegroundColor = cell.Fg ?? ConsoleColor.White;
                Console.Write(cell.Text);
            }

            Console.ForegroundColor = TableRimColor;
            Console.Write("│");
            Console.ResetColor();
            Console.WriteLine();
        }

        Console.WriteLine("╰" + rim + "╯");
        Console.ResetColor();
    }


    // ================================================================
    // US03: huvudmeny med piltangentnavigering
    // ================================================================

    /// <summary>Antal spelare som sitter vid bordet (inkl. datorspelare, US13).</summary>
    private static int SeatedCount(Game game)
        => game.SeatedPlayers.Count;

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
                    s => int.TryParse(s, out int n) && n >= 1 && n <= items.Length,
                    valueOnEof: "5")); // EOF (Ctrl+Z / pipe som tar slut) = Avsluta
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
                    ShowRules(game.Rules);
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
    /// Sortering i programmet: 1 = flest vinster, 2 = flest förluster,
    /// 3 = namn A-Ö. Enter/Esc (eller en tom rad) tillbaka till menyn.
    /// </summary>
    public void ShowHighscores(Game game)
    {
        List<Player> all = game.SavedPlayers.ToList();
        int sortMode = 1;      // 1 = vinster, 2 = förluster, 3 = namn
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
                Console.WriteLine($"   MEST VUNNET:   {TopScore(all, p => p.Wins, "vinst", "vinster")}");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"   MEST FÖRLORAT: {TopScore(all, p => p.Losses, "förlust", "förluster")}");
                Console.ResetColor();
                Console.WriteLine();

                // Statistiktabellen; scrollas om den är längre än fönstret.
                int rows = Math.Min(VisibleRows(), sorted.Count);

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"   {"#",3}  {"Namn",-18} {"Vinster",8} {"Förluster",10} {"Lika",4} {"Mest antal marker under spel",28}");
                Console.ResetColor();

                for (int i = 0; i < rows; i++)
                {
                    Player p = sorted[firstVisible + i];
                    int rank = firstVisible + i + 1;
                    Console.WriteLine($"   {rank,3}  {p.Name,-18} {p.Wins,8} {p.Losses,10} {p.Pushes,4} {p.MaxChipsDuringGame,28}");
                }

                if (sorted.Count > rows)
                    DrawFooter($"... {sorted.Count - rows} spelare till   ·   ↑/↓ = scrolla");
            }

            DrawFooter("[1] flest vinster   [2] flest förluster   [3] namn A-Ö   ·   Enter/Esc = tillbaka");

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
    /// Sorterar highscorelistan: 1 = flest vinster (sedan minst förluster
    /// vid oavgjort), 2 = flest förluster (sedan flest vinster vid oavgjort),
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
    /// "Kalle & Hedda (2 vinster)".
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
    /// En skärm som tydligt sammanfattar spelets regler: först husreglerna
    /// ur rules.txt (Tillval 3) med eventuella inläsningsmeddelanden,
    /// sedan de grundläggande spelreglerna.
    /// Enter (eller en rad i fallback-läget) tillbaka till menyn.
    /// </summary>
    public void ShowRules(RuleSet rules)
    {
        Clear();
        DrawTitle("SPELREGLER");

        // Meddelanden från inläsningen av husreglerna, om sådana finns.
        foreach (string message in rules.Messages)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"   OBS: {message}");
            Console.ResetColor();
        }

        // Husreglerna som gäller vid detta bord (Tillval 3, rules.txt)
        // följt av de grundläggande spelreglerna. Raderna som inte börjar
        // med blanksteg är rubriker.
        string[] lines =
        [
            "HUSREGLER (rules.txt)",
            $"  Antal lekar: {rules.NumberOfDecks}",
            $"  Blandas om vid färre än {rules.ReshuffleThreshold} kort",
            $"  Givaren {(rules.DealerHitsOnSoftSeventeen ? "drar" : "stannar")} på mjuk 17",
            $"  Naturlig tjugoett {(rules.NaturalBeatsThreeCardTwentyOne ? "slår" : "slår inte")} 21 av tre kort",
            "",
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
            $"  {(rules.NaturalBeatsThreeCardTwentyOne ? "slår" : "slår inte")} en 21 byggd av tre eller fler kort.",
            "",
            "DIN TUR",
            "  Köpa (k) = ta ett kort till · Stanna (s) = avsluta din tur.",
            "  Över 21 = spräckt = förlust direkt.",
            "",
            "GIVAREN",
            "  Väntar tills alla spelare stannat eller spräckt, vänder",
            "  hålkortet och köpar medan värdet är 16 eller lägre.",
            rules.DealerHitsOnSoftSeventeen
                ? "  Stannar på hård 17, drar på mjuk 17."
                : "  Stannar på 17 eller högre (även mjuk 17).",
            "",
            "AVGÖRANDE",
            "  Vinst · Förlust · Lika (push) · Tjugoett.",
            "  Lika räknas varken som vinst eller förlust.",
            "",
            "LEKEN",
            $"  {rules.NumberOfDecks * 52} kort. Blandas om när färre än {rules.ReshuffleThreshold} kort är kvar.",
            "  1–5 spelare per runda."
        ];

        foreach (string line in lines)
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
        List<Player> seated = game.SeatedPlayers.ToList();

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
                Console.WriteLine($"     {p.Name}{(p.IsComputer ? " (dator)" : "")}  (W {p.Wins} / L {p.Losses} / T {p.Pushes})  (Saldo: {p.Wallet.Balance})");
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
            {
                // US15: spelare som gått slut på marker kan inte spela mer.
                string broke = p.Wallet.IsBroke ? "  (SLUT PÅ MARKER)" : "";
                Console.WriteLine($"     {p.Name}  (W {p.Wins} / L {p.Losses} / T {p.Pushes})  (Saldo: {p.Wallet.Balance}){broke}");
            }
        }

        string add = ReadUntilValid(
            "\n   Vill du lägga till spelare? (j/n) ",
            " === J ELLER N ===",
            s => s.Length == 1 && (s[0] is 'j' or 'J' or 'n' or 'N'));

        if (add.StartsWith("j", StringComparison.OrdinalIgnoreCase))
            AddSeatedPlayers(game);
    }

    /// <summary>
    /// Frågar antal (US01), direkt efter det hur många av spelarna som är
    /// datorer (US13), och sedan namn (US01/US11) på de mänskliga.
    /// Sätter alla ner vid bordet.
    /// </summary>
    private void AddSeatedPlayers(Game game)
    {
        // Max 5 spelare vid bordet totalt, inte bara per gång.
        int freeSeats = 5 - SeatedCount(game);
        if (freeSeats == 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("   === BORDET ÄR FULLT – MAX 5 SPELARE ===");
            Console.ResetColor();
            return;
        }

        Print(freeSeats); // US01: antal nya spelare, validerat

        // US13: direkt efter antalet: hur många av dem ska vara datorer?
        // Alternativet visas om det redan sitter en mänsklig spelare vid
        // bordet – då får alla nya spelare vara datorer (en ytterligare
        // människa behövs inte). Första tillägget kräver minst en
        // människa; finns inget val att göra returneras 0 direkt.
        int computers = AskComputerCount(game);
        int humans = NumbersOfPlayers - computers;

        // US13: datorspelarna – strategi + automatiskt namn, direkt ner.
        for (int i = 1; i <= computers; i++)
        {
            IPlayerStrategy strategy = AskStrategy();
            string name = UniqueComputerName(game, i);
            game.CreateAndSeatComputer(name, strategy);
            Console.WriteLine($"   {name} sitter nu vid bordet (dator).");
        }

        // US01/US11: sedan de mänskliga – namn, validerat. humans kan vara
        // 0 om en människa redan sitter vid bordet.
        NamesOfPlayers(humans);
        foreach (string name in players)
        {
            Player? player = game.FindOrCreatePlayer(name);
            if (player == null)
            {
                Console.WriteLine($"   === {name} SITTER REDAN VID BORDET ===");
                continue;
            }
            if (player.Wallet.IsBroke)
            {
                // US15: spelare som gått slut på marker kan inte satsa
                // och får därför inte plats vid bordet.
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"   === {name} HAR SLUT PÅ MARKER OCH KAN INTE SPELA VIDARE ===");
                Console.ResetColor();
                continue;
            }
            game.SeatPlayer(player);
            Console.WriteLine($"   {name} sitter nu vid bordet.");
        }
        players.Clear();
    }

    /// <summary>Frågar antalet nya spelare (1–maxPlayers), validerat.</summary>
    public void Print(int maxPlayers = 5)
    {
        Console.WriteLine(); // tom rad mellan kommandona
        Console.WriteLine($"   1 - {maxPlayers} spelare");
        NumbersOfPlayers = int.Parse(
            ReadUntilValid(
                "Välj antalet spelare: ",
                $" === ANGE ETT NUMMER MELLAN 1 OCH {maxPlayers} ===",
                s => int.TryParse(s, out int n) && n >= 1 && n <= maxPlayers));
    }

    /// <summary>Frågar namnen på count spelare, validerade.</summary>
    public void NamesOfPlayers(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Console.WriteLine(); // tom rad mellan kommandona
            players.Add(ReadUntilValid(
                "Ange namn på spelaren: ",
                " === ANGE ETT NAMN ===",
                s => !string.IsNullOrWhiteSpace(s)).Trim());
        }
    }

    // ================================================================
    // US13: datorspelare – antal, strategi, namn
    // ================================================================

    /// <summary>
    /// Frågar hur många av de nya spelarna som ska vara datorspelare.
    /// Alternativet visas om det redan sitter en mänsklig spelare vid
    /// bordet – då är 0–antal ett giltigt svar (alla kan vara datorer,
    /// ingen ytterligare människa behövs). Första tillägget kräver minst
    /// en människa, så max blir antal minus ett; är max 0 (totalt 1
    /// spelare) finns inget val att göra och metoden returnerar 0 direkt.
    /// </summary>
    public int AskComputerCount(Game game)
    {
        bool humanSeated = game.SeatedPlayers.Any(p => !p.IsComputer);
        int maxComputers = humanSeated ? NumbersOfPlayers : NumbersOfPlayers - 1;

        if (maxComputers <= 0)
            return 0;

        Console.WriteLine(); // tom rad mellan kommandona
        return int.Parse(ReadUntilValid(
            $"   Hur många av dem ska vara datorspelare? (0–{maxComputers}) ",
            $" === ANGE ETT NUMMER MELLAN 0 OCH {maxComputers} ===",
            s => int.TryParse(s, out int n) && n >= 0 && n <= maxComputers));
    }

    /// <summary>Frågar vilken strategi datorspelaren ska ha.</summary>
    public IPlayerStrategy AskStrategy()
    {
        Console.WriteLine(); // tom rad mellan kommandona
        Console.WriteLine("   Vilken strategi ska datorspelaren ha?");
        Console.WriteLine("     1. Enkel (köpa under 17)");
        Console.WriteLine("     2. Grundstrategi (tabell)");

        string choice = ReadUntilValid(
            "   Välj strategi: ",
            " === VÄLJ 1 ELLER 2 ===",
            s => s is "1" or "2");

        return choice == "1" ? new SimpleStrategy() : new BasicStrategy();
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

    // ---------- US11: validering ----------

    /// <summary>
    /// Fråga om tills svaret är giltigt – ingen inmatning får någonsin
    /// krascha programmet eller tyst acceptera fel.
    /// Vid slut på inmatning (EOF – omdirigerad inmatning som tar slut
    /// eller Ctrl+Z) returneras <paramref name="valueOnEof"/> direkt om
    /// det angavs; annars fortsätter frågan.
    /// </summary>
    /// <param name="prompt">Frågan som visas för spelaren.</param>
    /// <param name="errorMessage">Visas om svaret inte är giltigt.</param>
    /// <param name="isValid">Returnerar true om svaret är giltigt.</param>
    /// <param name="valueOnEof">Värde som returneras direkt vid EOF,
    /// t.ex. menynumret för Avsluta. Null = fråga om igen (standard).</param>
    /// <returns>Det första giltiga svaret.</returns>
    public string ReadUntilValid(string prompt, string errorMessage,
        Func<string, bool> isValid, string? valueOnEof = null)
    {
        while (true)
        {
            Console.Write(prompt);
            string? line = Console.ReadLine();
            if (line is null)
            {
                if (valueOnEof is not null)
                    return valueOnEof;
                continue; // EOF – ingen åtgärd angiven, fråga om igen
            }
            if (isValid(line))
                return line;
            Console.WriteLine(errorMessage);
        }
    }

    // ================================================================
    // US08/US11/US12: rundan – livebord, turer och resultat
    // ================================================================

    /// <summary>
    /// Läser in insatsen (US15). En datorspelare sätter sitt eget spel
    /// (ComputerBet) och frågas aldrig. En mänsklig spelare får se
    /// blackjackmattan med sin balans och raden med satsningschips
    /// (1–100), och svarar sedan hur många marker som sätts.
    /// </summary>
    public int ReadBet(Player player)
    {
        if (player.IsComputer)
            return ComputerBet(player);

        // Samma bredd som livebordet – mattan ska kännas likadan.
        int width = FeltWidthFor(_game != null ? SeatedCount(_game) : 1);

        Clear();
        List<List<FeltCell>> rows = new List<List<FeltCell>>();
        rows.Add(LabelRow("", width: width));
        rows.Add(TitlePlateRow(width));
        rows.Add(LabelRow("", width: width));
        rows.Add(LabelRow("VÄLJ DIN INSATS", width, ConsoleColor.Yellow));
        rows.Add(LabelRow($"{player.Name}  ·  Saldo: {player.Wallet.Balance}", width: width));
        rows.Add(LabelRow("", width: width));
        rows.Add(LabelRow("MARKER", width, TableRimColor));
        foreach (List<FeltCell> row in ChipRows(ChipRenderer.StandardValues, width))
            rows.Add(row);
        rows.Add(LabelRow("", width: width));
        DrawFelt(rows, width);

        int bet = int.Parse(
            ReadUntilValid(
                "   Hur många marker vill du satsa?  ",
                "   === Ange en insats över 0 som du har råd med... ===",
                s => int.TryParse(s, out int n) && player.Wallet.CanAfford(n)));
        return bet;
    }

    /// <summary>
    /// Datorspelarens insats: ett slumpat belopp mellan 5 och hela
    /// saldot (har spelaren färre än 5 marker satsas hela saldot).
    /// Beloppet är alltid giltigt: 0 &lt; insats ≤ marker.
    /// </summary>
    public int ComputerBet(Player player)
    {
        int balance = player.Wallet.Balance;
        int minBet = Math.Min(5, balance);
        return _computerBetRandom.Next(minBet, balance + 1);
    }

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
    /// US13: visar en datorspelares beslut – livebordet ritas om med
    /// datorn markerad och beslutet (köper/stannar) skrivs under bordet.
    /// </summary>
    public void ShowComputerMove(Player player, Move move)
    {
        DrawBoard(_game!, activePlayer: player,
            banner: $"{player.Name.ToUpperInvariant()} (DATOR)");

        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine($"   {player.Name} (dator): {(move == Move.Hit ? "köper" : "stannar")}");
        Console.ResetColor();
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
    /// Rundans sista vy: samma speltitel som huvudmenyn överst,
    /// sedan alla händar öppna (även hålkortet) med avgörandet för
    /// varje spelare.
    /// </summary>
    public void ShowRoundEnd(Round round)
    {
        _holeRevealed = true; // hålkortet syns i sista vyn
        Clear();
        DrawTitle("BLACKJACK – TJUGOETT");

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

        // US15: spelare som gick slut på marker under rundan kan inte
        // fortsätta spela – de lämnar bordet.
        foreach (Player p in round.Players)
        {
            if (!p.Wallet.IsBroke)
                continue;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"   {p.Name} är slut i kassan – flest {p.MaxChipsDuringGame} marker under spelet – lämnar bordet.");
            Console.ResetColor();
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
    /// Ritar rundans livebord som en blackjack-matta (grafik i stil
    /// med ett casino-bord): guldplåt "BLACKJACK", givarens kort överst
    /// (hålkortet dolt tills det vänts), och varje spelare i sitt eget
    /// fält med namn och värde överst, kort i mitten och sin insats
    /// (som text och som färgade chips) under korten. Den aktiva
    /// spelaren får en färgad banner och sin rubrik markerad som gul plåt.
    /// </summary>
    private void DrawBoard(Game game, Player? activePlayer = null,
        string? banner = null, ConsoleColor bannerColor = ConsoleColor.Yellow)
    {
        Clear();

        // Mattans bredd anpassas efter antalet säten (FeltWidthFor) så
        // att alla spelare ryms i en enda rad – även fem.
        int width = FeltWidthFor(Math.Max(1, game.SeatedPlayers.Count));
        List<List<FeltCell>> rows = new List<List<FeltCell>>();

        // --- Mattans huvud ---
        rows.Add(LabelRow("", width: width));
        rows.Add(TitlePlateRow(width));
        rows.Add(LabelRow("", width: width));

        // --- Givarens fält (alltid överst) ---
        string dealerLabel = _dealer == null
            ? "GIVARE"
            : _holeRevealed
                ? $"GIVARE  ({_dealer.Hand.GetValue()})"
                : "GIVARE  (hålkortet dolt)";
        rows.Add(LabelRow(dealerLabel, width, ConsoleColor.Yellow));
        if (_dealer != null)
        {
            foreach (List<FeltCell> row in HandRows(_dealer.Hand.Cards, !_holeRevealed, width))
                rows.Add(row);
        }

        rows.Add(LabelRow("", width: width));
        rows.Add(SeparatorRow(width));

        // --- Spelarnas säten – alla i en rad, sida vid sida ---
        DrawSeatRow(game.SeatedPlayers, activePlayer, banner, bannerColor, width, rows);
        rows.Add(LabelRow("", width: width));

        DrawFelt(rows, width);
    }

    /// <summary>
    /// Alla säten i en rad sida vid sida. Varje säte är lika brett
    /// (mattans bredd minus mellanrum, fördelat jämnt) och lika högt:
    /// banner (eller tomt), namnrad, kort, insatsrad och insatschips.
    /// </summary>
    private static void DrawSeatRow(IReadOnlyList<Player> players,
        Player? activePlayer, string? banner, ConsoleColor bannerColor,
        int width, List<List<FeltCell>> rows)
    {
        int count = players.Count;
        if (count == 0)
            return;

        // Minst ett tecken per säte, så att en extremt smal konsolruta
        // inte ger negativa bredder nedströms.
        int seatWidth = Math.Max(1, (width - 2 * (count - 1)) / count);

        List<List<List<FeltCell>>> blocks = new List<List<List<FeltCell>>>();
        for (int i = 0; i < count; i++)
            blocks.Add(SeatBlock(players[i], ReferenceEquals(players[i], activePlayer),
                seatWidth, banner, bannerColor));

        int maxRows = 0;
        foreach (List<List<FeltCell>> block in blocks)
            maxRows = Math.Max(maxRows, block.Count);

        for (int r = 0; r < maxRows; r++)
        {
            List<FeltCell> joined = new List<FeltCell>();
            for (int s = 0; s < count; s++)
            {
                if (s > 0)
                    joined.Add(Felt("  "));
                // Korta block fylls med en tom rad i sätets bredd – annars
                // skulle raden bli smalare och CenterRow flytta sätena.
                joined.AddRange(r < blocks[s].Count ? blocks[s][r] : LabelRow("", width: seatWidth));
            }
            rows.Add(CenterRow(joined, width)); // fyller ut till mattans bredd

            // Skiljelinje under namnrader (banner + namn = rader 0–1).
            if (r == 1)
                rows.Add(SeparatorRow(width));
        }
    }

    /// <summary>
    /// Ett säte: rad 0 = turen-bannern (eller tomt), rad 1 = namn + värde,
    /// sedan korten, en insatsrad (tydlig brytning mellan kort och chips)
    /// och spelarens insats som chips. Varje rad är exakt
    /// <paramref name="width"/> tecken bred.
    /// </summary>
    private static List<List<FeltCell>> SeatBlock(Player p, bool active, int width,
        string? banner, ConsoleColor bannerColor)
    {
        List<List<FeltCell>> rows = new List<List<FeltCell>>();

        // Turen-bannern – bara den aktiva spelaren får den; övriga säten
        // får en tom rad så att alla säten hamnar på samma höjd.
        rows.Add(active && banner != null
            ? [new FeltCell($"   {banner}".PadRight(width), bannerColor, ConsoleColor.Black)]
            : LabelRow("", width: width));

        // Namn + värde (insatsen visas under korten i stället); ► och gul
        // bakgrund när det är hens tur.
        string label = $"{p.Name}  ({p.Hand.GetValue()})";
        if (p.IsBusted)
            label += "  – SPRÄCKT!";

        string text = (active ? "► " : "") + label;
        if (text.Length > width)
            text = text[..width];

        rows.Add([new FeltCell(
            text.PadRight(width),
            active ? ConsoleColor.Yellow : (ConsoleColor?)null,
            active ? ConsoleColor.Black : (p.IsBusted ? ConsoleColor.Red : (ConsoleColor?)null))]);

        // Korten – 7 rader per kortrad (en tom hand ger tomma rader).
        if (p.Hand.Count > 0)
            rows.AddRange(HandRows(p.Hand.Cards, false, width));
        else
            for (int i = 0; i < 7; i++)
                rows.Add(LabelRow("", width: width));

        // Tydlig brytning mellan korten och markerna: insatsen som text
        // (eller en tom rad om spelaren ännu inte har bett).
        rows.Add(p.Wallet.CurrentBet > 0
            ? LabelRow($"insats {p.Wallet.CurrentBet}", width, ConsoleColor.DarkCyan)
            : LabelRow("", width: width));

        // Insatsen som chips framför spelaren (eller en tom rad).
        if (p.Wallet.CurrentBet > 0)
            rows.AddRange(BetChipRows(p.Wallet.CurrentBet, width: width));
        else
            rows.Add(LabelRow("", width: width));

        return rows;
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

    /// <summary>US15: meddelande för varje spelare som fått slut på marker
    /// och lämnat bordet.</summary>
    public void ShowEliminated(List<Player> eliminated)
    {
        if (eliminated.Count == 0)
            return;

        Console.ForegroundColor = ConsoleColor.Yellow;
        foreach (Player p in eliminated)
            Console.WriteLine($" === {p.Name} har inga fler marker och lämnar bordet ===");
        Console.ResetColor();
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