# Tjugoett – User Stories och branchplan

Kunskapskontroll 3, MAI26. Källspec: `MAI26-kunskapskontroll-3-tjugoett.pdf`.

Varje del av programmet är en user story på en egen feature-branch utgående från
`dev`. Tillbaka till `dev` via pull request (1 godkännande). `dev` mergas till
`main` en gång, strax före inlämning (2 godkännanden).

Skeletten i brancherna innehåller `TODO:er` – det är gruppens jobb att
implementera dem. Kom ihåg: kod ni inte kan redogöra för godkänns inte.

## Statusöversikt

| Story | Branch | Status |
|---|---|---|
| US01 Lägga till spelare (1–5) | `US01---Add-player` | ✅ mergad |
| US02 Välja sparad spelare eller skapa ny | `US02-Select-saved-player-or-create-new-one` | ✅ mergad |
| US03 Meny | `US03-Menu` | ✅ genomförd – meny med ↑/↓-navigering |
| US04 Card-class | `US04-Card-class` | ✅ mergad |
| US05 Deck: blanda, dela ut, räkna kort | `US05-Deck` | ✅ genomförd – i dev |
| US06 Hand: värde, mjuk/hård, spräckt, naturlig 21 | `US06-Hand` | ✅ genomförd – i dev |
| US07 Participant + Dealer + Move/Outcome | `US07-Participant-Dealer` | ✅ genomförd – i dev |
| US08 Round: en giv | `US08-Round` | ✅ genomförd – i dev |
| US09 Spelloop + Program.cs | `US09-Game-loop` | ✅ genomförd – i dev |
| US10 GameStorage: spara/ladda | `US10-GameStorage` | ✅ genomförd – i dev |
| US11 Inmatning: validering + visa händar | `US11-Input-validation` | ✅ genomförd – pilstöd + livebord |
| US12 Tillval 1: Kortgrafik | `US12-Card-graphics` | ✅ genomförd – kort i alla vyer |
| US13 Tillval 2: Datorspelare med strategi | `US13-Computer-players` | ✅ genomförd – antal + strategi i spelarmenyn |
| US14 Tillval 3: Husregler från konfigurationsfil | `US14-House-rules` | ✅ genomförd – rules.txt |
| US15 Tillval 4: Satsningar och marker | `US15-Chips-bets` | ✅ genomförd – insats + plånbok |
| US16 Tillval 5: Sko med flera lekar + korträkning | `US16-Shoe-card-counting` | ❌ skelett borttaget – ej genomförd |

## ✅ Tidigare problem (nu åtgärdade)

