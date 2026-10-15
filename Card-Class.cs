using System;
using System.Collections.Generic;

/*
***US04: Kort med färg och valör***
***Card-class created by Daniel Norberg med AI stöd***
***AI modell : QWEN3.8 27b q6***
*/

/// De fyra färgerna i en standardkortlek.
public enum Suit //Använder enum istället för class då det är fasta värden.
{
    Hearts,   // Hjärter
    Diamonds, // Ruter
    Clubs,    // Klöver
    Spades    // Spader
}

/// Kortens valörer, från två (2) till ess.
/// Numeriska värden (2–14) underlättar jämförelser i spellogik.

public enum Rank //Använder enum istället för class då det är fasta värden.
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

//Refererar att suit och rank är readonly properties som sätts i konstruktorn och inte kan ändras efteråt.
    public Card(Suit suit, Rank rank)
    {
        Suit = suit;
        Rank = rank;
    }
//Override ToString() för att ge en mer läsbar representation av kortet.
    public override string ToString()
    {
        return $"{RankToString()} av {SuitToString()}";
    }
//Metoder för att konvertera Rank och Suit till strängar på svenska.
    private string RankToString()
    {
        return Rank switch
        {
            Rank.Jack => "Knekt",
            Rank.Queen => "Dama",
            Rank.King => "Kung",
            Rank.Ace => "Ess",
            _ => ((int)Rank).ToString() // Om Rank inte matchar någon av de 4 valörer, använda Rank.ToString()
        };
    }
//Metod för att konvertera Suit till strängar på svenska.
    private string SuitToString()
    {
        return Suit switch
        {
            Suit.Hearts => "hjärter",
            Suit.Diamonds => "ruter",
            Suit.Clubs => "klöver",
            Suit.Spades => "spader",
            _ => Suit.ToString().ToLower() // Om Suit inte matchar någon av de 4 färgerna, använda Suit.ToString()
        };
    }
}

// US05: Deck flyttad till Deck.cs – den har eget ansvar (blandning,
// deal, korträkning) och egen Random. Denna fil innehåller bara
// Suit, Rank och Card.

