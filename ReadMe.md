# Tjugoett (Blackjack)

Konsolspel i C#: 1–5 spelare mot en given. Byggs i Kunskapskontroll 3, MAI26.
Klasser och metoder på engelska, allt användaren ser på svenska.

## Så startar man

```
dotnet run --project Blackjack.csproj
```

OBS: `Program.cs` ingår i `US09-Game-loop` – projektet bygger inte förrän
den branchen är mergad till dev.

## Översikt över klasserna

| Klass | Ansvar | Story |
|---|---|---|
| Suit, Rank | Enum:ar för färg och valör | US04 |
| Card | Ett kort: färg + valör | US04 |
| Deck | Bygger lek, blandar (Fisher-Yates), delar ut, räkna kort | US04/US05 |
| Hand | Samling kort: värde, mjuk/hård, spräckt, naturlig 21 | US06 |
| Participant | Bas: namn + hand | US07 |
| Player | Ärver Participant: statistik, sitter vid bordet | US01/US02/US07 |
| Dealer | Ärver Participant: hålkort + fast regel | US07 |
| Round | En giv: deal, turer, givarens tur, avgörande | US08 |
| Game | Meny och spelloop | US02/US03/US09 |
| Move, Outcome | Enum:ar – drag och utfall | US07 |
| IPlayerStrategy | Gränssnitt: datorspelarens beslut | US13 |
| SimpleStrategy, BasicStrategy | Datorstrategier: köpa <17 / tabell | US13 |
| GameStorage | Spara/ladda spelare + resultat | US10 |
| ConsoleUi | All in- och utmatning | US11 |

## Vem har gjort vad

| Gruppmedlem | Tillval | Branch |
|---|---|---|
| Hadi Bahaulddin | 1 – Kortgrafik | feature-card-graphics, US18-visa-hander |
| (fyll i) | (1–5) | (branch) |

**Tillval 1 – Kortgrafik (Hadi):** Jag gjorde så att korten visas som riktiga kort i terminalen, bredvid varandra. Hjärter och ruter är röda, klöver och spader är svarta. Givarens dolda kort visas med baksidan upp tills det vänds. CardRenderer ritar korten och ConsoleUi skriver ut dem i färg.

Jag fixade också några buggar som vi hittade när vi testade: naturligt 21 mot 21 med tre kort, att spelet kraschade när korten tog slut, max 5 spelare vid bordet och att "Dama" skulle vara "Dam".

**Tillval 2 – Datorspelare:** Direkt efter valet av antal spelare (totalt antal: mänskliga + datorer) väljer man hur många av dem som ska vara datorspelare. Det måste alltid bli minst en mänsklig spelare, så max antal datorer är de valda spelarna minus ett. Varje datorspelare får automatiskt ett namn ("Dator 1", "Dator 2", …) och en strategi – antingen den enkla (köpa under 17) eller grundstrategin (tabell). Dator- och mänskliga spelare sitter samtidigt vid bordet och Round vet inte skillnaden: beslutet levereras in som en funktion där `Program.cs` väljer mellan `ConsoleUi` och `IPlayerStrategy`. Mänskliga spelare spelar först i tur och ordning, sedan datorspelarna. Datorspelare sparas inte i `players.txt`.

## Regler som gäller

Enligt specen i uppgiftsbeskrivningen (52 kort, givaren stannar på mjuk 17,
naturlig 21 betalar enligt avgörandetabellen, omblandning vid <15 kort).
Avvikelser från specifikationen listas här:
- (inga än)

## Kända brister

- `Program.cs` saknas tills US09 mergas.
- `US03-Menu` och `feature-card-graphics` behöver rebase mot dev.

## Klassdiagram

![Klassdiagram](docs/klassdiagram.png)

Källfilen finns i `docs/klassdiagram.md` och uppdateras när koden ändras.

## User stories

Se [docs/user-stories.md](docs/user-stories.md).
