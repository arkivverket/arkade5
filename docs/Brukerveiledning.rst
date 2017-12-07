Brukerveiledning
================

Oppdatert 21.08.2017

Arkade 5 brukes ved å lese inn et arkivuttrekk, utføre testing av uttrekket og/eller opprette en arkivpakke av uttrekket. Ved testing genereres det en utfyllende testrapport.
Arkade 5 muliggjør også opprettelse/endring av metadata for arkivuttrekk.


Oppstart/avslutning
~~~~~~~~~~~~~~~~~~~

Ved oppstart av Arkade åpnes innlastingsvinduet. Dersom et tilgjengelig område for midlertidige filer ikke allerede er definert, vil Arkade først be om at dette blir opppgitt (se innstillinger). Arkade avsluttes ved å lukke hovedvinduet. Ved avslutning igangsettes sletting av midlertidige filer.


Innlastingsvinduet
~~~~~~~~~~~~~~~~~~

.. image:: img/LoadArchiveWindow.png

Innlasting
----------

Velg og last inn arkivuttrekket som skal behandles:

1) Klikk på knappen "Velg katalog" dersom uttrekket er en ordinær fil-/mappestruktur. I tilfelle velges katalogen som inneholder arkivbeskrivelse-filen (arkivstruktur.xml, addml.xml eller NOARKIH.xml). Klikk på knappen "Velg SIP/AIP-fil" dersom uttrekket er en AIP- eller SIP-struktur pakket som en tar-fil.

2) Oppgi arkivtype for det valgte uttrekket. Arkade 5 støtter typene "Fagsystem", "Noark 3", "Noark 4" og "Noark 5".

3) Klikk på knappen "Last inn uttrekk". Det valgte uttrekket vil lastes inn og åpnes i testvinduet.


Testvinduet
~~~~~~~~~~~

.. image:: img/TestRunWindow.png

Øverst i testvinduet vises:

* Full filsti for det valgte uttrekket
* En unik identifikator (UUID), generert for den gjeldende behandlingen av det valgte uttrekket
* Valgt arkivtype
* Hvilken fil som under testing prosesseres i øyeblikket
* Løpende informasjon om testkjøring*

*\*For uttrekk av typen Noark 5 vises antall prosesserte XML-elementer. For uttrekk basert på en ADDML-fil vises antall prosesserte filer og antall prosesserte poster.*


Testkjøring
-----------

Klikk på knappen "Start testing" for å starte testkjøring på det valgte uttrekket. Testkjøringen vil vare fra noen minutter til mange timer, avhengig av uttrekkets størrelse.

Under testkjøring vil det, i den nedre delen av vinduet, vises meldinger om innlesing, ev. strukturelle feil/mangler ved uttrekket, rapportgenerering og fullført testing.


Testrapport
-----------

Etter fullført testing vil en rapport i HTML-format bli generert. Klikk på knappen "Vis rapport" for å åpne den i en nettleser (den vil tilpasse seg gjeldende skjermflate). Ved opprettelse av arkivpakke inkluderes alltid testrapporten. Dersom den (i tillegg) skal tas vare på et annet sted, lagres den ved hjelp av nettleseren. Med rapporten åpen kan dette, i de fleste nettlesere, gjøres ved å taste Ctrl+s.

.. image:: img/HtmlTestReport.png


Arkivpakkegenerering
--------------------

Klikk på knappen "Opprett pakke" for å lage en arkivpakke (AIP/SIP) av uttrekket. Dette åpner arkivpakkevinduet der valg for pakken kan gjøres før den opprettes.

*Det er mulig å opprette en arkivpakke uten først å utføre testing av det aktuelle arkivuttrekket. En slik pakke vil ikke inneholde noen testrapport.*


Nytt uttrekk / ny kjøring
-------------------------

Ved klikk på knappen "Ny kjøring" avsluttes pågående arkivbehandling og Arkade returnerer til innlastingsvinduet. 
Dersom det inneværende arkivuttrekket endres, f.eks. som følge av feil/mangler vist i testrapporten, må uttrekket lastes inn på nytt (og ev. tester kjøres på nytt) før knappen "Opprett pakke" oppretter en pakke som inneholder endringene (og knappen "Vis rapport" åpner en gyldig testrapport). Uttrekket lastes inn på nytt ved å klikke "Ny kjøring" (eller ved å starte Arkade på nytt).

*NB! Skal det opprettes en arkivpakke som inkluderer resultatene fra inneværende testkjøring, må dette gjøres før "Ny kjøring" klikkes (eller Arkade avsluttes).*


Arkivpakkevinduet
~~~~~~~~~~~~~~~~~

.. image:: img/PackageWindow.png

Arkade tillater registrering av metadata for arkivpakken som skal opprettes. Arkade vil forsøke å lese inn eventuelle eksisterende metadata, fra en mets.xml-fil i arkivuttrekket, og forhåndsutfylle feltene i arkivpakkevinduet. Når pakken opprettes skrives den utfylte informasjonen til en (ny) mets.xml-fil som legges ved i arkivpakken. Metadataene skrives også til filen info.xml som legges utenfor, på samme nivå som, arkivpakken.

I nedre del av vinduet velges ønsket pakketype, SIP eller AIP.

Når ønskede metadata er oppgitt, klikkes knappen "Opprett pakke". Dette åpner et dialogvindu for valg av pakkens plassering. Ved valgt plassering opprettes arkivpakken.

Arkivpakken vil opprettes som en tar-fil og filnavnet vil være UUID-en som er generert for den gjeldende arkivbehandlingen: *{uuid}.tar*. Pakken og tilhørende info.xml-fil plasseres i en katalog *Arkadepakke-{uuid}*. Når alt er ferdig generert, vises denne katalogen på den valgte plasseringen.

*TIPS: Så lenge arkivpakkevinduet ikke forlates, kan metadata endres og "Opprett pakke" klikkes på nytt. Velges samme pakkeplassering, overskrives foregående pakke og info.xml med oppdaterte metadata. Sørg bare for at filene som skal overskrives ikke er opptatt, f.eks. ved at de er åpnet i andre programmer.*

NB! Knappen "Ny kjøring" avslutter gjeldene arkivbehandling.


Innstillinger
~~~~~~~~~~~~~

Prosesseringsområde
-------------------

Under kjøring benytter Arkade et filområde til plassering av midlertidige filer fra arkivprosessering, system- og feillogger samt andre systemfiler. Plassering for prosesseringsområdet velges av bruker som en katalog i filsystemet og må være definert før arkiv kan behandles. Plasseringen som velges må være egnet med tanke på størrelse, tilgjengelighet og personvern. Størrelsen må være minst den av alle uttrekk som skal behandles under samme kjøring i tillegg til plass for systemfiler. Ved avslutning av Arkade igangsettes sletting av midlertidige filer og gamle loggfiler. Plasseringen av prosesseringsområdet kan når som helst endres fra innstillingsvinduet. Ved endring av plassering, igangsettes sletting av opprinnelig prosesseringsområde. Ny plassering vil tas i bruk neste gang Arkade startes.
