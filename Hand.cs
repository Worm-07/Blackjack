/*
 ***US07 + US08 + US09: Hand – värde, mjuk/hård, spräckt, naturlig 21***
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

    /// Tömmer handen – används när en ny runda startar.
    public void Clear()
    {
        _cards.Clear();
    }

    /// Handens totala värde: 2–10 sitt eget värde, klätt = 10,
    /// ess = 1 eller 11 – det som är bäst utan att spräcka.
    public int GetValue()
    {
        (int value, _) = ComputeValue();
        return value;
    }

    /// Mjuk hand: minst ett ess räknas som 11 i det aktuella värdet.
    /// Exempel: A + 6 är en mjuk 17. Drar man 9 blir det A + 6 + 9 = hård 16.
    public bool IsSoft
    {
        get
        {
            // Mjuk om minst ett ess fortfarande räknas som 11 i värdet.
            (_, int softAces) = ComputeValue();
            return softAces > 0;
        }
    }

    ///Spräckt: handens värde över 21.
    public bool IsBusted
    {
        get
        {
            return GetValue() > 21;
        }
    }

    /// Naturlig tjugoett: exakt två kort som tillsammans blir 21
    /// (ess + tiokort/klätt).
    public bool IsNatural
    {
        get
        {
            return Count == 2 && GetValue() == 21;
        }
    }

    /// Basvärde för ett kort: 2–10 sitt värde, klätt = 10, ess = 11.
    /// Esset justeras av GetValue/IsSoft – Card håller bara färg + valör.
    private static int GetCardValue(Card card)
    {
        return card.Rank switch
        {
            Rank.Jack or Rank.Queen or Rank.King => 10,
            Rank.Ace => 11,
            _ => (int)card.Rank // 2–10 har sitt eget värde i enumen
        };
    }
    
    /// Räknar ut handens värde. Returnerar värdet och hur många ess som
    /// fortfarande räknas som 11. Summerar först med alla ess som 11 och
    /// sänker ess till 1 (−10) så länge summan överstiger 21.
    private (int Value, int SoftAces) ComputeValue()
    {
        int value = 0;
        int aces = 0;
        foreach (Card card in _cards)
        {
            value += GetCardValue(card);
            if (card.Rank == Rank.Ace)
                aces++;
        }

        while (value > 21 && aces > 0)
        {
            value -= 10; // ess 11 → 1
            aces--;
        }

        return (value, aces);
    }
}
