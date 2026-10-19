/*
 ***US09: Program.cs – entry point***
 ***Skelett – se TODO:erna.***
 */

// En Random delas av HELA programmet. Skapas den på nytt i Deck
// varje gång blandas den "lika" – Random såddas från klockan och
// två korta körningar ger samma lek.
Random random = new();

// US10: ladda tidigare spelare (namn + statistik) ur filen.
List<Player> players = new GameStorage().LoadPlayers();

// Tillval 3: Läs husreglerna från rules.txt. Saknas filen används
// standardvärden. ConsoleUi visar reglerna och eventuella meddelanden.
RuleSet rules = RuleSet.LoadFromFile();

Game game = new(players, random, rules);
ConsoleUi ui = new();
ui.ShowRules(rules);

// Spelloopen: meny (US03) → runda (US08) → spara (US10).
while (true)
{
    if (ui.ShowMenu(game))
    {
        // US08: en runda. US11: spelarens beslut (köpa/stanna) levereras
        // till Round som Func<Player, Move> – Round pratar aldrig med Console.
        // Givarens hand visas via ShowDealer (hålkortet dolt tills det vänts)
        // och en spräckt spelare ser sin hand via ShowHand.
        Round round = game.PlayRound(ui.ReadMove, ui.ReadBet, ui.ShowDealer, ui.ShowHand);

        // US09: visa resultatet för varje spelare (Round visar själv aldrig).
        foreach (Player p in round.Players)
            ui.ShowResult(p, round.Settle(p));

        new GameStorage().SavePlayers(game.SavedPlayers); // US10
    }
    else
    {
        break;
    }
}