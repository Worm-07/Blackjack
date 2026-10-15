/*
 ***Tillval 2 (US13): Grundstrategi***
 */

/// Grundstrategi: slå upp rätt drag i en tabell utifrån handens värde,
/// om den är mjuk eller hård, och vilket kort givaren visar.
/// Tabellen får tas från en publicerad grundstrategi – men du ska kunna
/// förklara hur din kod läser den.
public class BasicStrategy : IPlayerStrategy
{
    // TODO Tillval 2: tabell, t.ex. (handvärde, mjuk/hård, givarens kort) → Move
    // public static readonly (int HandValue, bool Soft, int DealerUpCard, Move Move)[] Table =
    // {
    //     // mjuka händar 13–17 …
    //     // hårda händer 8–21 …
    // };

    public Move Decide(Player player, Card dealerUpCard)
    {
        // TODO Tillval 2: slå upp (värde, mjuk/hård, givarens kort) i tabellen
        throw new NotImplementedException();
    }
}
