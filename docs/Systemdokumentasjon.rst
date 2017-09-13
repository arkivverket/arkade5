*******************
Systemdokumentasjon
*******************

Source code
===========

The source code is located at the GitHub-repository: https://github.com/arkivverket/arkade5/

Arkade is developed with .Net and C#. The solution-file (.sln) is compatible with Visual Studio 2015 and above. 

Overview
--------
Arkade provides mainly three different functions: 

* Archive testing
* Report generator
* Package creator

These functions are exposed in the API and the graphical user interface project is also using the API-class to interact with the core functions.

.. image:: img/project-overview.png

Below is a brief description of each project in the solution. 


Arkivverket.Arkade
------------------
This is the core library with functions for testing archive extractions, generating reports and creating SIP/AIP-packages.

The most notable classes in the core project are the test engines, package creator and report generator:

* Arkivverket.Arkade.Core.Noark5.Noark5TestEngine
* Arkivverket.Arkade.Core.Addml.AddmlDatasetTestEngine
* Arkivverket.Arkade.Core.InformationPackageCreator
* Arkivverket.Arkade.Report.HtmlReportGenerator

A short description of the packages in the core project:

**Core** - Domain classes

**ExternalModels** - Classes generated from xml schemas

**Identify** - Identification classes for reading and identifying an archive extraction

**Logging** - Classes related to logging of events during testing

**Metadata** - Contains classes related to creating metadata files for archive extractions

**Report** - Classes for generating test reports

**Resource** - Various resource files, language files, images etc.

**Tests** - Contains test classes for testing archive extractions

**Util** - General utilities


Arkivverket.Arkade.UI
---------------------

This project provides the graphical user interface of the Arkade 5 software. It is based on WPF, Windows Presentation Foundation. 
Together with WPF, the application uses the Prism_ library for creating a loosly coupled, maintainable and testable XAML application.  

Autofac_ is used as a dependency framework. Bootstrapping of the applications happens in **Bootstrapper.cs**. It is based on the bootstrapper provided by Prism and it loads the Autofac-module provided by the Arkade core library. 

The design and layout is based on Google's Material_ Design. This has been implemented with the help of the `MaterialDesignThemes-library <http://materialdesigninxaml.net/>`_. Note that the user interface is only inspired by the material design, not neccessary strictly following it in every situation. 


.. _Prism: https://github.com/PrismLibrary/Prism
.. _Autofac: https://autofac.org
.. _Material: https://material.google.com/

Arkivverket.Arkade.Test
-----------------------
This project contains the unit tests and other tests classes for the project. Unit tests are created with xUnit. 

Setup
-----
This is the setup project for creating installation binaries. You need the `Wix-toolset <http://wixtoolset.org/>`_ to be able to use the Setup-project. 

Signing the installation file
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

In order to sign the msi file, you need the **signtool.exe** on your computer. This can be installed together with the Visual Studio. The ClickOnce Publishing package contains this tools.

Signing of the installation file is done by the continuous integration server. Signing is performed with a certificate provided by Arkivverket. The following command is run to sign the installation file::

    "C:\Program Files (x86)\Windows Kits\8.1\bin\x64\signtool.exe" sign /f PATH_TO_CERTIFICATE_FILE.pfx /p CERTIFICATE_PASSWORD src\Setup\bin\Release\Setup.msi

Sample.ConsoleApp
------------------------------
This is a sample application, which demonstrates the use of the Arkade API.

Porting to other platforms
--------------------------
For now the application is only developed for use on the Microsoft Windows platform. However, with the new `.Net Core project <https://www.microsoft.com/net/core/platform>`_ from Microsoft, a cross platform application should be possible when the framework and tools has matured. The .Net Core platform allows running .net applications on linux and mac in addition to windows. 

The Arkade project has few external dependencies and it should be possible to either update them to newer versions when they are compatible with .net core or replace them with other compatible libraries. Currently there are only three external libraries in use: 

* `SharpZipLib <https://icsharpcode.github.io/SharpZipLib/>`_
* Autofac_
* `Serilog <https://serilog.net/>`_

In addition there are some of the .net packages that has been restructured, deprecated or removed that need to be fixed before the application is fully cross platform compatible.

The graphical user interface can be a challenge to port, currently Microsoft has no plans for porting the Windows Presentation Framework to other platforms. This means that for creating a linux desktop app, you might have to recreate the user interface with another graphics library. Preferably a library that works on both linux and mac.

Some useful links regarding porting to .net core: 
* https://blogs.msdn.microsoft.com/dotnet/2016/02/10/porting-to-net-core/
* https://marketplace.visualstudio.com/items?itemName=ConnieYau.NETPortabilityAnalyzer


Arkade API
==========

The Arkade project provides API-classes for simplified use of the core functionality. There are two API-classes included: Arkade.cs and ArkadeApi.cs. They are located inside the namespace **Arkivverket.Arkade.Core**. Both classes provides the same functionality, the difference is that Autofac_ is used for dependency injection in the Arkade class. The ArkadeApi class must be instantiated manually. There is an Autofac module that can be used, **Arkivverket.Arkade.Util.ArkadeAutofacModule**, if the client software already is using Autofac for dependency injection. 

This is the signature of the Arkade API class:

.. image:: img/api-signature.png

