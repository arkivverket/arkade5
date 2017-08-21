Brukerveiledning
================

Oppdatert 21.08.2017

Arkade 5 brukes ved å lese inn et arkivuttrekk, kjøre tester på uttrekket og/eller opprette en arkivpakke av uttrekket. Ved testing genereres det en utfyllende testrapport.
Arkade 5 muliggjør også opprettelse/endring av metadata for arkivuttrekk.

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


Rapportgenerering
-----------------

En HTML-rapport vil automatisk bli generert ved fullført testing. Klikk på knappen "Vis rapport" for å se rapporten i en nettleser. Rapporten vil tilpasse seg den gjeldende skjermflaten.

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

Arkade tillater registrering av metadata for arkivpakken som skal opprettes. Arkade vil forsøke å lese inn eventuelle eksisterende metadata, fra en mets.xml-fil i arkivittrekket, og forhåndsutfylle feltene i pakkevinduet. Når pakken opprettes skrives den utfylte informasjonen til en (ny) mets.xml-fil som legges ved i arkivpakken. Metadataene skrives også til filen info.xml* som legges utenfor, på samme nivå som, arkivpakken.

*\*Filnavnet info.xml-filen blir opprettet med vil være UUID-en som er generert for den gjeldende arkivbehandlingen: {uuid}.xml*

I nedre del av vinduet velges ønsket pakketype, SIP eller AIP.

For å opprette en arkivpakke, klikk på knappen "Opprett pakke". 

Arkivpakken vil opprettes som en tar-fil og filnavnet vil være UUID-en som er generert for den gjeldende arkivbehandlingen: *{uuid}.tar*. Pakken vil plasseres i arbeidskatalogen for den gjeldende arkivbehandlingen. (Mer om arbeidskatalogen under `Fil- og kataloginformasjon`_)

*TIPS: Metadata for arkivpakken kan endres etter at pakken er opprettet forutsatt at gjeldende arkivbehandling ikke er avsluttet. Ved gjentatte klikk på "Opprett pakke" vil den samme pakken (samt info.xml-fil) overskrives. Sørg bare for at fildestinasjonen ikke er opptatt, f.eks. ved at pakken er åpnet i, og oppholdes av, et eksternt pakkeprogram.*

NB! Knappen "Ny kjøring" avslutter gjeldene arkivbehandling.


Fil- og kataloginformasjon
~~~~~~~~~~~~~~~~~~~~~~~~~~

Arkade benytter en katalog, "Arkade", for alle filer som produseres/oppdateres under kjøring av programmet. Katalogen opprettes automatisk om den ikke finnes og er/blir plassert direkte under filområdet for innlogget Windows-bruker: *C:\\Brukere\\{bruker}\\Arkade\\*

Hver gang et arkivuttrekk lastes inn med Arkade, opprettes det en arbeidskatalog for den gjeldende behandlingen av uttrekket. Plasseringen for arbeidkatalogene er *~\\Arkade\\work\\*.

Navnet på en arbeidskatalog blir generert under innlasting av arkivuttrekket og er sammensatt av dato og tidspunkt for innlastingen etterfulgt av en unik identifikator (UUID) som blir generert for den gjeldende arkivbehandlingen.

Etter fullført testkjøring vil en loggfil på XML-format, inneholdende informasjon om utførelsen av hver test, og en testrapport på HTML-format være opprettet under følgende plassering i den gjeldende arbeidkatalogen:
*\\administrative_metadata\\repository_operations\\arkade-log.xml*

*Arkades system- og feillogger (beregnet på systemutviklere) skrives til katalogen ~\\Arkade\\logs\\.*
