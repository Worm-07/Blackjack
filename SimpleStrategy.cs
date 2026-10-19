/*
 ***Tillval 2 (US13): Enkel strategi***
 */

/// Enkel strategi: köpa så länge handens värde är under 17, annars stanna.
public class SimpleStrategy : IPlayerStrategy
{
    public Move Decide(Hand hand, Card dealerUpCard)
    {
        // Köpa så länge handens värde är under 17, annars stanna.
        return hand.GetValue() < 17 ? Move.Hit : Move.Stand;
    }
}
