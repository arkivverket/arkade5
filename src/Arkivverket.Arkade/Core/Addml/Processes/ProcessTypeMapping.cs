using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Arkivverket.Arkade.Core.Addml.Processes
{
    public class ProcessTypeMapping
    {
        private readonly Dictionary<string, Type> _nameToClass;

        public ProcessTypeMapping()
        {
            _nameToClass = Assembly.GetExecutingAssembly().GetTypes()
                    .Where(t => t.GetInterfaces().Contains(typeof(IAddmlProcess))
                                && t.GetConstructor(Type.EmptyTypes) != null)
                    .Select(t => Activator.CreateInstance(t) as IAddmlProcess)
                    .ToDictionary(t => t.GetName(), t => t.GetType());
        }

        public Type GetType(string processName)
        {
            return !_nameToClass.ContainsKey(processName) ? null : _nameToClass[processName];
        }
    }
}