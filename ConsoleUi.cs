class Consoleui
{
    public List<int> players = [];
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
}