/*
 ***US10: GameStorage – läser och skriver spelare och resultat till fil***
 ***Skelett – metoderna är markerade med TODO och ska implementeras.***
 */

/// Läser och skriver spelare och resultat till fil, så att de finns kvar
/// nästa gång programmet startas.
///
/// Filformat, en spelare per rad:  Namn;Vinstar;Förluster;Lika
/// Exempel:                         Danie;12;5;3
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
        // TODO US10:
        // if (!File.Exists(FileName)) return new List<Player>();
        // Läs rad för rad, Split(';') → 4 delar, int.TryParse på siffrorna,
        // new Player(name, w, l, p) – hoppa över rader som inte stämmer.
        throw new NotImplementedException();
    }

    /// <summary>Skriv alla spelare (namn + statistik) till fil.</summary>
    public void SavePlayers(IReadOnlyList<Player> players)
    {
        // TODO US10:
        // File.WriteAllLines(FileName,
        //     players.Select(p => $"{p.Name};{p.Wins};{p.Losses};{p.Pushes}"));
        throw new NotImplementedException();
    }
}
