Versjonshistorikk
=================

Gjenstående funsjonalitet før release 1.0
------------------------------------------

* Metadata registrering
* Noark 4 (implementere støtte for utvidet XSLT transformasjon til ADDML)
* Rapporter (utbedringer med tekster og sammendrag) 
* Laste inn tar pakke (av mappe mappestruktur)
* Oppdateringer av Noark5 tester (se v. 0.6.18)


Versjon 0.6.41
--------------
*Dato: 10.05.2017*

*Oppdateringer siden forrige versjon:*

*Oppdateringer*

* Løpende journal, offentlig journal: kan nå lese store filer
* Undermapper i Dokumenter, Noark5: leser nå disse 
* XML validering: stoppe validering etter 100 feil
* Kjøre videre ved feil filnavn
* ADDML skilletegn: raportere 100, så telle videre, ikke terminerer
* Mer brukervennlige feil meldinger


*Ytelse*

* søkefunksjoner i Noark5 ved behandling av dokumenter
* leser bare nødvendig informasjon fra enkelte filer, journal og logg fil

*Bug fix*

* Problem ved valg av vedlagt XSD addml, arkivstruktur
* Oppdateringer på Lag pakke GUIet.


*Nyttutvikling*

* GUI for Metadata, eksempel implementasjon av dette for testing og feedback. Funksjonen er ikke integrert inn i pakke genereringen.

Preutfylling av person data i filen:
C:\Users\{innlogget bruker}\Arkade\metadata-feltverdier.xml
C:\Brukere\{innlogget bruker}\Arkade\metadata-feltverdier.xml



Versjon 0.6.24
--------------
*Dato: 03.04.2017*

*Oppdateringer siden forrige versjon:*

* Feilfiksing: Problem med "null pointer exception" for Noark5 uttrekk er utbedret.
* Ny funksjonalitet: Valideringer mot vedlagte XSD filer
* Utbedring: ADDML avvik ved feil antall felt i en post (ofte skilletegn feil) rapporterer også post nummer.
* Utbedring: Programmet er satt opp til å terminere etter å ha funnet 1000 ADDML avvik ved feil antall felt i en post



Versjon 0.6.18
--------------
*Dato: 20.03.2017*

*Oppdateringer siden forrige versjon:*

* Alle Noark5 tester er implementert (se flere dealjer nedenfor)
* Minnebruksproblemer som oppstod ved ett stort antall Noark 5 dokumenter er utbedret (internt funnet)
* Fikset bug med «Kjør programmet på nytt» knappen for Noark 5
* Utbedring: Noark5 XML valideringsfeil blir listet samlet i GUI (og rapport) og programmet kjører videre.
* Utbedring: Kjører videre fra feil antall felt i ADDML post, hopper over behandling av posten og logger i GUI og rapport


*Noark5 implementasjonsdetaljer*

De følgende Noark5 testene ble implementert etter en ufullstending spesifikasjon, utbedringer med flere rapporteringsdetaljer følger i senere versjoner:

