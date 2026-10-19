/*
 ***US09: Program.cs – entry point***
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
        // till Round som Func<Player, Move> – Round pratar aldrig med Console.
        // Gränssnittet ritar livebordet med kortgrafik (US12) via ShowDealer
        // (hålkortet dolt tills det vänts) och markerar spräckt spelare via
        // ShowHand – så korten syns under rundans gång.
        Round round = game.PlayRound(ui.ReadMove, ui.ShowDealer, ui.ShowHand);

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
