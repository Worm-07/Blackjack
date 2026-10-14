/*
 ***US10: Participant – gemensam bas för Player och Dealer***
 ***Skelett – implementera TODO:erna.***
 */

/// Gemensam bas: både spelare och givare har ett namn och en hand,
/// och kan ta emot kort.
/// Designfråga (US10): givaren är "en delvis spelare" – de delar namn, hand
/// och att de kan spräcka. Inte delat: beslutet (givaren har en fast
/// regel) och hålkortet.
/// Strukturkrav: ingen in-/utmatering här.
public class Participant
{
    public string Name { get; }

    public Hand Hand { get; }

    protected Participant(string name)
    {
        Name = name;
        Hand = new Hand();
    }

    /// Lägg till ett kort i handen.
    public void AddCard(Card card)
    {
        Hand.AddCard(card);
    }

    /// Har handen över 21? (US09)
    public bool IsBusted => Hand.IsBusted;
}
