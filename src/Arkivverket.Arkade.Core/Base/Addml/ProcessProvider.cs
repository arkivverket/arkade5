using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base.Addml
{
    public static class ProcessProvider
    {
        public static IEnumerable<IAddmlProcess> GetAllProcesses()
        {
            return Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.GetInterfaces().Contains(typeof(IAddmlProcess))
                            && t.GetConstructor(Type.EmptyTypes) != null)
                .Select(t => Activator.CreateInstance(t) as IAddmlProcess);
        }

        public static IEnumerable<TestId> GetAllTestIds()
        {
            return GetAllProcesses().Select(p => p.GetId());
        }
    }
}