1. **dev saknade Program.cs** – löst, entry point finns (US09, PR #11).
2. **`US03-Menu` och `feature-card-graphics` var föråldrade** – löst:
   kortgrafiken är i dev (US18) och menyn är omgjord med pilstöd.

## Mergordning (beroenden)

- Parallellt: **US05, US06, US10, US11, US12, US14, US15, US16** och US03 efter rebase.
- Därefter i ordning: **US07 → US08 → US09**.
  US07 refererar till US06:s `Hand`; US08 refererar till US07:s `Dealer`/`Move`;
  US09 refererar till US08:s `Round`.
- Tillval: US12 och US13 kan börja tidigt (bara ny kod). US14–US16 kräver att
  grunden står eftersom de ändrar gemensam kod.

## Gemensam del

### US05 – Deck: blanda, dela ut, räkna kort
Som spelare vill jag att kortleken blandas riktigt och aldrig tar slut,
så att spelet är fair över många rundor.
- [x] Fisher-Yates: gå baklänges, byt plats med slumpval bland föregående
- [x] En `Random` delas av hela programmet (injekteras, skapas i Program.cs)
- [x] `Deal()` tar bort + returnerar ett kort, `RemainingCards` uppdateras
- [x] Färre än 15 kort kvar → ny blandad lek inför nästa runda
- [x] Deck flyttas ur Card-Class.cs; ingen Console i klassen

Branch: `US05-Deck` · Beror på: US04

### US06 – Hand: beräkna handens värde
Som spelare vill jag att handens värde alltid räknas rätt (även med flera ess),
så att mjuka/hårda händar, spräck och naturlig 21 avgörs rätt.
- [x] 2–10 sitt värde, klätt = 10, ess = 1 eller 11 (bäst utan att spräcka)
- [x] Värdet beräknas varje gång och sparas aldrig (esset byter värde)
- [x] `IsSoft`: något ess räknas som 11 i det aktuella värdet (A+6 = mjuk 17)
- [x] `IsBusted`: värdet över 21
- [x] `IsNatural`: exakt två kort som tillsammans blir 21

Branch: `US06-Hand` · Beror på: US04

### US07 – Participant + Dealer (+ Move/Outcome)
Som utvecklare vill jag att Player och Dealer delar det gemensamma
(namn, hand) så att strukturen blir tydlig och lätt att utöka.
- [x] `Participant`: namn + hand, `AddCard`
- [x] `Player : Participant` behåller sin statistik (Wins/Losses/Pushes)
- [x] `Dealer : Participant` har hålkortet + den fasta regeln
- [x] Givarens regel: köpa medan ≤16, stanna på 17+ (även mjuk 17)
- [x] `Move` (Hit/Stand) och `Outcome` (Win/Lose/Push/Blackjack)
- [x] Ingen Console i regelklasserna

Branch: `US07-Participant-Dealer` · Beror på: US06

### US08 – Round: en giv från start till avgörande
Som spelare vill jag att en runda spelas igenom helt,
så att programmet går att spela utan att krascha.
- [x] `DealCards`: 2 öppet till varje spelare, 1 öppet + 1 dolt till givaren
- [x] Spelarna i tur och ordning: köpa/stanna; spräckt = omedelbar förlust
- [x] Givarens tur: vänd hålkortet, följ regeln (US07)
- [x] `Settle`: avgörandetabellen (förlust / vinst / lika / naturlig 21)
- [x] Round pratar aldrig med Console – beslutet skickas in som `Func<Player, Move>`

Branch: `US08-Round` · Beror på: US05, US06, US07

### US09 – Spelloop + Program.cs
Som spelare vill jag kunna spela runda efter runda,
så att programmet går att spela igenom från start till slut.
- [x] Program.cs: skapar delad `Random`, spelare, `Game`; kör loopen
- [x] `Game.PlayRound`: Deck (omblandning vid <15) → Dealer → Round → statistik
- [x] Spela igen / avsluta styrs av menyn (US03)
- [x] Spara spelarna efter varje runda (US10)

Branch: `US09-Game-loop` · Beror på: US03, US05, US07, US08

### US10 – GameStorage: spara och ladda spelare
Som spelare vill jag att mina resultat finns kvar vid nästa start,
så att jag kan följa min utveckling över tid.
- [x] Spara namn + vunna/förlorade/lika till fil
- [x] Läs tillbaka vid start; saknad fil = tom lista (inte ett fel)
- [x] Trasig rad i filen ignoreras – ingen krasch

Branch: `US10-GameStorage` · Beror på: US01/US02

### US11 – Inmatning: validering och visa händar
Som spelare vill jag att felaktig inmatning hanteras vänligt
och att jag ser mina och givarens händar tydligt.
- [x] Alla läsningar frågar om tills svaret är giltigt (ingen krasch)
- [x] `ReadMove`: köpa/stanna med validering
- [x] `ShowHand` / `ShowDealer` (hålkortet dolt tills det vänts)
- [x] Klassnamnet `Consoleui` avviker från specens `ConsoleUi` – byt namn (filen heter redan `ConsoleUi.cs`)

Branch: `US11-Input-validation` · Beror på: US07/US08 (för händar)

## Tillval (ett per gruppmedlem)

### US12 – Tillval 1: Kortgrafik i konsolen
Som spelare vill jag se korten som kort (ruta med valör i hörn, färg i mitten),
så att spelet blir tydligt och roligare.
- [x] Renderaren returnerar `string[]` (rader) – ConsoleUi sätter färgen
- [x] Korten i en hand står bredvid varandra
- [x] Hålkortet ritas med baksidan upp (schackglyfer för klädda kort)
- [x] Inga ändringar krävs i regelklasserna (annars: läckage, skriv i rapporten)

Branch: `US12-Card-graphics` · Beror på: US06 · (se också `feature-card-graphics` med skiss)

### US13 – Tillval 2: Datorspelare med strategi
Som spelare vill jag kunna sitta mot datorspelare,
så att spelet går att testa och spela ensam.
- [x] `IPlayerStrategy.Decide(hand, dealerUpCard) → Move`
- [x] Minst två strategier: enkel (köpa <17) + grundstrategi (tabell)
- [x] Dator- och mänskliga spelare kan sitta samtidigt, olika strategier
- [x] Minst en mänsklig spelare krävs (max antal datorer = antal spelare − 1)
- [x] Mänskliga spelare spelar först i tur och ordning, sedan datorspelarna

Branch: `US13-Computer-players` · Beror på: US06/US07/US08

### US14 – Tillval 3: Husregler från konfigurationsfil
Som spelare vill jag kunna byta husregler utan omkompilering.
- [x] `RuleSet` läses från fil vid start, injekteras i Dealer/Round/Deck
- [x] Mjuk 17, antal lekar, omblandningströskel, naturlig 21 slår 3-korts-21
- [x] Saknad fil → standardvärden + meddelande, ingen krasch

Branch: `US14-House-rules` · Beror på: grunden (US05–US09)

### US15 – Tillval 4: Satsningar och marker
Som spelare vill jag kunna satsa marker,
så att vinst och förlust får konsekvens.
- [x] Marker per spelare, satsa innan given (0 < insats ≤ marker)
- [x] Vinst 1:1, naturlig 21 3:2, lika → insats tillbaka
- [ ] Slutar på marker → lämnar bordet; markerna sparas mellan körningar
- [ ] Överkurs: försäkring (2:1)

Branch: `US15-Chips-bets` · Beror på: grunden + US10

### US16 – Tillval 5: Sko med flera lekar + korträkning
Som spelare vill jag spela med en sko och se korträkning i realtid.
- [ ] Flera lekar blandade, skiljekort i bakre fjärdedelen → om efter rundan
- [ ] Hi-Lo: 2–6:+1, 7–9:0, 10/klätt/ess:−1
- [ ] Löpande + sanna räkningen + indikator för gynnsamt läge
- [ ] Räknaren är en ren observatör – den ändrar inget i spelet

Branch: `US16-Shoe-card-counting` · Beror på: grunden + US05

## De fyra designfrågorna (svara innan ni kodar – behövs i rapporten)

1. **Ska Card ha ett Value?** Förslag: nej som fält – esset gör ett fast värde
   felaktigt. Värderegeln bor i Hand och beräknas varje gång.
2. **Är givaren en spelare?** Förslag: delvis – via Participant delar de namn,
   hand och att de kan spräcka. Inte delat: beslutet (fast regel) och hålkortet.
3. **Vem bestämmer vad en spelare gör?** Förslag: aldrig Player själv – i
   grunden frågar ConsoleUi (Round får beslutet in som parameter), i US13
   byts det mot IPlayerStrategy.
4. **Var ligger givarens regel?** Förslag: på Dealer (`ShouldHit`), inte i
   Round – Round orkestrerar och behöver inte känna till allas regler.
