namespace Arkivverket.Arkade.Core.Addml.Processes
{
    public interface IAddmlFileProcess
    {
        void Run(FlatFile flatFile);
    }
}