* Noark5 Testpunkt Analyse 01. Antall arkiver i arkivstrukturen
* Noark5 Testpunkt Analyse 02. Antall arkivdeler i arkivstrukturen
* Noark5 Testpunkt Analyse 03: Arkivdelen[e]s status i arkivstrukturen
* Noark5 Testpunkt Analyse 05. Antall klasser i arkivstrukturen
* Noark5 Testpunkt Analyse 04. Antall klassifikasjonssystemer i arkivstrukturen
* Noark5 Testpunkt Analyse 06. Antall klasser uten underklasser eller mapper i det primære klassifikasjonssystemet i arkivstrukturen
* Noark5 Testpunkt Kontroll 46. Kontroll på om dokumentobjektene i arkivstrukturen refererer til eksisterende dokumentfiler i arkivuttrekket
* Noark5 Testpunkt Analyse 07. Antall mapper i arkivstrukturen
* Noark5 Testpunkt Analyse 12. Antall registreringer i arkivstrukturen
* Noark5 Testpunkt Kontroll 40. Kontroll av sjekksummene i arkivuttrekk.xml
* Noark5 Testpunkt Kontroll 41. Validering av arkivstruktur.xml
* Noark5 Testpunkt Kontroll 44. Kontroll på at registreringer bare er knyttet til klasser uten underklasser i arkivstrukturen
* Noark5 Testpunkt Kontroll 42. Validering av endringslogg.xml
* Noark5 Testpunkt Analyse 37. Eventuelt - antall konverterte dokumenter i arkivstrukturen
* Noark5 Testpunkt Analyse 14. Antall registreringer for hvert år i arkivstrukturen
* Noark5 Testpunkt Kontroll 48. Kontroll av systemidentifikasjonene i arkivstrukturen
* Noark5 Testpunkt Analyse 39. Start- og sluttdato i arkivuttrekket
* Noark5 Testpunkt Kontroll 49. Kontroll av referansene til arkivdel i arkivstrukturen
* Noark5 Testpunkt Kontroll 50. Kontroll av referansene til sekundær klassifikasjon i arkivstrukturen
* Noark5 Testpunkt Kontroll 51. Kontroll av referansene i endringsloggen
* Noark5 Testpunkt Kontroll 45. Kontroll av sjekksummer
* Noark5 Testpunkt Analyse 26. Antall saksparter i arkivstrukturen
* Noark5 Testpunkt Analyse 29. Antall presedenser i arkivstrukturen
* Noark5 Testpunkt Analyse 30. Antall korrespondanseparter i arkivstrukturen
* Noark5 Testpunkt Analyse 31. Antall avskrivninger i arkivstrukturen
* Noark5 Testpunkt Analyse 32. Antall dokumentflyter i arkivstrukturen
* Noark5 Testpunkt Analyse 08. Antall mapper for hvert år i arkivstrukturen
* Noark5 Testpunkt Analyse 27. Antall merknader i arkivstrukturen
* Noark5 Testpunkt Analyse 10. Antall mapper uten undermapper eller registreringer i arkivstrukturen
* Noark5 Testpunkt Analyse 28. Antall kryssreferanser i arkivstrukturen
* Noark5 Testpunkt Analyse 21. Antall dokumentobjekter i arkivstrukturen
* Noark5 Testpunkt Analyse 19. Antall dokumentbeskrivelser uten dokumentobjekt i arkivstrukturen
* Noark5 Testpunkt Analyse 18. Antall dokumentbeskrivelser i arkivstrukturen
* Noark5 Testpunkt Analyse 16. Antall registreringer uten dokumentbeskrivelse i arkivstrukturen
* Noark5 Testpunkt Analyse 23. Antall dokumentfiler i arkivuttrekket
* Noark5 Testpunkt Analyse 22. Start- og sluttdato for dokumentene i arkivstrukturen
* Noark5 Testpunkt Analyse 38. Antall journalposter i arkivuttrekket
* Noark5 Testpunkt Analyse 09. Antall mapper som er klassifisert med hver enkelt klasse i arkivstrukturen
* Noark5 Testpunkt Analyse 11. Saksmappenes status i arkivstrukturen


De følgende Noark5 testene ble implementert etter en detaljert kravspesifikasjon (24.01.2017):

