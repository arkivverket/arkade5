@echo off
echo "============================ GENERATE C# classes ============================"

echo "Generate classes for info.xsd"
"C:\Program Files (x86)\Microsoft SDKs\Windows\v8.1A\bin\NETFX 4.5.1 Tools\xsd.exe" /nologo info.xsd /c /n:Arkivverket.Arkade.ExternalModels.Info
copy /y info.cs ..\Info.cs
del info.cs

echo "Generate classes for addml.xsd"
"C:\Program Files (x86)\Microsoft SDKs\Windows\v8.1A\bin\NETFX 4.5.1 Tools\xsd.exe" /nologo addml.xsd /c /n:Arkivverket.Arkade.ExternalModels.Addml
copy /y Addml.cs ..\Addml.cs
del addml.cs

echo "Generate classes for arkivstruktur.xsd"
"C:\Program Files (x86)\Microsoft SDKs\Windows\v8.1A\bin\NETFX 4.5.1 Tools\xsd.exe" /nologo arkivstruktur.xsd metadatakatalog.xsd /c /n:Arkivverket.Arkade.ExternalModels.Noark5
copy /y arkivstruktur_metadatakatalog.cs ..\Arkivstruktur.cs
del arkivstruktur_metadatakatalog.cs

echo "Generate classes for testSessionLog.xsd"
"C:\Program Files (x86)\Microsoft SDKs\Windows\v8.1A\bin\NETFX 4.5.1 Tools\xsd.exe" /nologo testSessionLog.xsd /c /n:Arkivverket.Arkade.ExternalModels.TestSessionLog
copy /y testSessionLog.cs ..\TestSessionLog.cs
del testSessionLog.cs

echo "Generate classes for DIAS_METS.xsd"
"C:\Program Files (x86)\Microsoft SDKs\Windows\v8.1A\bin\NETFX 4.5.1 Tools\xsd.exe" /nologo DIAS_METS.xsd xlink.xsd /c /n:Arkivverket.Arkade.ExternalModels.DiasMets
copy /y DIAS_METS_xlink.cs ..\DiasMets.cs
del DIAS_METS_xlink.cs 

echo "Generate classes for DIAS_PREMIS.xsd"
"C:\Program Files (x86)\Microsoft SDKs\Windows\v8.1A\bin\NETFX 4.5.1 Tools\xsd.exe" /nologo DIAS_PREMIS.xsd xlink.xsd /c /n:Arkivverket.Arkade.ExternalModels.DiasPremis
copy /y DIAS_PREMIS_xlink.cs ..\DiasPremis.cs
del DIAS_PREMIS_xlink.cs 
pause
