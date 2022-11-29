using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Resources;
using Serilog;
using System.IO;
using System.Threading.Tasks;
using Arkivverket.Arkade.Core.Logging;

namespace Arkivverket.Arkade.Core.Util
{
    public class IoAccessErrorHandler
    {
        private readonly IStatusEventHandler _statusEventHandler;
        private bool _abortExecution;

        public IoAccessErrorHandler(IStatusEventHandler statusEventHandler)
        {
            _statusEventHandler = statusEventHandler;
            _statusEventHandler.AbortExecutionEvent += OnExecutionAborted;
        }

        public void HandleLostIoAccessToDiskLocation(DirectoryInfo location, IoAccessType ioAccessType)
        {
            _statusEventHandler.RaiseEventIoAccessLost(location, ioAccessType);
            while (!_abortExecution)
            {
                Task.Delay(3000);

                if (location.Exists)
                    break;

                _statusEventHandler.RaiseEventIoAccessLost(location, ioAccessType);
            }

            if (_abortExecution)
            {
                Log.Fatal("User action: aborted execution due to loss of R/W access");
                _abortExecution = false;
                var message = ioAccessType == IoAccessType.Write
                    ? string.Format(ExceptionMessages.WriteAccessLostMessage, location.FullName)
                    : string.Format(ExceptionMessages.ReadAccessLostMessage, location.FullName);
                throw new ArkadeException(message);
            }
        }

        private void OnExecutionAborted(object sender, IoAccessEventArgs eventArgs) => _abortExecution = true;
    }
}
