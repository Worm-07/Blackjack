/*
 ***Tillval 3 (US14): Husregler från konfigurationsfil***
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
    
    /// Meddelanden från inläsningen (saknad fil, okända nycklar, felaktiga värden).
    /// Tom lista betyder att allt lästes in utan problem.
    public IReadOnlyList<string> Messages { get; init; } = new List<string>();

    /// Gränser så att ett galet värde i filen inte förstör spelet.
    public const int MinDecks = 1;
    public const int MaxDecks = 8;
    public const int MinThreshold = 5;

    /// Läser in husreglerna. Format: en regel per rad, "Nyckel=Värde".
    /// Tomma rader och rader som börjar med # ignoreras.
    /// Saknas filen, eller är en rad trasig, används standardvärdet och
    /// ett meddelande läggs i Messages. Metoden kraschar aldrig.
    public static RuleSet LoadFromFile(string path = FileName)
    {
        RuleSet defaults = new RuleSet();
        List<string> messages = new List<string>();

        if (!File.Exists(path))
        {
            messages.Add($"Filen {path} saknas. Standardregler används.");
            return new RuleSet { Messages = messages };
        }

        string[] lines;
        try
        {
            lines = File.ReadAllLines(path);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            messages.Add($"Kunde inte läsa {path}. Standardregler används.");
            return new RuleSet { Messages = messages };
        }

        // Börja från standardvärdena och skriv över det som finns i filen.
        bool hitsSoft17 = defaults.DealerHitsOnSoftSeventeen;
        int decks = defaults.NumberOfDecks;
        int threshold = defaults.ReshuffleThreshold;
        bool naturalBeats = defaults.NaturalBeatsThreeCardTwentyOne;

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            int lineNumber = i + 1;

            if (line.Length == 0 || line.StartsWith('#'))
                continue;

            string[] parts = line.Split('=', 2);
            if (parts.Length != 2)
            {
                messages.Add($"Rad {lineNumber}: saknar '=' och ignoreras.");
                continue;
            }

            string key = parts[0].Trim();
            string value = parts[1].Trim();

            switch (key)
            {
                case nameof(DealerHitsOnSoftSeventeen):
                    if (!TryParseBool(value, out hitsSoft17))
                    {
                        hitsSoft17 = defaults.DealerHitsOnSoftSeventeen;
                        messages.Add($"Rad {lineNumber}: {key} måste vara ja eller nej.");
                    }
                    break;

                case nameof(NumberOfDecks):
                    if (!int.TryParse(value, out decks) || decks < MinDecks || decks > MaxDecks)
                    {
                        decks = defaults.NumberOfDecks;
                        messages.Add($"Rad {lineNumber}: {key} måste vara {MinDecks} till {MaxDecks}.");
                    }
                    break;

                case nameof(ReshuffleThreshold):
                    if (!int.TryParse(value, out threshold) || threshold < MinThreshold)
                    {
                        threshold = defaults.ReshuffleThreshold;
                        messages.Add($"Rad {lineNumber}: {key} måste vara minst {MinThreshold}.");
                    }
                    break;

                case nameof(NaturalBeatsThreeCardTwentyOne):
                    if (!TryParseBool(value, out naturalBeats))
                    {
                        naturalBeats = defaults.NaturalBeatsThreeCardTwentyOne;
                        messages.Add($"Rad {lineNumber}: {key} måste vara ja eller nej.");
                    }
                    break;

                default:
                    messages.Add($"Rad {lineNumber}: okänd regel '{key}' ignoreras.");
                    break;
            }
        }

        // Tröskeln får inte vara så hög att skon blandas om före varje runda.
        if (threshold >= decks * 52)
        {
            messages.Add($"{nameof(ReshuffleThreshold)} är för hög för {decks} lek(ar). Standard används.");
            threshold = defaults.ReshuffleThreshold;
        }

        return new RuleSet
        {
            DealerHitsOnSoftSeventeen = hitsSoft17,
            NumberOfDecks = decks,
            ReshuffleThreshold = threshold,
            NaturalBeatsThreeCardTwentyOne = naturalBeats,
            Messages = messages
        };
    }

    /// Tolkar ja/nej (och true/false) utan hänsyn till versaler.
    private static bool TryParseBool(string text, out bool result)
    {
        switch (text.ToLowerInvariant())
        {
            case "ja":
            case "true":
                result = true;
                return true;
            case "nej":
            case "false":
                result = false;
                return true;
            default:
                result = false;
                return false;
        }
    }
}