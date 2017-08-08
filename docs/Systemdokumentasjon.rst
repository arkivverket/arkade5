*******************
Systemdokumentasjon
*******************

Source code
===========

The source code is located at the GitHub-repository: https://github.com/arkivverket/arkade5/

Arkade is developed with .Net and C#. The solution-file (.sln) is compatible with Visual Studio 2015 and above. 

Below is a brief description of each project in the solution. 


Arkivverket.Arkade
------------------
This is the core library with functions for reading and testing archive extractions, generating reports and creating SIP/AIP-packages.

List of packages:

**Core** - Domain classes

**ExternalModels** - Classes generated from xml schemas

**Identify** - Identification classes for reading and identifying an archive extraction

**Logging** - Classes related to logging of events during testing

**Metadata** - Contains classes related to creating metadata files for archive extractions

**Report** - Classes for generating test reports

**Resource** - Various resource files, language files, images etc.

**Tests** - Contains all test classes for testing archive extractions

**Util** - General utilities


Arkivverket.Arkade.UI
---------------------

This project provides the graphical user interface of the Arkade 5 software. It is based on WPF, Windows Presentation Foundation. 
Together with WPF, the application uses the Prism_ library for creating a loosly coupled, maintainable and testable XAML application.  

Autofac_ is used as a dependency framework. Bootstrapping of the applications happens in **Bootstrapper.cs**. It is based on the bootstrapper provided by Prism and it loads the Autofac-module provided by the Arkade core library. 

The design and layout is based on Google's Material_ Design. This has been implemented with the help of the [MaterialDesignThemes-library](http://materialdesigninxaml.net/). Note that the user interface is only inspired by the material design, not neccessary strictly following it in every situation. 


.. _Prism: https://github.com/PrismLibrary/Prism
.. _Autofac: https://github.com/PrismLibrary/Prism
.. _Material: https://material.google.com/

Arkivverket.Arkade.ConsoleTest
------------------------------
This is a sample application, which demonstrates the use of the Arkade API.

Arkivverket.Arkade.Test
-----------------------
This project contains the unit tests and other tests classes for the project. Unit tests are created with xUnit. 

Setup
-----
This is the setup project for creating installation binaries. You need the `Wix-toolset <http://wixtoolset.org/>`_ to be able to use the Setup-project. 

Arkade API
==========

The Arkade project provides API-classes for simplified use of the core functionality. There are two API-classes included: Arkade.cs and ArkadeApi.cs. They are located inside the namespace **Arkivverket.Arkade.Core**. Both classes provides the same functionality, the difference is that Autofac is used for dependency injection in the Arkade class. The ArkadeApi class must be instantiated manually. There is an Autofac module that can be used, **Arkivverket.Arkade.Util.ArkadeAutofacModule**, if the client software already is using Autofac for dependency injection. 

This is the signature of the Arkade API class:

.. image:: img/api-signature.png


There are two **RunTests** methods that runs for a given archive, either from a directory structure or a SIP/AIP package file (.tar). After the tests are run, the api returns a **TestSession**. The **TestSession** class contains all necessary information for creating a package with tests results or generating a report. 

A simple test run may look like this:

.. code-block:: C

   
   var arkade = new Arkade();
   var testSession = arkade.RunTests(ArchiveFile.Read("c:\\tmp\\ExampleArchive.tar", ArchiveType.Noark5));
   arkade.SaveReport(testSession, new FileInfo("c:\\tmp\TestReport.html"));
   arkade.CreatePackage(testSession, PackageType.SubmissionInformationPackage);

Also the **TestSession** class contains various information about the testing that has been done. The TestSuite property contains a list of all tests that has been run and their results. 


ADDML
=====

Arkade is built to support ADDML version 8.2. 

List of supported ADDML processes:

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


NOARK 5
=======

Arkade supports the NOARK5 standard.

List of implemeted Noark5 Tests:

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

