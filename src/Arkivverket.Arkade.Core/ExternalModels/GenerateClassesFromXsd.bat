@ECHO off

SET generator="C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools\xsd.exe"
SET baseNamespace=Arkivverket.Arkade.Core.ExternalModels
SET xsdFilePath=xsd

ECHO. && ECHO    Generating C# classes from XSD schema files
ECHO ------------------------------------------------- && ECHO.

:: Parameters: Output filename, namespace (after base namespace), schemafile [, second schemafile]
CALL :GenerateClass Addml.cs, Addml, addml.xsd
CALL :GenerateClass Arkivstruktur.cs, Noark5, arkivstruktur.xsd, metadatakatalog.xsd
CALL :GenerateClass Cpf.cs, Cpf, cpf.xsd, xlink.xsd
CALL :GenerateClass DiasPremis.cs, DiasPremis, DIAS_PREMIS.xsd, xlink.xsd
CALL :GenerateClass Ead.cs, Ead, ead3.xsd
CALL :GenerateClass Info.cs, Info, info.xsd
CALL :GenerateClass Mets.cs, Mets, mets.xsd, xlink.xsd
CALL :GenerateClass Metadata.cs, Metadata, metadata.xsd
CALL :GenerateClass Siard1metadata.cs, Siard1Metadata, siard1metadata.xsd
CALL :GenerateClass TestSessionLog.cs, TestSessionLog, testSessionLog.xsd

ECHO. && PAUSE

EXIT /B 0

:GenerateClass
	ECHO    Generating: %~1
	SET command=%generator% %xsdFilePath%\%~3
    IF NOT [%~4]==[] SET command=%command% %xsdFilePath%\%~4
	SET command=%command% /c /n:%baseNamespace%.%~2
	%command% >nul
    SET lastInputXsd=%~3
    IF NOT [%~4]==[] SET lastInputXsd=%~4
    SET outputFileName=%lastInputXsd:.xsd=%
    MOVE %outputFileName%.cs %~1 >nul
