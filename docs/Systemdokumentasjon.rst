Systemdokumentasjon
===================

List of supported ADDML processes

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
* Control_ForeignKey


List of implemeted Noark5 Tests

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



Arkivverket.Arkade
------------------
This is the core library with functions for reading and testing archive extractions, generating reports and creating SIP/AIP-packages.

List of packages:

**Core** - Common classes

**ExternalModels** - Classes generated from xml schemas

**Identify** - Identification classes for reading and identifying an archive extraction

**Tests** - Contains all test classes for testing archive extractions

**Util** - General utilities

ArkadeAPI
---------

Single interface to the core functionality.


.. code-block:: C

   public class ArkadeApi
   {
      public ArkadeApi(TestSessionFactory testSessionFactory, 
            TestEngineFactory testEngineFactory, 
            MetadataFilesCreator metadataFilesCreator, 
            InformationPackageCreator informationPackageCreator, 
            TestSessionXmlGenerator testSessionXmlGenerator) {}
         
         public TestSession RunTests(ArchiveDirectory archiveDirectory) {}
         public TestSession RunTests(ArchiveFile archive) {}
         public void CreatePackage(TestSession testSession, PackageType packageType)
         public void SaveReport(TestSession testSession, FileInfo file) {}
   }



Arkivverket.Arkade.UI
---------------------

This project provides the graphical user interface of the Arkade 5 software. It is based on WPF, Windows Presentation Foundation. 
Together with WPF, the application uses the Prism_ library for creating a loosly coupled, maintainable and testable XAML application.  

Autofac_ is used as a dependency framework. Bootstrapping of the applications happens in **Bootstrapper.cs**. It is based on the bootstrapper provided by Prism and it loads the Autofac-module provided by the Arkade core library. 

The design and layout is based on Google's Material_ Design. This has been implemented with the help of the [MaterialDesignThemes-library](http://materialdesigninxaml.net/). Note that the user interface is only inspired by the material design, not neccessary strictly following it in every situation. 


.. _Prism: https://github.com/PrismLibrary/Prism
.. _Autofac: https://github.com/PrismLibrary/Prism
.. _Material: https://material.google.com/
