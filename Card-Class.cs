using System.Collections.Generic;

/*

***Card-class created by Daniel Norberg***

US04: Kort med färg och valör

*/

/// De fyra färgerna i en standardkortlek.

public enum Suit
{
    Hearts,   // Hjärter
    Diamonds, // Ruter
    Clubs,    // Klöver
    Spades    // Spader
}


/// Kortens valörer, från två (2) till ess.
/// Numeriska värden (2–14) underlättar jämförelser i spellogik.


public enum Rank
{
    Two = 2,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Jack,
    Queen,
    King,
    Ace
}

/// Ett enskilt kort med en färg (Suit) och en valör (Rank).
public class Card
{
    public Suit Suit { get; }
    public Rank Rank { get; }

    public Card(Suit suit, Rank rank)
    {
        Suit = suit;
        Rank = rank;
    }

    public override string ToString()
    {
        return $"{RankToString()} av {SuitToString()}";
    }

    private string RankToString()
    {
        return Rank switch
        {
            Rank.Jack => "Knekt",
            Rank.Queen => "Dama",
            Rank.King => "Kung",
            Rank.Ace => "Ess",
            _ => ((int)Rank).ToString()
        };
    }

    private string SuitToString()
    {
        return Suit switch
        {
            Suit.Hearts => "hjärter",
            Suit.Diamonds => "ruter",
            Suit.Clubs => "klöver",
            Suit.Spades => "spader",
            _ => Suit.ToString().ToLower()
        };
    }
}

/// En standardkortlek med 52 kort (4 färger × 13 valörer).
public class Deck
{
    public List<Card> Cards { get; }

    public Deck()
    {
        Cards = new List<Card>(52);
        foreach (Suit suit in Enum.GetValues<Suit>())
        {
            foreach (Rank rank in Enum.GetValues<Rank>())
            {
                Cards.Add(new Card(suit, rank));
            }
        }
    }
}
