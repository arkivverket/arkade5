using System;
using System.IO;

namespace Arkivverket.Arkade.Core.Base;

public class ArchiveProcessing()
{
    public Archive Archive
    {
        get => _archive;
        set
        {
            if (_archive is not null)
                throw new ArgumentException("Archive already set");
            _archive = value;
        }
    }

    public InputDiasPackage InputDiasPackage
    {
        get => _inputDiasPackage;
        set
        {
            PreventDifferentArchiveInstances(value.Archive);
            _archive = value.Archive;
            _inputDiasPackage = value;
        }
    }

    public OutputDiasPackage OutputDiasPackage
    {
        get => _outputDiasPackage;
        set
        {
            PreventDifferentArchiveInstances(value.Archive);
            _archive = value.Archive;
            _outputDiasPackage = value;
        }
    }

    public TestSession TestSession
    {
        get => _testSession;
        set
        {
            PreventDifferentArchiveInstances(value.Archive);
            _archive = value.Archive;
            _testSession = value;
        }
    }

    public DirectoryInfo ProcessingDirectory => _processingDirectory ?? CreateProcessingDirectory();

    private Archive _archive;
    private InputDiasPackage _inputDiasPackage;
    private OutputDiasPackage _outputDiasPackage;
    private TestSession _testSession;
    private DirectoryInfo _processingDirectory;

    private DirectoryInfo CreateProcessingDirectory()
    {
        string workDirectoryFullName = ArkadeProcessingArea.WorkDirectory.FullName;
        var nowTimeStampString = DateTime.Now.ToString("yyyyMMddHHmmss");

        _processingDirectory = new DirectoryInfo(Path.Combine(workDirectoryFullName, nowTimeStampString));
        _processingDirectory.Create();

        return _processingDirectory;
    }

    private void PreventDifferentArchiveInstances(Archive incomingArchiveObject)
    {
        if (_archive != null && !_archive.Equals(incomingArchiveObject))
            throw new ArgumentException("Archive instance mismatch");
    }
}
