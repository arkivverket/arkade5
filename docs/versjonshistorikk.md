# Versjon 0.3.0
**Dato: 03.10.2016**

Dette er første versjonen som slippes til testing. I denne versjonen har vi fokusert på infrastruktur og legge grunnlaget for det videre arbeidet med utvikling av verktøyet. Det er implementert 7 testpunkter for NOARK 5 arkivuttrekk.

Brukergrensesnittet er ikke er ferdig utviklet, men danner et utgangspunkt for sluttproduktet. 

Verktøyet produsere ikke noen annen output enn xml-loggen på nåværende tidspunkt. Denne legges i arbeidskatalogen under c:\temp sammen med arkivvutrekket som er testet.

**Krav som kan testes:**

K1.2 - Verktøyet er modulbasert - slik at det senere vil være enkelt å koble på funksjoner eller tjenester, tilpasse nye protokoller for kommunikasjon, og dessuten senere kunne skille ut en klient- og en tjenerdel

K1.5 - Brukergrensesnitt er malbasert, med tanke på fremtidige nye språkversjoner

K1.6 - Verktøyet lar seg enkelt installere på en Windows maskin (Windows 7, 8 og 10)

K2.7 - Verktøyet kan pakke ut en TAR fil

K2.8 - Verktøyet kan evaluere sjekksummer for tilhørende dokumenter i et uttrekk henhold til definerte algoritmer (SHA eller MD5 basert)

K2.9 - Verktøyet kan lese inn en ADDML definisjon

K2.10 - Verktøyet viser til enhver tid hva det gjør, og fremdrift for de forskjellige prosesser

K3.2 - Verktøyet tester sjekksummer for alle filer i uttrekket. Det gis feilmelding og programmet avsluttes dersom disse ikke stemmer.

K3.7 - Verktøyet kan teste Noark-5 ved hjelp av ADDML definisjonen, med tillegg av krav definert i Vedlegg 5. Se liste over testpunkter som er implementert nedenfor.

K5.2 - Alle automatiske operasjoner logges - med tidsstempel

K5.3 - Syntaks for loggen er strengt definert - og dokumentert på engelsk - som et XML format i form av en XSD fil

K5.4 - Det genereres en detaljert logg i henhold til XSD definisjonen over. Det tekstlige innholdet skal være engelsk- eller norskspråklig

**Testpunkter som er implementert for Noark 5**

1. Antall arkiver i arkivstrukturen
2. Antall arkivdeler i arkivstrukturen
3. Arkiveldene status i arkivstrukturen
4. Antall klassifikasjonssystemer i arkivstrukturen
5. Antall klasser i arkivsstrukturen
6. Antall klasser uten underklasser eller mapper i det primære klassifikasjonssystemet i arkivstrukturen
7. Antall mapper i arkivstrukturen
