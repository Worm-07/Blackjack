/*
 ***US10: GameStorage – läser och skriver spelare och resultat till fil***
 */

/// Läser och skriver spelare och resultat till fil, så att de finns kvar
/// nästa gång programmet startas.
/// Filformat, en spelare per rad:  Namn;Vinstar;Förluster;Lika;Saldo
/// Exempel:                         Daniel;12;5;3;100
public class GameStorage
{
    /// Filen ligger i programkatalogen.
    public const string FileName = "players.txt";

    /// Läser alla sparade spelare. Saknas filen → tom lista (inte ett fel).
    /// Trasiga rader ignoreras – programmet ska aldrig krascha på filen.
    public List<Player> LoadPlayers()
    {
        var players = new List<Player>();
        if (!File.Exists(FileName))
            return players;

        foreach (var line in File.ReadAllLines(FileName))
        {
            var parts = line.Split(';');
            // 5 fält = gammalt format utan MestAntalMarkerUnderSpel, 6 = nuvarande.
            if (parts.Length is not (5 or 6))
                continue; // trasig rad – ignoreras

            if (!int.TryParse(parts[1], out int wins) ||
                !int.TryParse(parts[2], out int losses) ||
                !int.TryParse(parts[3], out int pushes) ||
                !int.TryParse(parts[4], out int balance))
                continue; // trasig rad – ignoreras

            int maxChips = 0;
            if (parts.Length == 6)
            {
                if (!int.TryParse(parts[5], out maxChips))
                    continue; // trasig rad – ignoreras
            }

            players.Add(new Player(parts[0], wins, losses, pushes, balance, maxChips));
        }

        return players;
    }

    /// Skriv alla spelare (namn + statistik + saldo) till fil.
    public void SavePlayers(IReadOnlyList<Player> players)
    {
        File.WriteAllLines(
            FileName,
            players.Select(p => $"{p.Name};{p.Wins};{p.Losses};{p.Pushes};{p.Wallet.Balance};{p.MaxChipsDuringGame}"));
    }
}
