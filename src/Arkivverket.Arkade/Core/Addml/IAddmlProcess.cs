using System.Collections.Generic;

namespace Arkivverket.Arkade.Core.Addml
{
    public interface IAddmlProcess
    {
        /// <summary>
        /// Return results from the process. 
        /// </summary>
        /// <returns></returns>
        TestRun GetTestRun();

        /// <summary>
        /// Let the process clean up when last line of a file has been read. 
        /// </summary>
        void EndOfFile();
    }
}