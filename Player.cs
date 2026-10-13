public class Player
{
    public string Name { get; }

    public int Wins { get; private set; }
    public int Losses { get; private set; }
    public int Pushes { get; private set; }

    public Player(string name)
    {
        Name = name;
    }

    public Player(string name, int wins, int losses, int pushes)
    {
        Name = name;
        Wins = wins;
        Losses = losses;
        Pushes = pushes;
    }
}