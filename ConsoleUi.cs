class Consoleui
{
    public List<string> players = [];
    public int NumbersOfPlayers;
    public void Print()
    {
        Console.WriteLine("1 - 5 spelare");
        Console.WriteLine("Välj antalet spelare: ");
        string? input = Console.ReadLine();
        if (int.TryParse(input, out int output))
        {
            if (output >= 1 && output <= 5)
            {
                NumbersOfPlayers = output;
            }
            else
            {
                Console.WriteLine(" === VÄLJ MELLAN 1 OCH 5 SPELARE ===");
            }
        }
        else
        {
            Console.WriteLine(" === ANGE ETT NUMMER ===");
        }
    }

    public void NamesOfPlayers()
    {
        for (int i = 0; i < NumbersOfPlayers; i++)
        {
            string? name = "";
            while (true)
            {
                Console.WriteLine("Ange namn på spelaren: ");
                name = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(name))
                {
                    Console.WriteLine(" === ANGE ETT NAMN ===");
                }
                else
                {
                    break;
                }
            }
            players.Insert(i, name);
        }
    }
}