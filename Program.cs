/*
 ***US09: Program.cs – entry point***
 */

// En Random delas av HELA programmet. Skapas den på nytt i Deck
// varje gång blandas den "lika" – Random såddas från klockan och
// två korta körningar ger samma lek.
Random random = new();

// US10: ladda tidigare spelare (namn + statistik) ur filen.
List<Player> players = new GameStorage().LoadPlayers();

// Tillval 3: Läs husreglerna från rules.txt. Saknas filen används
// standardvärden. Reglerna (tillsammans med eventuella
// inläsningsmeddelanden) visas under menyposten Regler.
RuleSet rules = RuleSet.LoadFromFile();

Game game = new(players, random, rules);
ConsoleUi ui = new();

// Spelloopen: meny (US03) → runda (US08) → spara (US10).
while (true)
{
    if (ui.ShowMenu(game))
    {
        // US08: en runda. US11: spelarens beslut (köpa/stanna) levereras
        // till Round som Func<Player, Card, Move> – Round pratar aldrig
        // med Console. Givarens hand visas via ShowDealer (hålkortet dolt
        // tills det vänts) och en spräckt spelare ser sin hand via ShowHand.
        // US13: mänskliga spelare frågas av ConsoleUi, datorspelare avgörs
        // av sin IPlayerStrategy – Round behöver inte veta skillnaden.
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
            ui.ReadBet, ui.ShowDealer, ui.ShowHand);

        // US09: visa resultatet: sista vyn med alla händar öppna och
        // avgörandet för varje spelare (Round visar själv aldrig).
        ui.ShowRoundEnd(round);

        new GameStorage().SavePlayers(game.SavedPlayers); // US10
    }
    else
    {
        break;
    }
}