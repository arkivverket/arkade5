using System;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Base.Archives;
using Arkivverket.Arkade.Core.Base.Siard;
using Arkivverket.Arkade.Core.Logging;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Testing.Noark5;
using Serilog;
using static Arkivverket.Arkade.Core.Util.ArkadeConstants;

namespace Arkivverket.Arkade.Core.Identify
{
    public class TestSessionFactory
    {
        private readonly ILogger _log = Log.ForContext<TestSessionFactory>();
        private readonly IStatusEventHandler _statusEventHandler;

        public TestSessionFactory(IStatusEventHandler statusEventHandler)
        {
            _statusEventHandler = statusEventHandler;
        }

        public TestSession NewSession(Archive archive)
        {
            if (archive is Noark5Archive { AddmlXmlUnit.Schema: ArkadeBuiltInXmlSchema })
            {
                // TODO: Use version info from Version object in messages
                _statusEventHandler?.RaiseEventOperationMessage(
                    Noark5Messages.MissingAddmlSchema,
                    string.Format(Noark5Messages.UsingBuiltInAddmlSchemaFile, BuiltInAddmlSchemaVersion),
                    OperationMessageStatus.Warning);
                Log.Warning(string.Format(Noark5Messages.InternalSchemaFileIsUsed,
                    AddmlXsdFileName, BuiltInAddmlSchemaVersion));
            }

            var testSession = new TestSession(archive.ProcessingDirectory.CreateSubdirectory("tmp-testresults"));

            if (archive is Noark5Archive or SiardArchive)
            {
                return testSession;
            }

            if(archive is not AddmlBasedArchive addmlBasedArchive)
                throw new ArgumentException("Archive must be an AddmlBasedArchive from here ..."); // TODO: Follow up
            
            AddmlInfo addml = addmlBasedArchive.AddmlInfo;

            try
            {
                var addmlDefinitionParser = new AddmlDefinitionParser(addml, addmlBasedArchive.Content, _statusEventHandler);

                testSession.AddmlDefinition = addmlDefinitionParser.GetAddmlDefinition();
            }
            catch (Exception exception)
            {
                var message = string.Format(ExceptionMessages.FileNotRead, addmlBasedArchive.AddmlXmlUnit.File.Name) + " " + exception.Message;
                _log.Warning(message);//exception, message);
                _statusEventHandler.RaiseEventOperationMessage(null, message, OperationMessageStatus.Error);
            }

            return testSession;
        }
    }
}
