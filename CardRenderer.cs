/// Ritar kort som textrutor, 9 tecken breda och 7 rader höga.
/// Returnerar raderna som string[] och skriver aldrig ut något själv.
/// Färgen (röd/svart) sätts i ConsoleUi, inte här.
public static class CardRenderer
{
    // Bredden innanför ramen. Hela kortet blir 7 + 2 = 9 tecken.
    private const int InnerWidth = 7;

    // Mönstret på kortets baksida.
    private const char BackPattern = '▒';

    public static string[] RenderCard(Card card)
    {
        string rank = RankLabel(card.Rank);
        string middle = MiddleSymbol(card);

        return
        [
            TopBorder(),
            Row(rank.PadRight(InnerWidth)),
            EmptyRow(),
            Row(Center(middle)),
            EmptyRow(),
            Row(rank.PadLeft(InnerWidth)),
            BottomBorder()
        ];
    }

    // Givarens hålkort: samma ram, men baksidan upp.
    public static string[] RenderFaceDown()
    {
        string back = new string(BackPattern, InnerWidth);

        return
        [
            TopBorder(),
            Row(back),
            Row(back),
            Row(back),
            Row(back),
            Row(back),
            BottomBorder()
        ];
    }

    // --- Ramen, gemensam för alla kort ---

    private static string TopBorder() => "┌" + new string('─', InnerWidth) + "┐";

    private static string BottomBorder() => "└" + new string('─', InnerWidth) + "┘";

    private static string Row(string content) => "│" + content + "│";

    private static string EmptyRow() => Row(new string(' ', InnerWidth));

    // --- Innehållet på kortets framsida ---

    // Texten i hörnen. Svenska bokstäver, eftersom användaren ser dem.
    private static string RankLabel(Rank rank)
    {
        return rank switch
        {
            Rank.Ace => "A",
            Rank.Jack => "Kn",
            Rank.Queen => "D",
            Rank.King => "K",
            _ => ((int)rank).ToString()
        };
    }

    private static string SuitSymbol(Suit suit)
    {
        return suit switch
        {
            Suit.Hearts => "♥",
            Suit.Diamonds => "♦",
            Suit.Clubs => "♣",
            Suit.Spades => "♠",
            _ => "?"
        };
    }

    // Klädda kort får en schackglyf bredvid färgen, övriga bara färgen.
    private static string MiddleSymbol(Card card)
    {
        string suit = SuitSymbol(card.Suit);

        return card.Rank switch
        {
            Rank.Jack => "♞ " + suit,
            Rank.Queen => "♛ " + suit,
            Rank.King => "♚ " + suit,
            _ => suit
        };
    }

    // Placerar texten i mitten av det inre fältet.
    private static string Center(string text)
    {
        int left = (InnerWidth - text.Length) / 2;
        int right = InnerWidth - left - text.Length;
        return new string(' ', left) + text + new string(' ', right);
    }
}