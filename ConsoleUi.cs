class Consoleui
{
    public List<string> players = [];
    public int NumbersOfPlayers;
    public void Print()
    {
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