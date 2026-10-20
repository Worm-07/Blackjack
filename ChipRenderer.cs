/*
 ***Tillval 1 (US12): ChipRenderer – satsningschips som textrutor***
 ***Samma uppdelning som i CardRenderer: formen här, färgen i ConsoleUi.***
 */

/// Ritar satsningschips (marker) som textrutor, 9 tecken breda och
/// 5 rader höga. Returnerar raderna som string[] och skriver aldrig
/// ut något själv. Chipens grundfärg (vit 1, röd 5, ...) sätts i
/// ConsoleUi, inte här.
public static class ChipRenderer
{
    // De vanliga chipvärdena som erbjuds på insatsskärmen.
    public static readonly int[] StandardValues = [1, 5, 10, 25, 50, 100];

    // Mellanrummet mellan chipen i en rad. Publik så att ConsoleUi
    // kan använda samma.
    public const string ChipSpacing = "  ";

    // Bredden innanför ramen. Hela chippet blir 7 + 2 = 9 tecken.
    private const int InnerWidth = 7;

    public static string[] RenderChip(int value)
    {
        string v = value.ToString();

        return
        [
            TopBorder(),
            EdgeRow(v),
            Row(Center(v)),
            EdgeRow(v),
            BottomBorder()
        ];
    }

    // Ritar varje chip i raden för sig, bredvid varandra. ConsoleUi
    // använder det för att färga chipen olika.
    public static List<string[]> RenderChipRow(IReadOnlyList<int> values)
    {
        List<string[]> chips = new List<string[]>();

        foreach (int value in values)
        {
            chips.Add(RenderChip(value));
        }

        return chips;
    }

    // --- Ramen, gemensam för alla chips ---

    private static string TopBorder() => "╭" + new string('─', InnerWidth) + "╮";

    private static string BottomBorder() => "╰" + new string('─', InnerWidth) + "╯";

    private static string Row(string content) => "│" + content + "│";

    // Kantraden: en kantremsa (▌/▐) på varje sida – som chips rand –
    // med värdet centrerat mellan dem.
    private static string EdgeRow(string value)
    {
        int room = InnerWidth - 2 - value.Length;
        int left = room / 2;
        int right = room - left;
        return "│▌" + new string(' ', left) + value + new string(' ', right) + "▐│";
    }

    // Placerar värdet i mitten av det inre fältet.
    private static string Center(string text)
    {
        int left = (InnerWidth - text.Length) / 2;
        int right = InnerWidth - left - text.Length;
        return new string(' ', left) + text + new string(' ', right);
    }
}