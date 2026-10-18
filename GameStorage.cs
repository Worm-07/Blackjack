/*
 ***US10: GameStorage – läser och skriver spelare och resultat till fil***
 */

/// Läser och skriver spelare och resultat till fil, så att de finns kvar
/// nästa gång programmet startas.
///
/// Filformat, en spelare per rad:  Namn;Vinstar;Förluster;Lika;Saldo
/// Exempel:                         Danie;12;5;3;100
public class GameStorage
{
    /// Filen ligger i programkatalogen.
    public const string FileName = "players.txt";

    /// <summary>
    /// Läser alla sparade spelare. Saknas filen → tom lista (inte ett fel).
    /// Trasiga rader ignoreras – programmet ska aldrig krascha på filen.
    /// </summary>
    public List<Player> LoadPlayers()
    {
        var players = new List<Player>();
        if (!File.Exists(FileName))
            return players;

        foreach (var line in File.ReadAllLines(FileName))
        {
            var parts = line.Split(';');
            if (parts.Length != 5)
                continue; // trasig rad – ignoreras

            if (!int.TryParse(parts[1], out int wins) ||
                !int.TryParse(parts[2], out int losses) ||
                !int.TryParse(parts[3], out int pushes) ||
                !int.TryParse(parts[4], out int balance))
                continue; // trasig rad – ignoreras

            players.Add(new Player(parts[0], wins, losses, pushes, balance));
        }

        return players;
    }

    /// <summary>Skriv alla spelare (namn + statistik + saldo) till fil.</summary>
    public void SavePlayers(IReadOnlyList<Player> players)
    {
        File.WriteAllLines(
            FileName,
            players.Select(p => $"{p.Name};{p.Wins};{p.Losses};{p.Pushes};{p.Wallet.Balance}"));
    }
}
