/*
 ***US15: Outcome – hur en spelares runda slutar***
 */

/// Utfallet av en runda i förhållande till givaren (US15):
/// Win, Lose, Push eller Blackjack.
public enum Outcome
{
    /// Vinst.
    Win,

    /// Förlust.
    Lose,

    /// Lika – rundan räknas varken som vinst eller förlust.
    Push,

    /// Naturlig tjugoett (ess + tiokort på de två första korten).
    Blackjack
}
