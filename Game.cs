class Game
{
    public enum MenuChoice { StartNewRound, StartExistingRound, ShowStatistic, Quit }

    private Consoleui _ui = new Consoleui();

    public void Run()
    {
        bool running = true;

        while (running)
        {
            MenuChoice choice = _ui.Menu();

            switch (choice)
            {
                case MenuChoice.StartNewRound:
                    // Starta ny runda när Round finns
                    break;
                case MenuChoice.StartExistingRound:
                    // Återuppta senaste spelet
                    break;
                case MenuChoice.ShowStatistic:
                    // Visa statestik
                    break;
                case MenuChoice.Quit:
                    running = false;
                    break;
            }
        }
    }
}