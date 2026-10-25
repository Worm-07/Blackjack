/*
 ***Tillval 2 (US13): Datorspelare med strategi***
 */

/// Gemensamt gränssnitt för datorspelares strategier.
/// Polymorfism som gör något begripligt: samma anrop, olika beteende –
/// och utbytbart under körning. Flera datorspelare kan ha olika
/// strategier samtidigt.
public interface IPlayerStrategy
{
    /// Bestäm nästa drag utifrån spelarens hand och givarens öppna kort.
    Move Decide(Hand hand, Card dealerUpCard);
}
