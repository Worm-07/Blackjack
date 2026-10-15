/*
 ***Tillval 4 (US15): Satsningar och marker***
 ***Skelett – metoderna är markerade med TODO och ska implementeras.***
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
        // TODO Tillval 4: kasta exception (eller kolla CanAfford först) om
        // !CanAfford(amount); Balance -= amount; CurrentBet = amount;
        throw new NotImplementedException();
    }

    /// Vinst betalar 1:1 – insatsen tillbaka plus lika mycket till.
    public void CollectWin()
    {
        // TODO Tillval 4: Balance += CurrentBet * 2; CurrentBet = 0;
        throw new NotImplementedException();
    }

    /// Naturlig tjugoett betalar 3:2 – insatsen tillbaka + 1,5 × insatsen.
    public void CollectBlackjack()
    {
        // TODO Tillval 4: Balance += CurrentBet + (CurrentBet * 3) / 2; CurrentBet = 0;
        throw new NotImplementedException();
    }

    /// Lika ger insatsen tillbaka.
    public void ReturnBet()
    {
        // TODO Tillval 4: Balance += CurrentBet; CurrentBet = 0;
        throw new NotImplementedException();
    }

    /// Förlust: insatsen är redan borta (dragen vid Bet) – nollställ bara.
    public void CollectLoss()
    {
        // TODO Tillval 4: CurrentBet = 0;
        throw new NotImplementedException();
    }

    /// En spelare som får slut på marker lämnar bordet.
    public bool IsBroke => Balance == 0;
}
