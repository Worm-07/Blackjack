/*
 ***Tillval 2 (US13): Enkel strategi***
 */

/// Enkel strategi: köpa så länge handens värde är under 17, annars stanna.
public class SimpleStrategy : IPlayerStrategy
{
    public Move Decide(Player player, Card dealerUpCard)
    {
        // TODO Tillval 2 (kräver US06/US07: Player.Hand):
        // return player.Hand.GetValue() < 17 ? Move.Hit : Move.Stand;
        throw new NotImplementedException();
    }
}
