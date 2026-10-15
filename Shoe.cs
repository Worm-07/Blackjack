/*
 ***Tillval 5 (US16): Sko med flera lekar och skiljekort***
 ***Skelett – metoderna är markerade med TODO och ska implementeras.***
 */

/// En sko: flera lekar blandade till en, med ett skiljekort på slumpmässig
/// plats i den bakre fjärdedelen. När skiljekortet dras spelas rundan
/// färdigt och skon blandas om därefter.
/// Ersätter Deck i spelet (Round tar emot en Shoe istället).
public class Shoe
{
    private readonly Random _random;
    private readonly List<Card> _cards = new();
    private int _cutCardIndex;
    private bool _reshuffleAfterRound;

    public Shoe(int numberOfDecks, Random random)
    {
        // TODO Tillval 5: bygg numberOfDecks × 52 kort, bland,
        // sätt _cutCardIndex på slumpmässig plats i den bakre fjärdedelen.
        throw new NotImplementedException();
    }

    /// Hur många kort som är kvar i skon.
    public int RemainingCards => _cards.Count; // TODO Tillval 5: justera för skiljekortet

    /// Ska skon blandas om efter pågående runda? (skiljekortet har dragits)
    public bool ShouldReshuffleAfterRound => _reshuffleAfterRound;

    /// Tar bort och returnerar nästa kort. Sätter _reshuffleAfterRound
    /// när skiljekortet dras.
    public Card Deal()
    {
        // TODO Tillval 5
        throw new NotImplementedException();
    }

    /// Blanda om skon och återställ skiljekortet.
    public void Reshuffle()
    {
        // TODO Tillval 5
        throw new NotImplementedException();
    }
}
