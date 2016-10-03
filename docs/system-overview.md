# Arkivverket.Arkade
This is the core library with functions for reading and testing archive extractions, generating reports and creating SIP/AIP-packages.

## Packages:
**Core** - Common classes

**ExternalModels** - Classes generated from xml schemas

**Identify** - Identification classes for reading and identifying an archive extraction

**Tests** - Contains all test classes for testing archive extractions

**Util** - General utilities


# Arkivverket.Arkade.UI

This project provides the graphical user interface of the Arkade 5 software. It is based on WPF, Windows Presentation Foundation. Together with WPF, the application uses the [Prism library](https://github.com/PrismLibrary/Prism) for creating a loosly coupled, maintainable and testable XAML application.  

[Autofac](https://autofac.org) is used as a dependency framework. Bootstrapping of the applications happens in **Bootstrapper.cs**. It is based on the bootstrapper provided by Prism and it loads the Autofac-module provided by the Arkade core library. 

The design and layout is based on Google's [Material Design](https://material.google.com/). This has been implemented with the help of the [MaterialDesignThemes-library](http://materialdesigninxaml.net/). Note that the user interface is only inspired by the material design, not neccessary strictly following it in every situation. 