There are two **RunTests** methods that runs for a given archive, either from a directory structure or a SIP/AIP package file (.tar). After the tests are run, the api returns a **TestSession**. The **TestSession** class contains all necessary information for creating a package with tests results or generating a report. 

A simple test run may look like this:

.. code-block:: C

   
   var arkade = new Arkade();
   var testSession = arkade.RunTests(ArchiveFile.Read("c:\\tmp\\ExampleArchive.tar", ArchiveType.Noark5));
   arkade.SaveReport(testSession, new FileInfo("c:\\tmp\TestReport.html"));
   arkade.CreatePackage(testSession, PackageType.SubmissionInformationPackage);

The **TestSession** class contains various information about the testing that has been done. The TestSuite property contains a list of all tests that has been run and their results. 

See the sample project (Sample.ConsoleApp) in the source code, for a complete example that runs testing on a Noark5 archive.


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

List of implemented Noark5 Tests:

* Analyse 01 - Antall arkiver i arkivstrukturen
* Analyse 02 - Antall arkivdeler i arkivstrukturen
* Analyse 03 - Arkivdelen[e]s status i arkivstrukturen
* Analyse 04 - Antall klassifikasjonssystemer i arkivstrukturen
* Analyse 05 - Antall klasser i arkivstrukturen
* Analyse 06 - Antall klasser uten underklasser eller mapper i det primære klassifikasjonssystemet i arkivstrukturen
* Analyse 07 - Antall mapper i arkivstrukturen
* Analyse 08 - Antall mapper for hvert år i arkivstrukturen
* Analyse 09 - Antall mapper som er klassifisert med hver enkelt klasse i arkivstrukturen
* Analyse 10 - Antall mapper uten undermapper eller registreringer i arkivstrukturen
* Analyse 11 - Saksmappenes status i arkivstrukturen
* Analyse 12 - Antall registreringer i arkivstrukturen
* Analyse 13 - Antall forskjellige journalposttyper i arkivstrukturen
* Analyse 14 - Antall registreringer for hvert år i arkivstrukturen
* Analyse 15 - Antall registreringer som er klassifisert med hver enkelt klasse i arkivstrukturen
* Analyse 16 - Antall registreringer uten dokumentbeskrivelse i arkivstrukturen
* Analyse 17 - Journalpostenes status i arkivstrukturen
* Analyse 18 - Antall dokumentbeskrivelser i arkivstrukturen
* Analyse 19 - Antall dokumentbeskrivelser uten dokumentobjekt i arkivstrukturen
* Analyse 20 - Dokumentbeskrivelsenes status i arkivstrukturen
* Analyse 21 - Antall dokumentobjekter i arkivstrukturen
* Analyse 22 - Start- og sluttdato for dokumentene i arkivstrukturen
* Analyse 23 - Antall dokumentfiler i arkivuttrekket
* Analyse 24 - Antall dokumenter i arkivuttrekket fordelt på dokumentformat
* Analyse 25 - Antall dokumentfiler som blir referert til av flere enn ett dokumentobjekt
* Analyse 26 - Antall saksparter i arkivstrukturen
* Analyse 27 - Antall merknader i arkivstrukturen
* Analyse 28 - Antall kryssreferanser i arkivstrukturen
* Analyse 29 - Antall presedenser i arkivstrukturen
* Analyse 30 - Antall korrespondanseparter i arkivstrukturen
* Analyse 31 - Antall avskrivninger i arkivstrukturen
* Analyse 32 - Antall dokumentflyter i arkivstrukturen
* Analyse 33 - Eventuelt - antall skjerminger i arkivstrukturen
* Analyse 34 - Eventuelt - antall graderinger i arkivstrukturen
* Analyse 35 - Eventuelt - antall kassasjonsvedtak i arkivstrukturen
* Analyse 36 - Eventuelt - antall utførte kassasjoner i arkivstrukturen
* Analyse 37 - Eventuelt - antall konverterte dokumenter i arkivstrukturen
* Analyse 38 - Antall journalposter i arkivuttrekket
* Analyse 39 - Start- og sluttdato i arkivuttrekket
* Kontroll 40 - Kontroll av sjekksummene i arkivuttrekk.xml
* Kontroll 41 - Validering av arkivstruktur.xml
* Kontroll 42 - Validering av endringslogg.xml
* Kontroll 43 - Kontroll på at mappene bare er knyttet til klasser uten underklasser i arkivstrukturen
* Kontroll 44 - Kontroll på at registreringer bare er knyttet til klasser uten underklasser i arkivstrukturen
* Kontroll 45 - Kontroll av sjekksummer
* Kontroll 46 - Kontroll på om dokumentobjektene i arkivstrukturen refererer til eksisterende dokumentfiler i arkivuttrekket
* Kontroll 47 - Kontroll på at det ikke finnes dokumentfiler i arkivuttrekket som mangler referanse fra arkivstrukturen
* Kontroll 48 - Kontroll av systemidentifikasjonene i arkivstrukturen
* Kontroll 49 - Kontroll av referansene til arkivdel i arkivstrukturen
* Kontroll 50 - Kontroll av referansene til sekundær klassifikasjon i arkivstrukturen
* Kontroll 51 - Kontroll av referansene i endringsloggen
