/*
 ***Tillval 4 (US15): Satsningar och marker***
 */

/// En spelares plånbok: marker, satsning och utbetalning.
/// Grundversionen räknar bara vunna/förlorade rundor – här läggs en ekonomi till:
/// vinst betalar 1:1, naturlig tjugoett 3:2, lika ger insatsen tillbaka.
public class ChipWallet
{
    /// Markerna som spelaren har kvar.
    public int Balance { get; private set; }

    /// Aktuell insats (noll om ingen runda pågår).
    public int CurrentBet { get; private set; }

    public ChipWallet(int startingChips = 100)
    {
        Balance = startingChips;
    }

    /// Insatsen får inte vara större än spelarens marker, och inte noll.
    public bool CanAfford(int bet) => bet > 0 && bet <= Balance;

    /// Satsa innan given. Dra ifrån markerna och spara insatsen.
    public void Bet(int amount)
    {
        if (CanAfford(amount))
        {
            Balance -= amount;
            CurrentBet = amount;
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Användaren försöker betta negativt, 0 eller över sitt saldo");
        }
    }

    /// Vinst betalar 1:1 – insatsen tillbaka plus lika mycket till.
    public void CollectWin()
    {
        Balance += CurrentBet * 2;
        CurrentBet = 0;
    }

    /// Naturlig tjugoett betalar 3:2 – insatsen tillbaka + 1,5 × insatsen.
    /// Heltalsdivisionen avrundar nedåt.
    public void CollectBlackjack()
    {
        Balance += CurrentBet + (CurrentBet * 3 / 2);
        CurrentBet = 0;
    }

    /// Lika ger insatsen tillbaka.
    public void ReturnBet()
    {
        Balance += CurrentBet;
        CurrentBet = 0;
    }

    /// Förlust: insatsen är redan borta (dragen vid Bet) – nollställ bara.
    public void CollectLoss()
    {
        CurrentBet = 0;
    }

    /// En spelare som får slut på marker lämnar bordet.
    public bool IsBroke => Balance == 0;
}
