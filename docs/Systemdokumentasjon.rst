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

* Antall arkiver i arkivstrukturen
* Antall arkivdeler i arkivstrukturen
* Arkiveldene status i arkivstrukturen
* Antall klassifikasjonssystemer i arkivstrukturen
* Antall klasser i arkivsstrukturen
* Antall mapper i arkivstrukturen
* Kontroller at refererte dokumenter eksisterer i uttrekket
* Checksum validations
* Schema Validation of XML
* Antall mapper for hvert år i arkivstrukturen
* Antall registreringer for hvert år i arkivstrukturen
* Antall registreringer uten dokumentbeskrivelse i arkivstrukturen
* Antall dokumentbeskrivelser i arkivstrukturen
* Antall dokumentbeskrivelser uten dokumentobjekt i arkivstrukturen
* Antall dokumentobjekter i arkivstrukturen
* Antall dokumentfiler i arkivuttrekket
* Antall saksparter i arkivstrukturen
* Antall merknader i arkivstrukturen
* Antall kryssreferanser i arkivstrukturen
* Antall presedenser i arkivstrukturen
* Antall korrespondanseparter i arkivstrukturen
* Antall avskrivninger i arkivstrukturen
* Antall dokumentflyter i arkivstrukturen
* Antall mapper som er klassifisert med hver enkelt klasse i arkivstrukturen
* Saksmappenes status i arkivstrukturen
* Antall forskjellige journalposttyper i arkivstrukturen
* Start- og sluttdato for dokumentene i arkivstrukturen
* Antall journalposter i arkivuttrekket
* Kontroll på at det ikke finnes dokumentfiler i arkivuttrekket som mangler referanse fra arkivstrukturen


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
