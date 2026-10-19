# Tjugoett (Blackjack)

Konsolspel i C#: 1–5 spelare mot en given. Byggs i Kunskapskontroll 3, MAI26.
Klasser och metoder på engelska, allt användaren ser på svenska.

## Så startar man

```
dotnet run --project Blackjack.csproj
```

## Styrning

| Skärm | Kontroller |
|---|---|
| Huvudmeny | `↑`/`↓` (eller `1`–`5`) markera, `Enter` välja, `Esc` avsluta |
| Spelare | `j` lägg till, `1` starta runda, `Enter`/`Esc` tillbaka |
| Highscores | `1` flest vinstar, `2` flest förluster, `3` namn A–Ö, `Enter`/`Esc` tillbaka |
| Regler | `Enter`/`Esc` tillbaka |
| Runda | `k` köpa, `s` stanna (även piltangenterna: `→` köpa, `←` stanna) |

I en interaktiv konsol färgsätts den valda menyposten och den aktiva
spelaren. Korten ritas som kort (US12): valör i hörnen, färg i mitten,
givarens hålkort dolt (baksidan upp) tills det vänts.

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
| ConsoleUi | All in- och utmatning: meny, highscores, regler, livebord | US11 |
| CardRenderer | Kortgrafik: kort som rader med box-drag | US12 |

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

- Inga kända funktionella brister.
- Den lokala `dev` ligger efter `origin/dev` (tillval US13–US16 är mergade
  på origin men ej tagna in i den lokala grenen).

## Klassdiagram

Läggs in som `docs/klassdiagram.png` och ska spegla koden vid inlämning.

## User stories

Se [docs/user-stories.md](docs/user-stories.md).
