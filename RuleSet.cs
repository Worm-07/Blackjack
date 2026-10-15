/*
 ***Tillval 3 (US14): Husregler från konfigurationsfil***
 ***Skelett – metoden är markerad med TODO och ska implementeras.***
 */

/// Regelinställningar som läses in från en textfil vid start och skickas
/// in (injekteras) i Dealer, Round och Deck där behoven finns.
/// Gör reglerna inställbara utan omkompilering.
public class RuleSet
{
    /// Konfigurationsfilen ligger i programkatalogen.
    public const string FileName = "rules.txt";

    /// Drar givaren på mjuk 17, eller stannar hen? (grund: stannar)
    public bool DealerHitsOnSoftSeventeen { get; init; } = false;

    /// Hur många lekar spelas det med? (grund: 1)
    public int NumberOfDecks { get; init; } = 1;

    /// Vid hur många kvarvarande kort blandas det om? (grund: 15)
    public int ReshuffleThreshold { get; init; } = 15;

    /// Vinner naturlig tjugoett över en 21 byggd av tre kort eller fler? (grund: ja)
    public bool NaturalBeatsThreeCardTwentyOne { get; init; } = true;

    /// <summary>
    /// Läser in rules.txt (format: "Nyckel=Värde" per rad).
    /// Saknas filen → standardvärden. Programmet ska inte krascha –
    /// meddelandet "standardvärden används" visas av ConsoleUi, inte här.
    /// </summary>
    public static RuleSet LoadFromFile()
    {
        // TODO Tillval 3:
        // if (!File.Exists(FileName)) return new RuleSet();
        // Annars parsea rad för rad och sätta de kända nycklarna.
        // Tips: returnera även en flagga/meddelande om standardvärden användes.
        throw new NotImplementedException();
    }
}
