/*
 ***Tillval 1 (US12): Kortgrafik i konsolen***
 ***Skelett – metoderna är markerade med TODO och ska implementeras.***
 */

/// Ritar kort som rutor i konsolen.
///
/// Ansvar: form ENDA. Renderaren returnerar rader som string[] så att
/// ConsoleUi kan fläta ihop dem och sätta färgen (Console.ForegroundColor
/// hör hemma i ConsoleUi – hjärter/ruter röda, klöver/spader svarta).
///
/// Test för grundkoden: om ingen regelklass behöver ändras när
/// presentationen byts ut var ansvarsfördelningen rätt. Måste du ändra
/// i Hand eller Round för att byta utseende har ni hittat en läckage –
/// skriv om det i rapporten.
public static class CardRenderer
{
    /// <summary>
    /// Ritar ett enskilt kort som rader.
    /// Exempel:
    ///   ┌─────┐
    ///   │  A  │
    ///   │ ♥   │
    ///   │   A │
    ///   └─────┘
    /// Valören i två hörn, färgsymbolen i mitten.
    /// Klädda kort: använd schackglyfer (t.ex. ♞ för knekt) i stället
    /// för försök att teckna ansikten.
    /// </summary>
    public static string[] RenderCard(Card card)
    {
        // TODO Tillval 1: bygg 5 rader från card.Rank och card.Suit
        throw new NotImplementedException();
    }

    /// <summary>Ritar ett kort med baksidan upp (givarens hålkort).</summary>
    public static string[] RenderFaceDown()
    {
        // TODO Tillval 1: ruta med mönster, t.ex. schackglyfer i mitten
        throw new NotImplementedException();
    }

    /// <summary>
    /// Ritar en hel hand: korten bredvid varandra, inte under varandra.
    /// Teknisk knut: bygg rad 1 för alla kort, rad 2 för alla kort, osv.
    /// </summary>
    public static string[] RenderHand(IReadOnlyList<Card> cards)
    {
        // TODO Tillval 1: häng ihop RenderCard:s rader, kort för kort
        throw new NotImplementedException();
    }
}
