/// Ritar kort som textrutor, 9 tecken breda och 7 rader höga.
/// Returnerar raderna som string[] och skriver aldrig ut något själv.
/// Färgen (röd/svart) sätts i ConsoleUi, inte här.
public static class CardRenderer
{
    // Bredden innanför ramen. Hela kortet blir 7 + 2 = 9 tecken.
    private const int InnerWidth = 7;

    // Antal rader i ett kort.
    private const int CardHeight = 7;

    // Mönstret på kortets baksida.
    private const char BackPattern = '▒';

    // Mellanrummet mellan korten i en hand.
    private const string CardSpacing = " ";

    // Givarens hålkort är det andra kortet som delas ut (index 1).
    private const int HoleCardIndex = 1;

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

    // Ritar en hel hand med korten bredvid varandra.
    // Med hideHoleCard = true ritas givarens hålkort med baksidan upp.
    public static string[] RenderHand(IReadOnlyList<Card> cards, bool hideHoleCard = false)
    {
        List<string[]> renderedCards = new List<string[]>();

        for (int i = 0; i < cards.Count; i++)
        {
            bool isHidden = hideHoleCard && i == HoleCardIndex;
            renderedCards.Add(isHidden ? RenderFaceDown() : RenderCard(cards[i]));
        }

        return JoinSideBySide(renderedCards);
    }

    // Bygger rad 0 för alla kort, sedan rad 1 för alla kort, och så vidare.
    private static string[] JoinSideBySide(List<string[]> renderedCards)
    {
        string[] lines = new string[CardHeight];

        for (int row = 0; row < CardHeight; row++)
        {
            string[] parts = new string[renderedCards.Count];

            for (int i = 0; i < renderedCards.Count; i++)
            {
                parts[i] = renderedCards[i][row];
            }

            lines[row] = string.Join(CardSpacing, parts);
        }

        return lines;
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