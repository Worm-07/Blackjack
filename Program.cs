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

Game game = new(players, random);
ConsoleUi ui = new();

// Spelloopen: meny (US03) → runda (US08) → spara (US10).
while (true)
{
    if (ui.ShowMenu(game))
    {
        // US08: en runda. US11: spelarens beslut (köpa/stanna) levereras
        // till Round som Func&lt;Player, Card, Move&gt; – Round pratar aldrig
        // med Console. Givarens hand visas via ShowDealer (hålkortet dolt
        // tills det vänts) och en spräckt spelare ser sin hand via ShowHand.
        // US13: mänskliga spelare frågas av ConsoleUi, datorspelare
        // avgörs av sin IPlayerStrategy – Round behöver inte veta skillnaden.
        Round round = game.PlayRound(
            (player, upCard) =>
            {
                if (player.Strategy is { } strategy)
                {
                    Move move = strategy.Decide(player.Hand, upCard);
                    ui.ShowComputerMove(player, move);
                    return move;
                }
                return ui.ReadMove(player);
            },
            ui.ShowDealer, ui.ShowHand);

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
