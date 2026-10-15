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
| GameStorage | Spara/ladda spelare + resultat | US10 |
| ConsoleUi | All in- och utmatning | US11 |

## Vem har gjort vad

| Gruppmedlem | Tillval | Branch |
|---|---|---|
| (fyll i) | (1–5) | (US12–US16) |

## Regler som gäller

Enligt specen i uppgiftsbeskrivningen (52 kort, givaren stannar på mjuk 17,
naturlig 21 betalar enligt avgörandetabellen, omblandning vid <15 kort).
Avvikelser från specifikationen listas här:
- (inga än)

## Kända brister

- `Program.cs` saknas tills US09 mergas.
- `US03-Menu` och `feature-card-graphics` behöver rebase mot dev.

## Klassdiagram

Läggs in som `docs/klassdiagram.png` och ska spegla koden vid inlämning.

## User stories

Se [docs/user-stories.md](docs/user-stories.md).
