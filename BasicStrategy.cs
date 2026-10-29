/*
 ***Tillval 2 (US13): Grundstrategi***
 */

/// Grundstrategi: slå upp rätt drag i en tabell utifrån handens värde,
/// om den är mjuk eller hård, och vilket kort givaren visar.
/// Tabellen är en publicerad grundstrategi (S17 – givaren stannar på
/// alla 17:or; spelet har bara köpa/stanna, så double/split saknas).
public class BasicStrategy : IPlayerStrategy
{
    // Rader: handens värde 4–21 (index = värde − 4).
    // Kolumner: givarens öppna kort 2,3,4,5,6,7,8,9,10,A
    //           (index = värde − 2, A = 11).
    // H = köpa (Hit), S = stanna (Stand).
    private static readonly string[] HardTable =
    {
        "HHHHHHHHHH", //  4
        "HHHHHHHHHH", //  5
        "HHHHHHHHHH", //  6
        "HHHHHHHHHH", //  7
        "HHHHHHHHHH", //  8
        "HHHHHHHHHH", //  9
        "HHHHHHHHHH", // 10
        "HHHHHHHHHH", // 11
        "HHSSSHHHHH", // 12
        "SSSSSHHHHH", // 13
        "SSSSSHHHHH", // 14
        "SSSSSHHHHH", // 15
        "SSSSSHHHHH", // 16
        "SSSSSSSSSS", // 17
        "SSSSSSSSSS", // 18
        "SSSSSSSSSS", // 19
        "SSSSSSSSSS", // 20
        "SSSSSSSSSS", // 21
    };

    private static readonly string[] SoftTable =
    {
        "HHHHHHHHHH", //  4 (kommer inte före – mjuka händer är minst 13)
        "HHHHHHHHHH", //  5
        "HHHHHHHHHH", //  6
        "HHHHHHHHHH", //  7
        "HHHHHHHHHH", //  8
        "HHHHHHHHHH", //  9
        "HHHHHHHHHH", // 10
        "HHHHHHHHHH", // 11
        "HHHHHHHHHH", // 12
        "HHHHHHHHHH", // 13
        "HHHHHHHHHH", // 14
        "HHHHHHHHHH", // 15
        "HHHHHHHHHH", // 16
        "HHHHHHHHHH", // 17
        "SSSSSSHHHH", // 18
        "SSSSSSSSSS", // 19
        "SSSSSSSSSS", // 20
        "SSSSSSSSSS", // 21
    };

    public Move Decide(Hand hand, Card dealerUpCard)
    {
        int value = hand.GetValue();
        int dealer = dealerUpCard.Rank switch
        {
            Rank.Ace => 11,
            Rank.Jack or Rank.Queen or Rank.King => 10,
            _ => (int)dealerUpCard.Rank // 2–10
        };

        string[] table = hand.IsSoft ? SoftTable : HardTable;

        // Försvarskontroll: handen är i praktiken alltid 4–21 när Decide
        // anropas (spräckta händer hanteras av Round innan anropet).
        if (value < 4 || value > 21)
            return Move.Stand;

        return table[value - 4][dealer - 2] == 'H' ? Move.Hit : Move.Stand;
    }
}