* Noark5 Testpunkt Kontroll 47. Kontroll på at det ikke finnes dokumentfiler i arkivuttrekket som mangler referanse fra arkivstrukturen
* Noark5 Testpunkt Analyse 13. Antall forskjellige journalposttyper i arkivstrukturen
* Noark5 Testpunkt Analyse 15. Antall registreringer som er klassifisert med hver enkelt klasse i arkivstrukturen
* Noark5 Testpunkt Analyse 17. Journalpostenes status i arkivstrukturen
* Noark5 Testpunkt Analyse 20. Dokumentbeskrivelsenes status i arkivstrukturen
* Noark5 Testpunkt Analyse 24. Antall dokumenter i arkivuttrekket fordelt på dokumentformat
* Noark5 Testpunkt Analyse 25. Antall dokumentfiler som blir referert til av flere enn ett dokumentobjekt
* Noark5 Testpunkt Analyse 34. Eventuelt - antall graderinger i arkivstrukturen
* Noark5 Testpunkt Kontroll 43. Kontroll på at mappene bare er knyttet til klasser uten underklasser i arkivstrukturen
* Noark5 Testpunkt Analyse 35. Eventuelt - antall kassasjonsvedtak i arkivstrukturen
* Noark5 Testpunkt Analyse 36. Eventuelt - antall utfÃ¸rte kassasjoner i arkivstrukturen
* Noark5 Testpunkt Analyse 33. Eventuelt - antall skjerminger i arkivstrukturen




Versjon 0.5.7
--------------
*Dato: 27.01.2017*

*Implementerte Noark5 tester siden forrige versjon:*

* Noark5 Testpunkt Analyse 08: Antall mapper for hvert år i arkivstrukturen
* Noark5 Testpunkt Analyse 14: Antall registreringer for hvert år i arkivstrukturen
* Noark5 Testpunkt Analyse 16: Antall registreringer uten dokumentbeskrivelse i arkivstrukturen
* Noark5 Testpunkt Analyse 18: Antall dokumentbeskrivelser i arkivstrukturen
* Noark5 Testpunkt Analyse 19: Antall dokumentbeskrivelser uten dokumentobjekt i arkivstrukturen
* Noark5 Testpunkt Analyse 21: Antall dokumentobjekter i arkivstrukturen
* Noark5 Testpunkt Analyse 23: Antall dokumentfiler i arkivuttrekket
* Noark5 Testpunkt Analyse 26: Antall saksparter i arkivstrukturen
* Noark5 Testpunkt Analyse 27: Antall merknader i arkivstrukturen
* Noark5 Testpunkt Analyse 28: Antall kryssreferanser i arkivstrukturen
* Noark5 Testpunkt Analyse 29: Antall presedenser i arkivstrukturen
* Noark5 Testpunkt Analyse 30: Antall korrespondanseparter i arkivstrukturen
* Noark5 Testpunkt Analyse 31: Antall avskrivninger i arkivstrukturen
* Noark5 Testpunkt Analyse 32: Antall dokumentflyter i arkivstrukturen
* Noark5 Testpunkt Analyse 09: Antall mapper som er klassifisert med hver enkelt klasse i arkivstrukturen
* Noark5 Testpunkt Analyse 11: Saksmappenes status i arkivstrukturen
* Noark5 Testpunkt Analyse 13: Antall forskjellige journalposttyper i arkivstrukturen
* Noark5 Testpunkt Analyse 22: Start- og sluttdato for dokumentene i arkivstrukturen
* Noark5 Testpunkt Analyse 38: Antall journalposter i arkivuttrekket
* Noark5 Testpunkt Kontroll 47: Kontroll på at det ikke finnes dokumentfiler i arkivuttrekket som mangler referanse fra arkivstrukturen


*Implementerte ADDML prosesser (Noark3, Noark4, og fagsystem) siden siste release:*

* ADDML post-prosess: Control_ForeignKey

*Andre oppdateringer:*

* Innlesing av IP pakker
* Oppretting av SIP og AIP pakker/pakkestruktur
* Arkade API -- kjernefunksjonenen i Arkade kjøres gjennom en API funksjon som kan være grunnlaget for andre grensesnitt implementasjoner
* Leser ikke lengre inn en tar fil som bare inneholder en mappestruktur
* XSLT fil for konverteriing fra Noark 4 (NOARK.IH) til ADDML.xml


Versjon 0.4.24
--------------
*Dato: 07.12.2016*

Dette er den andre versjonen av verktøyet som slippes for testing. 

Denne versjonen støtter følgende arkivtyper:

