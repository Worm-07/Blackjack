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
Consoleui ui = new();

// Spelloopen: meny (US03) → runda (US08) → spara (US10).
while (true)
{
    // TODO US09: menyn ligger i branch US03-Menu (ShowMenu i Consoleui):
    // 1) Spela runda  2) Spelare (visa/lägg till)  3) Avsluta
    if (ui.ShowMenu())
    {
        // US08: en runda. US11: spelarens beslut (köpa/stanna) levereras
        // till Round som Func<Player, Move> – Round pratar aldrig med Console.
        game.PlayRound(ui.ReadMove);
        new GameStorage().SavePlayers(game.SavedPlayers); // US10
    }
    else
    {
        break;
    }
}
