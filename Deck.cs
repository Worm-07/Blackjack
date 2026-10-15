/*
 ***US05 + US06: Deck – bygg & blanda leken, byt lek när den tar slut***
 ***Strukturkrav: ingen Console i regelklasser (US16).***
 */

/// En kortlek med 52 kort. Bygger leken, blandar (Fisher-Yates),
/// delar ut och håller koll på hur många kort som är kvar.
public class Deck
{
    /// US06: färre än 15 kort kvar → ny blandad lek inför nästa runda.
    /// Bytet sker aldrig mitt i en runda.
    public const int ReshuffleThreshold = 15;

    private readonly List<Card> _cards = new();
    private readonly Random _random;

    /// <summary>
    /// Random delas av hela programmet (skapas en gång i Program.cs).
    /// Skapar man en ny Random vid varje blandning får man samma lek i
    /// on och om igen, eftersom den såddas från klockan.
    /// </summary>
    public Deck(Random random)
    {
        _random = random;
        BuildDeck();
        Shuffle();
    }

    /// Hur många kort som är kvar i leken.
    public int RemainingCards => _cards.Count;

    public bool IsEmpty => _cards.Count == 0;

    /// Bygger en ny lek med 52 kort (4 färger × 13 valörer).
    private void BuildDeck()
    {
        _cards.Clear();
        foreach (Suit suit in Enum.GetValues<Suit>())
        {
            foreach (Rank rank in Enum.GetValues<Rank>())
            {
                _cards.Add(new Card(suit, rank));
            }
        }
    }

    /// <summary>
    /// Fisher-Yates: gå baklänges genom leken och byt plats på varje kort
    /// med ett slumpmässigt valt kort bland dem som ligger före.
    /// </summary>
    public void Shuffle()
    {
        for (int i = _cards.Count - 1; i > 0; i--)
        {
            int j = _random.Next(i + 1);
            (_cards[i], _cards[j]) = (_cards[j], _cards[i]);
        }
    }

    /// <summary>Tar bort och returnerar nästa kort i leken.</summary>
    public Card Deal()
    {
                if (IsEmpty)
        {
            // Leken tog slut mitt i en runda: ta fram en ny, blandad lek.
            BuildDeck();
            Shuffle();
        }

        Card card = _cards[^1];
        _cards.RemoveAt(_cards.Count - 1);
        return card;
    }

    /// <summary>
    /// Anropas inför varje ny runda: blandas om om färre än
    /// ReshuffleThreshold (15) kort är kvar.
    /// </summary>
    public void ReshuffleIfLow()
    {
        if (RemainingCards < ReshuffleThreshold)
        {
            BuildDeck();
            Shuffle();
        }
    }
}
