/*
 ***Tillval 5 (US16): Korträkning enligt Hi-Lo-systemet***
 ***Skelett – metoderna är markerade med TODO och ska implementeras.***
 */

/// En korträknare som följer med i spelet.
/// Räknevärde: 2–6 → +1, 7–9 → 0, 10/klätt/ess → −1.
///
/// Räknaren är en REN OBSERVATÖR: den får inte ändra något i spelet.
/// Går det inte att bygga utan att röra reglerna har ni hittat ett
/// problem – värt att skriva om i rapporten.
public class HiLoCounter
{
    public int DecksInShoe { get; }

    /// Den löpande räkningen: summan av räknevärdena för alla sedda kort.
    public int RunningCount { get; private set; }

    /// Antal kvarvarande lekar (behövs för den sanna räkningen).
    public int RemainingDecks { get; private set; }

    public HiLoCounter(int decksInShoe)
    {
        DecksInShoe = decksInShoe;
        RemainingDecks = decksInShoe;
    }

    /// Anropa varje gång ett kort blir synligt (spelarkort, givarens
    /// öppna kort, vänt hålkort, köpta kort).
    public void CardSeen(Card card)
    {
        // TODO Tillval 5:
        // RunningCount += card.Rank switch
        // {
        //     >= Rank.Two && <= Rank.Six => 1,
        //     >= Rank.Seven && <= Rank.Nine => 0,
        //     _ => -1 // Ten, Jack, Queen, King, Ace
        // };
        // (uppdatera även RemainingDecks när korten tas ur skon)
        throw new NotImplementedException();
    }

    /// Den sanna räkningen: den löpande räkningen delad med antalet
    /// kvarvarande lekar.
    public double TrueCount
    {
        get
        {
            // TODO Tillval 5: return RemainingDecks == 0 ? 0 : (double)RunningCount / RemainingDecks;
            throw new NotImplementedException();
        }
    }

    /// Hur gynnsamt läget är för spelarna, t.ex. "Låg / Neutral / Hög"
    /// baserat på den sanna räkningen.
    public string AdvantageIndicator
    {
        get
        {
            // TODO Tillval 5: t.ex. TrueCount < 0 → "Låg", 0–1 → "Neutral", > 1 → "Hög"
            throw new NotImplementedException();
        }
    }
}
