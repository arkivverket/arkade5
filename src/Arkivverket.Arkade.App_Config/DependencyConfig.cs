using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using AutofacSerilogIntegration;
using Autofac;

namespace Arkivverket.Arkade.App_Config
{
    class DependencyConfig
    {
        public static void Configure()
        {
            var builder = new ContainerBuilder();
            builder.RegisterLogger();
            IContainer Container = builder.Build();
        }


    }
}
