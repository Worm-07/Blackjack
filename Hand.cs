/*
 ***US06: Hand – en samling kort som kan räkna ut sitt eget värde***
 ***Skelett – metoderna är markerade med TODO och ska implementeras.***
 */

/// En hand (en samling kort). Beräknar sitt eget värde och vet om den är
/// mjuk, spräckt eller naturlig tjugoett.
///
/// Viktigt: värdet beräknas varje gång det frågas och sparas INTE.
/// Esset byter värde (1 ↔ 11) under handens gång, så ett sparat värde
/// hade snabbt blivit fel.
public class Hand
{
    private readonly List<Card> _cards = new();

    /// De kort som ingår, i ordning.
    public IReadOnlyList<Card> Cards => _cards;

    public int Count => _cards.Count;

    public void AddCard(Card card)
    {
        _cards.Add(card);
    }

    /// <summary>
    /// Handens totala värde: 2–10 sitt eget värde, klätt = 10,
    /// ess = 1 eller 11 – det som är bäst utan att spräcka.
    /// </summary>
    public int GetValue()
    {
        // TODO US06:
        // 1. Summera alla kort med GetCardValue (ess som 11).
        // 2. Så länge summan > 21 och det finns ess: dra ifrån 10 (ess 11 → 1).
        throw new NotImplementedException();
    }

    /// <summary>
    /// Mjuk hand: minst ett ess räknas som 11 i det aktuella värdet.
    /// Exempel: A + 6 är en mjuk 17. Drar man 9 blir det A + 6 + 9 = hård 16.
    /// </summary>
    public bool IsSoft
    {
        get
        {
            // TODO US06: summera med ess som 11; om summan > 21 måste minst
            // ett ess sänkas till 1 → hård hand, annars mjuk.
            throw new NotImplementedException();
        }
    }

    /// <summary>Spräckt: handens värde över 21.</summary>
    public bool IsBusted
    {
        get
        {
            // TODO US06: return GetValue() > 21;
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Naturlig tjugoett: exakt två kort som tillsammans blir 21
    /// (ess + tiokort/klätt).
    /// </summary>
    public bool IsNatural
    {
        get
        {
            // TODO US06: return Count == 2 && GetValue() == 21;
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Basvärde för ett kort: 2–10 sitt värde, klätt = 10, ess = 11.
    /// Esset justeras av GetValue/IsSoft – Card håller bara färg + valör.
    /// </summary>
    private static int GetCardValue(Card card)
    {
        // TODO US06: 2–10 → sitt värde, Jack/Queen/King → 10, Ace → 11
        throw new NotImplementedException();
    }
}