* Noark5
* Noark4
* Noark3
* Fagsystem

Arkivinnlesing kan gjøres fra mappestruktur og fra .tar-fil.

Programmet kjører 2 forskjellige løp for arkivtrekk som er definert av en ADDML fil i flatfil struktur og Noark5 uttrekk. Noark4 utrekk blir også kjørt under ADDML løpet ved at NOARKIH.xml filen blir transformert til en ADDML gjennom en XSLT.

*Implementerte Noark5 tester:*

* Noark5 Testpunkt Analyse 01: Antall arkiver i arkivstrukturen
* Noark5 Testpunkt Analyse 02: Antall arkivdeler i arkivstrukturen
* Noark5 Testpunkt Analyse 03: Arkiveldene status i arkivstrukturen
* Noark5 Testpunkt Analyse 04: Antall klassifikasjonssystemer i arkivstrukturen
* Noark5 Testpunkt Analyse 05: Antall klasser i arkivsstrukturen
* Noark5 Testpunkt Analyse 07: Antall mapper i arkivstrukturen
* Noark5 Testpunkt Kontroll 46: Kontroller at refererte dokumenter eksisterer i uttrekket 
* Noark5 Testpunkt Kontroll 46: Valider sjekksummer
* Valider xml i henhold til skjema

*Implementerte ADDML prosesser (Noark3, Noark4, og fagsystem):*

* Analyse_CountRecords
* Analyse_CountChars
* Analyse_FindExtremeRecords
* Analyse_CountRecordDefinitionOccurences
* Analyse_AllFrequenceList
* Analyse_CrossTable
* Analyse_CountNULL
* Analyse_FindExtremeValues
* Analyse_FindMinMaxValue
* Analyse_FrequenceList
* Control_AllFixedLength
* Control_NumberOfRecords
* Control_FixedLength
* Control_NotUsedRecordDef
* Control_Key 
* Control_ForeignKey
* Control_MinLength
* Control_MaxLength
* Control_DataFormat
* Control_NotNull
* Control_Uniqueness
* Control_Codes
* Control_Birthno
* Control_Organisationno
* Control_Accountno
* Control_Date_Value
* Control_Boolean_Value


Notater:

* Testrapporten og testloggen viser maks 100 like avvik. Dette er gjort for å redusere faren for minneproblemer ved uttrekk med store antall like avvik.
* Grensesnittet er oppdatert fra den tidligere versjonen.
* Arbeidsmappe ligger nå under c:/Brukere/{InnloggetBruker}/Arkade
* ADDML.xml eller arkivstruktur.xsd fil må ligge på rotnivå i mappestruktur eller tar fil. 
* ADDML.xml fil må definere hvilke prosesser som skal kjøres og hvordan de forholder seg til fil, post eller felt. Ikke alle tester kjøres på alle felt. 


Versjon 0.3.0
-------------
*Dato: 03.10.2016*

Dette er første versjonen som slippes til testing. I denne versjonen har vi fokusert på infrastruktur og legge grunnlaget for det videre arbeidet med utvikling av verktøyet. Det er implementert 7 testpunkter for NOARK 5 arkivuttrekk.

Brukergrensesnittet er ikke er ferdig utviklet, men danner et utgangspunkt for sluttproduktet.

Verktøyet produsere ikke noen annen output enn xml-loggen på nåværende tidspunkt. Denne legges i arbeidskatalogen under c:\temp sammen med arkivvutrekket som er testet.

Krav som kan testes:

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

Testpunkter som er implementert for Noark 5

* Antall arkiver i arkivstrukturen
* Antall arkivdeler i arkivstrukturen
* Arkiveldene status i arkivstrukturen
* Antall klassifikasjonssystemer i arkivstrukturen
* Antall klasser i arkivsstrukturen
* Antall klasser uten underklasser eller mapper i det primære klassifikasjonssystemet i arkivstrukturen
* Antall mapper i arkivstrukturen

